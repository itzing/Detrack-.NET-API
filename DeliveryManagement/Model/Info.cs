using System;
using System.Collections.Generic;

namespace DeliveryManagement.Model
{
	public class Info
	{
		public string Status { get; set; }
		public int Failed { get; set; }
		public int Deleted { get; set; }
		public Error Error { get; set; }
	}
}
