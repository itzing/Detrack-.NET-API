using System.Collections.Generic;

namespace DeliveryManagement.Model
{
	public class DeliveryListResponse : BaseResponse
	{
		public IEnumerable<Delivery> Deliveries { get; set; }
	}
}
