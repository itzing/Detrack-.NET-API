using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Detrack.Data;
using Detrack.Data.SQL;
using Detrack.Infrastructure.Exceptions;
using Detrack.Infrastructure.Logging;
using Detrack.Model;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;

namespace Detrack.Main
{
	public class DetrackSynchronizer : ApplicationContext
	{
		private static readonly ILog log = LogManager.GetLog("DetrackSynchronizer");

		public DetrackSynchronizer()
		{
			Console.WriteLine("==========START===========");

			SendItems<Delivery>(SyncStatus.New);
			SendItems<Delivery>(SyncStatus.Edit);
			SendItems<Delivery>(SyncStatus.Delete);

			SendItems<Collection>(SyncStatus.New);
			SendItems<Collection>(SyncStatus.Edit);
			SendItems<Collection>(SyncStatus.Delete);

			UpdateCompleted<Delivery>();
			UpdateCompleted<Collection>();

			Console.WriteLine("==========END===========");

			throw new ApplicationShutdownException();
		}

		private void SendItems<T>(SyncStatus status) where T: BaseDataObject
		{
			var detrackRepository = new DetrackRepository<T>();
			var deliveryLogRepository = new DeliveryLogRepository<T>();

			var itemsSource = deliveryLogRepository.GetItems(status).GetEnumerator();

			foreach (var deliveries in GetNextBatch(100, itemsSource))
			{
				if (deliveries.Count == 0)
					return;

				BaseResponse response = null;

				switch (status)
				{
					case SyncStatus.New:
						response = detrackRepository.Add(deliveries);
						break;
					case SyncStatus.Edit:
						response = detrackRepository.EditItems(deliveries);
						break;
					case SyncStatus.Delete:
						response = detrackRepository.DeleteItems(deliveries);
						break;
				}

				if (response!= null && response.Info.Status != Status.ok.ToString())
				{
					log.Info("Add request failed: {0} - {1}", response.Info.Error.Code, response.Info.Error.Message);
					return;
				}

				if (response == null)
				{
					log.Info("No response");
					return;
				}

				foreach (var operationResult in response.Results)
				{
					if (operationResult.Status != Status.ok.ToString())
					{
						deliveryLogRepository.ChangeItemStatus(operationResult.Do, SyncStatus.Error, operationResult.Errors);

						log.Info("{0} item if type {1} DO#{2}: FAILED", status, typeof(T), operationResult.Do);
						
						foreach (var error in operationResult.Errors)
						{
							log.Info("Error: {0} - {1}", error.Code, error.Message);
						}
					}
					else
					{
						var endStatus = status == SyncStatus.Delete ? SyncStatus.Deleted : SyncStatus.Processing;

						deliveryLogRepository.ChangeItemStatus(operationResult.Do, endStatus);
						log.Info("{0} item if type {1} DO#{2}: SUCCESS", status, typeof(T), operationResult.Do);
					}
				}
			}
		}

		private void UpdateCompleted<T>() where T : BaseDataObject
		{
			var detrackRepository = new DetrackRepository<T>();
			var deliveryLogRepository = new DeliveryLogRepository<T>();

			var processingItems = deliveryLogRepository.GetItems(SyncStatus.Processing);

			var response = detrackRepository.GetItems(processingItems);

			foreach (var operationResult in response.Results)
			{
				if (operationResult.Status != Status.ok.ToString())
				{
					log.Info("Item of type {0} DO#{1} failed to retrieve", typeof(T), operationResult.Do);
				}
				else
				{
					if (!operationResult.Completed) continue;

					if (operationResult.Delivery != null)
					{
						var historyRepository = new HistoryRepository();
						historyRepository.SaveCompletedDelivery(operationResult.Delivery);

						foreach (ImageType imageType in (ImageType[])Enum.GetValues(typeof(ImageType)))
						{
							detrackRepository.GetImage(operationResult.Delivery, imageType);
						}
					}
					else
					{
						var historyRepository = new HistoryRepository();
						historyRepository.SaveCompletedCollection(operationResult.Collection);
						foreach (ImageType imageType in (ImageType[])Enum.GetValues(typeof(ImageType)))
						{
							detrackRepository.GetImage(operationResult.Collection, imageType);
						}
					}


					deliveryLogRepository.ChangeItemStatus(operationResult.Do, SyncStatus.Completed);
					log.Info("Item of type {0} DO#{1} completed.", typeof(T), operationResult.Do);
				}
			}
		}

		public IEnumerable<List<T>> GetNextBatch<T>(int batchSize, IEnumerator<T> source)
		{
			var batch = new List<T>();

			while (source.MoveNext())
			{
				if (batch.Count == batchSize)
					batch.Clear();

				batch.Add(source.Current);
				if (batch.Count >= batchSize)
					yield return batch;
			}

			yield return batch;
		}
	}
}