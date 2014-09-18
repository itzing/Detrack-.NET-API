using System.Collections.Generic;

namespace DeliveryManagement.Model
{
	public class BaseResponse
	{
		public Info Info { get; set; }
		public IEnumerable<OperationResult> Results { get; set; }
	}
}
