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
			var fetcher = new SqlDatabase();
			Assert.IsTrue(fetcher.GetDeliveries().Count() != 0);
		}

		[TestMethod]
		public void GetCollections()
		{
			var fetcher = new SqlDatabase();
			Assert.IsTrue(fetcher.GetCollections().Count() != 0);
		}
	}
}
