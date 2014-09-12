using System.Collections.Generic;	

namespace DeliveryManagement.Model
{
	public class DeliveryAddResponse : BaseResponse
	{
		public IEnumerable<OperationResult> Results { get; set; }
	}
}
