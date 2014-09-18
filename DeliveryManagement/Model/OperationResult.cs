using System;
using System.Collections.Generic;
using DeliveryManagement.Model.Collections;
using DeliveryManagement.Model.Deliveries;

namespace DeliveryManagement.Model
{
	public class OperationResult
	{
		public DateTime Date { get; set; }
		public string Do { get; set; }
		public string Status { get; set; }
		public List<Error> Errors { get; set; }
		public Delivery Delivery { get; set; }
		public Collection Collection { get; set; }
	}
}
