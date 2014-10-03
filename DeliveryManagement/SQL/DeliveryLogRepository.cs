using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Detrack.Infrastructure.DB;
using Detrack.Infrastructure.Logging;
using Detrack.Model;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;

namespace Detrack.Data.SQL
{
	public class DeliveryLogRepository<T> where T : BaseDataObject
	{
		private static readonly ILog log = LogManager.GetLog("DeliveryLogRepository");

		private readonly string connectionString = ConfigurationManager.ConnectionStrings["Detrack"].ToString();
		private readonly IDatabase database = new Database(new SqlDatabaseAdapter()) { Log = log };

		public IEnumerable<T> GetItems(SyncStatus status)
		{
			return ItemType == ItemType.Delivery ? GetDeliveries(status).Cast<T>() : GetCollections(status).Cast<T>();
		}
		
		public IEnumerable<Delivery> GetDeliveries(SyncStatus status)
		{
			using (IDbConnection connection = database.NewConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{ 
					command.CommandText = @"select DH.DocDate as date, 
									DocEntry as do, 
									REPLACE(DH.Address2,CHAR(13),' ') as address, 
									'' as delivery_time, 
									DH.CardName as deliver_to, 
									DH.U_Phone1 as phone, 
									'brendan.bell32@gmail.com' as notify_email, 
									'' as notify_url, 
									'' as assign_to, 
									DH.Header as instructions, 
									'' as zone, 
									DH.TrnspCode
							from odln DH
							inner join [@DEL_LOG] DL on DH.DocEntry = DL.U_DocEntry
							Where DL.U_DocStatus = ? AND DL.U_DeliveryType=1
							ORDER by DocEntry ASC";

					connection.Open();
			
					foreach (var row in database.ExecuteReader(command, status))
					{
						var delivery = new Delivery
						{
							Date = DateTime.Parse(row.As<string>("date")),
							Do = row.As<string>("do"),
							Address = row.As<string>("address"),
							Delivery_Time = row.As<string>("delivery_time"),
							Deliver_To = row.As<string>("deliver_to"),
							Phone = row.As<string>("phone"),
							Notify_Email = row.As<string>("notify_email"),
							Notify_Url = row.As<string>("notify_url"),
							Assign_To = row.As<string>("assign_to"),
							Instructions = row.As<string>("instructions"),
							Zone = row.As<string>("zone")
						};

						delivery.Items = GetItems(delivery.Do);

						yield return delivery;
					}
				}
			}
		}


		public IEnumerable<Collection> GetCollections(SyncStatus status)
		{
			using (IDbConnection connection = database.NewConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{ 
					command.CommandText = @"select DH.DocDate as date, 
										DocEntry as do, 
										REPLACE(DH.Address2,CHAR(13),' ') as address, 
										'' as collection_time, 
										DH.CardName as collect_from, 
										DH.U_Phone1 as phone, 
										'brendan.bell32@gmail.com' as notify_email, 
										'' as notify_url,
										'' as assign_to, 
										DH.Header as instructions, 
										'' as zone, DH.TrnspCode
									from odln DH
										inner join [@DEL_LOG] DL on DH.DocEntry = DL.U_DocEntry
										Where DL.U_DocStatus = ? AND DL.U_DeliveryType=2
										ORDER by DocEntry ASC";

					connection.Open();

					foreach (var row in database.ExecuteReader(command, status))
					{
						var collection = new Collection
						{
							Date = DateTime.Parse(row.As<string>("date")),
							Do = row.As<string>("do"),
							Address = row.As<string>("address"),
							Collection_Time = row.As<string>("collection_time"),
							Collect_From = row.As<string>("collect_from"),
							Phone = row.As<string>("phone"),
							Notify_Email = row.As<string>("notify_email"),
							Notify_Url = row.As<string>("notify_url"),
							Assign_To = row.As<string>("assign_to"),
							Instructions = row.As<string>("instructions"),
							Zone = row.As<string>("zone")
						};

						collection.Items = GetItems(collection.Do);

						yield return collection;
					}
				}
			}
		}

		private List<Item> GetItems(string doNumber)
		{
			try
			{
				var items = new List<Item>();

				using (IDbConnection connection = database.NewConnection())
				{
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = @"SELECT 
													DocEntry as do, 
													LineNum as line_no, 
													vendorNum as sku, 
													dscription as 'desc', 
													quantity as qty 
												FROM dln1
												WHERE DocEntry = ?";

						connection.Open();

						foreach (var row in database.ExecuteReader(command, doNumber))
						{
							var item = new Item
							{
								Desc = row.As<string>("desc"),
								Qty = row.As<decimal?>("qty"),
								Sku = row.As<string>("sku")
							};

							items.Add(item);
						}
					}
				}
				return items;
			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.Message);
				throw;
			}
		}

		public void ChangeItemStatus(string itemDo, SyncStatus status, List<Error> errors = null)
		{
			using (var connection = new SqlConnection(connectionString))
			{
				string query;
				if (status != SyncStatus.Error)
					query = @"UPDATE [@DEL_LOG] SET U_DocStatus = @Status WHERE U_DocEntry = @DocEntry";
				else
					query = @"UPDATE [@DEL_LOG] 
								SET 
									U_DocStatus = @Status, 
									U_StatusCode = @StatusCode,
									U_StatusMessage = @StatusMessage
								WHERE 
									U_DocEntry = @DocEntry";

				using (var command = new SqlCommand(query))
				{
					command.Connection = connection;
					command.Parameters.Add("@DocEntry", SqlDbType.Int).Value = itemDo;
					command.Parameters.Add("@Status", SqlDbType.Int).Value = status;

					if (status == SyncStatus.Error && errors != null)
					{
						command.Parameters.Add("@StatusCode", SqlDbType.NVarChar).Value = errors.Count != 0 ? errors[0].Code : "0000";
						command.Parameters.Add("@StatusMessage", SqlDbType.NText).Value = errors.Count != 0 ? errors[0].Message : "No Error message";
					}

					connection.Open();

					command.ExecuteNonQuery();

					connection.Close();
				}
			}
		}

		private ItemType ItemType
		{
			get { return typeof(Delivery).IsAssignableFrom(typeof(T)) ? ItemType.Delivery : ItemType.Collection; }
		}
	}
}
