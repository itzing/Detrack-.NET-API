using System;
using System.Collections.Generic;

namespace DeliveryManagement.Model.Collections
{
	public class Collection : BaseDataObject
	{
		public Collection(DateTime date, string id, string address)
		{
			Date = date;
			Do = id;
			Address = address;
			Items = new List<Item>();
		}

		public Collection(DateTime date, string id)
		{
			Date = date;
			Do = id;
			Items = new List<Item>();
		}

		public Collection()
		{
			
		}

		public string Collection_Time { get; set; }
		public string Collect_From { get; set; }
		public string Sent_By { get; set; }
	}
}
