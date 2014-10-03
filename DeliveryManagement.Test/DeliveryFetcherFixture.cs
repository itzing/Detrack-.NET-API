using System.Linq;
using Detrack.Data.SQL;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;
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
			var repository = new DeliveryLogRepository<Delivery>();
			Assert.IsTrue(repository.GetDeliveries(SyncStatus.New).Count() != 0);
		}

		[TestMethod]
		public void GetCollections()
		{
			var repository = new DeliveryLogRepository<Collection>();
			Assert.IsTrue(repository.GetCollections(SyncStatus.New).Count() != 0);
		}
	}
}
