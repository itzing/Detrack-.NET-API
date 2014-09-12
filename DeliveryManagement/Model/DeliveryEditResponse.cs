using System.Collections.Generic;

namespace DeliveryManagement.Model
{
	public class DeliveryEditResponse : BaseResponse
	{
		public IEnumerable<OperationResult> Results { get; set; }
	}
}
