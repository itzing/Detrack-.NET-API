using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryManagement.Data.SQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeliveryManagement.Test
{
	[TestClass]
	public class DeliveryFetcherFixture
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod]
		public void GetDeliveries()
		{
			var fetcher = new DeliveriesFetcher();
			Assert.IsTrue(fetcher.GetDeliveries(0).Count() != 0);
		}
	}
}
