using System.Collections.Generic;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;

namespace Detrack.Model
{
	public class ListResponse : BaseResponse
	{
		public List<Delivery> Deliveries { get; set; }
		public List<Collection> Collections { get; set; }
	}
}
