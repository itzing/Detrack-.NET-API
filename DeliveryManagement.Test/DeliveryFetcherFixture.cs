using System.Linq;
using Detrack.Data.SQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Detrack.Data.Test
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
