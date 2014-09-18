using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using DeliveryManagement.Model;
using DeliveryManagement.Model.Deliveries;

namespace DeliveryManagement.Data.SQL
{
	public class DeliveriesFetcher
	{
		public IEnumerable<Delivery> GetDeliveries(long lastDo)
		{
			var connectionString = ConfigurationManager.ConnectionStrings["Detrack"].ToString();

			var deliveries = new List<Delivery>();
			using (var connection = new SqlConnection(connectionString))
			{
				const string query = @"select DH.DocDate as date, 
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
									Where trnspcode = 1
									AND DocEntry > @lastDo
								ORDER by DocEntry ASC";

				var sqlCommand = new SqlCommand(query, connection);

				sqlCommand.Parameters.AddWithValue("@lastDo", lastDo);
				
				connection.Open();
				
				using (var executeReader = sqlCommand.ExecuteReader())
				{
					while (executeReader.Read())
					{
						var delivery = new Delivery
						{
							Date = DateTime.Parse(executeReader["date"].ToString()),
							Do = executeReader["do"].ToString(),
							Address = executeReader["address"].ToString(),
							Deliver_To = executeReader["deliver_to"].ToString(),
							Phone = executeReader["phone"].ToString(),
							Notify_Email = executeReader["notify_email"].ToString(),
							Instructions = executeReader["instructions"].ToString()
						};

						deliveries.Add(delivery);
					}
					connection.Close();
				}
			}

			return deliveries;
		}
	}
}
