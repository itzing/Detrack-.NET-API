﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryManagement.Model
{
	public class ViewDeliveriesResponse : BaseResponse
	{
		public IEnumerable<OperationResult> Results { get; set; }
	}
}