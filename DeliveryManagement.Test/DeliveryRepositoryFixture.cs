using System;
using System.Collections.Generic;
using System.Linq;
using DeliveryManagement.Data;
using DeliveryManagement.Model;
using DeliveryManagement.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeliveryManagement.Test
{
	[TestClass]
	public class DeliveryRepositoryFixture
	{
		[TestInitialize]
		public void Setup()
		{
			var repository = new DeliveryRepository();

			repository.DeleteDeliveriesForDate(DateTime.Now);
		}

		[TestMethod]
		public void GetDeliveries()
		{
			AddDelivery();

			var manager = new DeliveryRepository();

			var result = manager.GetForDate(DateTime.Now);

			Assert.AreEqual(result.Count(), 2);
		}

		[TestMethod]
		public void AddDelivery()
		{
			var manager = new DeliveryRepository();

			var delivery = new Delivery(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));
			var delivery1 = new Delivery(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));

			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			delivery1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			manager.Add(new List<Delivery>{delivery, delivery1});

			Assert.AreEqual(manager.GetForDate(DateTime.Now).Count(), 2);
		}

		[TestMethod]
		public void DeleteAllDeliveries()
		{
			var repo = new DeliveryRepository();
			
			AddDelivery();

			repo.DeleteDeliveriesForDate(DateTime.Now);

			Assert.AreEqual(repo.GetForDate(DateTime.Now).Count(), 0);
		}

		[TestMethod]
		public void EditDeliveries()
		{
			var deliveryId1 = string.Format("DO{0}", DataHelper.GetNumericString(9));
			var address1 = DataHelper.GetAlphanumericString(50);

			var delivery1 = new Delivery(DateTime.Now, deliveryId1, address1);

			var deliveryId2 = string.Format("DO{0}", DataHelper.GetNumericString(9));
			var address2 = DataHelper.GetAlphanumericString(50);

			var delivery2 = new Delivery(DateTime.Now, deliveryId2, address2);

			var repo = new DeliveryRepository();

			repo.Add(new List<Delivery>{delivery1, delivery2});

			delivery1.Address = DataHelper.GetAlphanumericString(50);
			delivery2.Address = DataHelper.GetAlphanumericString(50);

			repo.EditDeliveries(new List<Delivery> { delivery1, delivery2 });

			var results =
				repo.GetDeliveries(new List<Delivery>
				{
					new Delivery(DateTime.Now, deliveryId1),
					new Delivery(DateTime.Now, deliveryId2)
				}).ToList();

			var d1 = results.FirstOrDefault(d => d.Do == deliveryId1);
			var d2 = results.FirstOrDefault(d => d.Do == deliveryId2);

			Assert.IsTrue(d1 != null && d1.Address != address1);
			Assert.IsTrue(d2 != null && d1.Address != address2);
		}

		[TestMethod]
		public void GetDelivery()
		{
			var repo = new DeliveryRepository();
			var deliveryId1 = string.Format("DO{0}", DataHelper.GetNumericString(9));
			var address1 = DataHelper.GetAlphanumericString(50);

			var delivery1 = new Delivery(DateTime.Now, deliveryId1, address1);

			AddDelivery();
			repo.Add(new List<Delivery>{delivery1});

			Assert.AreEqual(repo.GetDelivery(DateTime.Now, deliveryId1).Do, deliveryId1);
		}
	}
}
