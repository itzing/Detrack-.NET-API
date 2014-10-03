using System;
using System.Collections.Generic;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;

namespace Detrack.Model
{
	public class OperationResult
	{
		public DateTime Date { get; set; }
		public string Do { get; set; }
		public string Status { get; set; }
		public List<Error> Errors { get; set; }
		public Delivery Delivery { get; set; }
		public Collection Collection { get; set; }

		public bool Completed
		{
			get { return !GetStatusText().Equals("In Progress", StringComparison.OrdinalIgnoreCase); }
		}

		private string GetStatusText()
		{
			return Delivery != null ? Delivery.Status : Collection.Status;
		}
	}
}
