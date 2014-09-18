using System;
using System.Collections.Generic;

namespace Detrack.Model
{
	public class BaseDataObject
	{
		public BaseDataObject(DateTime date, string id, string address)
		{
			Date = date;
			Do = id;
			Address = address;
			Items = new List<Item>();
		}

		public BaseDataObject(DateTime date, string id)
		{
			Date = date;
			Do = id;
			Items = new List<Item>();
		}

		public BaseDataObject()
		{
			
		}

		public DateTime Date { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public string Notify_Email { get; set; }
		public string Notify_Url { get; set; }
		public string Assign_To { get; set; }
		public string Instructions { get; set; }
		public string Zone { get; set; }
		public string Reason { get; set; }
		public string Note { get; set; }
		public int Image { get; set; }
		public string View_Image_Url { get; set; }
		public string View_Signature_Url { get; set; }
		public string View_Photo_1_Url { get; set; }
		public string View_Photo_2_Url { get; set; }
		public string View_Photo_3_Url { get; set; }
		public string View_Photo_4_Url { get; set; }
		public string Do { get; set; }
		public string Status { get; set; }
		public DateTime? Time { get; set; }
		public float? Pod_Lat { get; set; }
		public float? Pod_Lng { get; set; }
		public string Pod_Address { get; set; }
		public string Job_Order { get; set; }

		public List<Item> Items { get; set; }

	}
}
