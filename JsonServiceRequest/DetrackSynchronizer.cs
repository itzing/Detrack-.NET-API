using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Detrack.Data;
using Detrack.Data.SQL;
using Detrack.Infrastructure.Exceptions;
using Detrack.Model;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;

namespace Detrack.Main
{
	public class DetrackSynchronizer : ApplicationContext
	{
		private readonly IEnumerator<Delivery> deliverySource;
		private readonly IEnumerator<Collection> collectionSource;
		private readonly DeliveryRepository deliveryRepository;
		private readonly CollectionRepository collectionRepository;
		private readonly SqlDatabase database;

		public DetrackSynchronizer()
		{
			database = new SqlDatabase();
			deliveryRepository = new DeliveryRepository();
			collectionRepository = new CollectionRepository();

			deliverySource = database.GetDeliveries().GetEnumerator();
			collectionSource = database.GetCollections().GetEnumerator();

			Console.WriteLine("==========START===========");

			SendDeliveries();
			SendCollections();

			Console.WriteLine("==========END===========");

			Console.ReadLine();
			throw new ApplicationShutdownException();
		}

		private void SendDeliveries()
		{
			foreach (var deliveries in GetNextBatch(100, deliverySource))
			{
				var response = deliveryRepository.Add(deliveries);

				if (response.Info.Status != Status.ok.ToString())
					Trace.TraceError("Add request failed: {0} - {1}", response.Info.Error.Code, response.Info.Error.Message);

				foreach (var operationResult in response.Results)
				{
					if (operationResult.Status != Status.ok.ToString())
					{
						database.AddError(operationResult);
						Trace.TraceInformation("Add Delivery DO#{0}: FAILED", operationResult.Do);
						
						foreach (var error in operationResult.Errors)
						{
							Trace.TraceInformation("Error: {0} - {1}", error.Code, error.Message);
						}
					}
					else
					{
						Trace.TraceInformation("Add Delivery DO#{0}: SUCCESS", operationResult.Do);
					}
				}
			}
		}

		private void SendCollections()
		{
			foreach (var collections in GetNextBatch(100, collectionSource))
			{
				var response = collectionRepository.Add(collections);

				if (response.Info.Status != Status.ok.ToString())
					Trace.TraceError("Add request failed: {0} - {1}", response.Info.Error.Code, response.Info.Error.Message);

				foreach (var operationResult in response.Results)
				{
					if (operationResult.Status != Status.ok.ToString())
					{
						database.AddError(operationResult);
						Trace.TraceInformation("Add Collection DO#{0}: FAILED", operationResult.Do);
						foreach (var error in operationResult.Errors)
						{
							Trace.TraceInformation("Error: {0} - {1}", error.Code, error.Message);
						}
					}
					else
					{
						Trace.TraceInformation("Add Collection DO#{0}: SUCCESS", operationResult.Do);
					}
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
