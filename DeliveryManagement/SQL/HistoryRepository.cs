using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Detrack.Infrastructure.DB;
using Detrack.Infrastructure.Logging;
using Detrack.Model;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;

namespace Detrack.Data.SQL
{
	public class HistoryRepository
	{
		private static readonly ILog log = LogManager.GetLog("HistoryRepository");
		private readonly IDatabase database = new Database(new SqlDatabaseAdapter()) { Log = log };

		public bool SaveCompletedDelivery(Delivery baseItem)
		{
			try
			{
				using (var connection = database.NewConnection())
				{
					using (IDbCommand command = connection.CreateCommand())
					{
						var update = DataItemExists(baseItem.Do);

						if (update)
							command.CommandText = @"UPDATE [@DEL_HIST_HEADER] 
														SET
															U_date = ?, 
															U_Address = ?, 
															U_Delivery_Time = ?, 
															U_Deliver_To = ?, 
															U_Phone = ?, 
															U_Notify_Email = ?, 
															U_Notify_Url = ?, 
															U_Assign_To = ?, 
															U_Instructions = ?,
															U_Zone = ?,
															U_Reason = ?,
															U_Note = ?,
															U_Received_By = ?,
															U_Image = ?,
															U_View_Image_Url = ?,
															U_Status = ?,
															U_Time = ?,
															U_Pod_Lat = ?,
															U_Pod_Lng = ?,
															U_Pod_Address = ?
														WHERE
															U_do = ?";
						else
							command.CommandText = @"INSERT INTO [@DEL_HIST_HEADER] 
													VALUES ( 
														(SELECT MAX(CODE)+1 FROM [@DEL_HIST_HEADER]), 
														(SELECT MAX(CODE)+1 FROM [@DEL_HIST_HEADER]),
														?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

						connection.Open();

						if (update)
							database.ExecuteNonQuery(command, baseItem.Date, baseItem.Address, baseItem.Delivery_Time, baseItem.Deliver_To,
													baseItem.Phone, baseItem.Notify_Email, baseItem.Notify_Url, baseItem.Assign_To, baseItem.Instructions,
													baseItem.Zone, baseItem.Reason, baseItem.Note, baseItem.Received_By, baseItem.Image.ToString(), baseItem.View_Image_Url,
													baseItem.Status, baseItem.Time.GetValueOrDefault().ToShortTimeString(), baseItem.Pod_Lat.ToString(),
													baseItem.Pod_Lng.ToString(), baseItem.Pod_Address, int.Parse(baseItem.Do));
						else
							database.ExecuteNonQuery(command, baseItem.Date, int.Parse(baseItem.Do), baseItem.Address, baseItem.Delivery_Time, baseItem.Deliver_To, null, null,
													baseItem.Phone, baseItem.Notify_Email, baseItem.Notify_Url, baseItem.Assign_To, baseItem.Instructions,
													baseItem.Zone, baseItem.Reason, baseItem.Note, baseItem.Received_By, null, baseItem.Image.ToString(), baseItem.View_Image_Url,
													baseItem.Status, baseItem.Time.GetValueOrDefault().ToShortTimeString(), baseItem.Pod_Lat.ToString(),
													baseItem.Pod_Lng.ToString(), baseItem.Pod_Address);

						foreach (var item in baseItem.Items)
						{
							SaveItems(item, baseItem.Do);
						}
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				log.Error(ex, "Error saving completed item {0}", baseItem.Do);
				return false;
			}

		}

		public bool SaveCompletedCollection(Collection baseItem)
		{
			try
			{
				using (var connection = database.NewConnection())
				{
					using (IDbCommand command = connection.CreateCommand())
					{
						var update = DataItemExists(baseItem.Do);

						if (update)
							command.CommandText = @"UPDATE [@DEL_HIST_HEADER] 
														SET
															U_date = ?, 
															U_Address = ?, 
															U_Collection_Time = ?, 
															U_Collect_From = ?, 
															U_Phone = ?, 
															U_Notify_Email = ?, 
															U_Notify_Url = ?, 
															U_Assign_To = ?, 
															U_Instructions = ?,
															U_Zone = ?,
															U_Reason = ?,
															U_Note = ?,
															U_Sent_By = ?,
															U_Image = ?,
															U_View_Image_Url = ?,
															U_Status = ?,
															U_Time = ?,
															U_Pod_Lat = ?,
															U_Pod_Lng = ?,
															U_Pod_Address = ?
														WHERE
															U_do = ?";
						else
							command.CommandText = @"INSERT INTO [@DEL_HIST_HEADER] 
													VALUES ( 
														(SELECT MAX(CODE)+1 FROM [@DEL_HIST_HEADER]), 
														(SELECT MAX(CODE)+1 FROM [@DEL_HIST_HEADER]),
														?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

						connection.Open();

						if (update)
							database.ExecuteNonQuery(command, baseItem.Date, baseItem.Address, baseItem.Collection_Time, baseItem.Collect_From,
													baseItem.Phone, baseItem.Notify_Email, baseItem.Notify_Url, baseItem.Assign_To, baseItem.Instructions,
													baseItem.Zone, baseItem.Reason, baseItem.Note, baseItem.Sent_By, baseItem.Image.ToString(), baseItem.View_Image_Url,
													baseItem.Status, baseItem.Time.GetValueOrDefault().ToShortTimeString(), baseItem.Pod_Lat.ToString(),
													baseItem.Pod_Lng.ToString(), baseItem.Pod_Address, int.Parse(baseItem.Do));
						else
							database.ExecuteNonQuery(command, baseItem.Date, int.Parse(baseItem.Do), baseItem.Address, null, null, baseItem.Collection_Time, baseItem.Collect_From,
													baseItem.Phone, baseItem.Notify_Email, baseItem.Notify_Url, baseItem.Assign_To, baseItem.Instructions,
													baseItem.Zone, baseItem.Reason, baseItem.Note, null, baseItem.Sent_By, baseItem.Image.ToString(), baseItem.View_Image_Url,
													baseItem.Status, baseItem.Time.GetValueOrDefault().ToShortTimeString(), baseItem.Pod_Lat.ToString(),
													baseItem.Pod_Lng.ToString(), baseItem.Pod_Address);

						foreach (var item in baseItem.Items)
						{
							SaveItems(item, baseItem.Do);
						}
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}

		}

		public bool SaveItems(Item item, string doNumber)
		{
			try
			{
				using (var connection = database.NewConnection())
				{
					using (IDbCommand command = connection.CreateCommand())
					{
						var update = ItemExists(item, doNumber);

						if (update)
							command.CommandText = @"UPDATE [@DEL_HIST_ITEMS] 
													SET
														U_sku = ?, 
														U_desc = ?, 
														U_qty = ?, 
														U_reject = ?, 
														U_reason= ?
													WHERE
														U_do = ? 
													AND
														U_sku = ?";
						else
							command.CommandText = @"INSERT INTO [@DEL_HIST_ITEMS] 
												VALUES ( 
													(SELECT MAX(CODE)+1 FROM [@DEL_HIST_ITEMS]), 
													(SELECT MAX(CODE)+1 FROM [@DEL_HIST_ITEMS]),
													?, ?, ?, ?, ?, ?);";

						connection.Open();

						if (update)
							database.ExecuteNonQuery(command, item.Sku, item.Desc, item.Qty, item.Reject, item.Reason, int.Parse(doNumber), item.Sku);
						else
							database.ExecuteNonQuery(command, int.Parse(doNumber), item.Sku, item.Desc, item.Qty, item.Reject, item.Reason);
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				log.Error(ex, "Error saving item: {0} for object {1}.", item.Sku, doNumber);
				return false;
			}
		}

		public bool DataItemExists(string doNumber)
		{
			return (database.ExecuteCount("[@DEL_HIST_HEADER]", "U_do", doNumber) > 0);
		}

		public bool ItemExists(Item item, string doNumber)
		{
			try
			{
				using (var connection = database.NewConnection())
				{
					using (var command = connection.CreateCommand())
					{
						command.CommandText = @"SELECT COUNT(*) FROM [@DEL_HIST_ITEMS] WHERE U_do = ? AND U_sku = ?";

						connection.Open();

						var result = database.ExecuteScalar<long>(command, doNumber, item.Sku);

						return result > 0;
					}
				}
			}
			catch (Exception ex)
			{
				log.Error(ex, "Error retrieving items count from database.");
				return false;
			}		
		}
	}
}
