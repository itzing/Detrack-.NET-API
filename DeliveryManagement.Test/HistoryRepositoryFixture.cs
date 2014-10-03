using System;
using System.Collections.Generic;
using System.Linq;
using Detrack.Data.SQL;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Detrack.Data.Test
{
	[TestClass]
	public class HistoryRepositoryFixture
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod]
		public void SaveCompletedDelivery()
		{
			var repo = new DetrackRepository<Delivery>("ab1d456a7296733ce90501565eaf3583058b183cb7c6df80");
			var history = new HistoryRepository();

			const string deliveryId1 = "8897978";

			var delivery1 = new Delivery(new DateTime(2014, 8, 29), deliveryId1);

			var response = repo.GetItems(new List<Delivery> {delivery1});

			foreach (var operationResult in response.Results)
			{
				Assert.IsTrue(history.SaveCompletedDelivery(operationResult.Delivery));
			}
		}

		[TestMethod]
		public void ItemExistTrue()
		{
			var history = new HistoryRepository();

			const string deliveryId1 = "8897978";

			Assert.IsTrue(history.DataItemExists(deliveryId1));
		}

		[TestMethod]
		public void ItemExistFalse()
		{
			var history = new HistoryRepository();

			const string deliveryId1 = "88979781";

			Assert.IsFalse(history.DataItemExists(deliveryId1));
		}
	}
}