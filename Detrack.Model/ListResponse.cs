using System.Collections.Generic;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;

namespace Detrack.Model
{
	public class ListResponse : BaseResponse
	{
		public IEnumerable<Delivery> Deliveries { get; set; }
		public IEnumerable<Collection> Collections { get; set; }
	}
}
