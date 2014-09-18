using System;
using System.Collections.Generic;

namespace DeliveryManagement.Model.Deliveries
{
	public class Delivery : BaseDataObject
	{
		public Delivery(DateTime date, string id, string address) : base(date, id, address)
		{
			Date = date;
			Do = id;
			Address = address;
			Items = new List<Item>();
		}

		public Delivery(DateTime date, string id) : base (date, id)
		{
			Date = date;
			Do = id;
			Items = new List<Item>();
		}

		public Delivery() : base()
		{
			
		}

		public string Delivery_Time { get; set; }
		public string Deliver_To { get; set; }
		public string Received_By { get; set; }
	}
}
