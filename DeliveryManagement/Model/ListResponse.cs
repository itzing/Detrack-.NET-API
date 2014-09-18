using System.Collections.Generic;
using DeliveryManagement.Model.Collections;
using DeliveryManagement.Model.Deliveries;

namespace DeliveryManagement.Model
{
	public class ListResponse : BaseResponse
	{
		public IEnumerable<Delivery> Deliveries { get; set; }
		public IEnumerable<Collection> Collections { get; set; }
	}
}
