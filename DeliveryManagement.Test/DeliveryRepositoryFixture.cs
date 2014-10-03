using System;
using System.Collections.Generic;
using System.Linq;
using Detrack.Infrastructure.Tools;
using Detrack.Model;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Detrack.Data.Test
{
	[TestClass]
	public class DeliveryRepositoryFixture
	{
		[TestInitialize]
		public void Setup()
		{
			var repository = new DetrackRepository<Delivery>();

			repository.DeleteForDate(DateTime.Now);
		}

		[TestMethod]
		public void GetDeliveries()
		{
			AddDelivery();

			var manager = new DetrackRepository<Delivery>();

			var result = manager.GetAllForDate(DateTime.Now);

			Assert.AreEqual(result.Count(), 2);
		}

		private Delivery GetRandomDelivery()
		{
			var delivery = new Delivery(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));

			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			return delivery;
		}

		private IEnumerable<Delivery> GetRandomDeliveries(int n)
		{
			for (var i = 0; i < n; i++)
			{
				yield return GetRandomDelivery();
			}
		}

		[TestMethod]
		public void TestTake()
		{
			var data = GetRandomDeliveries(201).GetEnumerator();

			var firstHundred = new List<Delivery>(100);

			while (data.MoveNext())
			{
				firstHundred.Add(data.Current);
				if (firstHundred.Count == 100)
					break;
			}

			var secondHundred = new List<Delivery>(100);

			while (data.MoveNext())
			{
				secondHundred.Add(data.Current);
				if (secondHundred.Count == 100)
					break;
			}

			data.MoveNext();
			var last = data.Current;

			Assert.IsFalse(data.MoveNext());
		}


		[TestMethod]
		public void AddDelivery()
		{
			var manager = new DetrackRepository<Delivery>();

			var delivery = new Delivery(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));
			var delivery1 = new Delivery(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));

			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			delivery1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			var response = manager.Add(new List<Delivery>{delivery, delivery1});

			Assert.AreEqual(response.Info.Status, Status.ok.ToString()); 
			Assert.AreEqual(response.Info.Failed, 0);

			var listResponse = manager.GetAllForDate(DateTime.Now);

			Assert.AreEqual(listResponse.Count(), 2);
		}

		[TestMethod]
		public void DeleteAllDeliveries()
		{
			var repo = new DetrackRepository<Delivery>();
			
			AddDelivery();

			var response = repo.DeleteForDate(DateTime.Now);

			Assert.AreEqual(response.Info.Status, Status.ok.ToString());
			Assert.AreEqual(response.Info.Failed, 0);

			var listResponse = repo.GetAllForDate(DateTime.Now);

			Assert.AreEqual(listResponse.Count(), 0);
		}

		[TestMethod]
		public void DeleteDeliveries()
		{
			var repo = new DetrackRepository<Delivery>();

			var delivery = new Delivery(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));
			var delivery1 = new Delivery(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));

			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			delivery1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			var response = repo.Add(new List<Delivery> { delivery, delivery1 });

			Assert.AreEqual(response.Info.Status, Status.ok.ToString());
			Assert.AreEqual(response.Info.Failed, 0);

			var deleteResponse = repo.DeleteItems(new List<Delivery> { delivery, delivery1 });

			Assert.AreEqual(deleteResponse.Info.Status, Status.ok.ToString());

			Assert.AreEqual(deleteResponse.Results.Count(), 2);

			var listResponse = repo.GetAllForDate(DateTime.Now);

			Assert.AreEqual(listResponse.Count(), 0);
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

			var repo = new DetrackRepository<Delivery>();

			repo.Add(new List<Delivery>{delivery1, delivery2});

			delivery1.Address = DataHelper.GetAlphanumericString(50);
			delivery2.Address = DataHelper.GetAlphanumericString(50);

			var response = repo.EditItems(new List<Delivery> { delivery1, delivery2 });

			Assert.AreEqual(response.Info.Status, Status.ok.ToString());
			Assert.AreEqual(response.Info.Failed, 0);

			var results =
				repo.GetItems(new List<Delivery>
				{
					new Delivery(DateTime.Now, deliveryId1),
					new Delivery(DateTime.Now, deliveryId2)
				});

			Assert.AreEqual(results.Info.Status, Status.ok.ToString());
			Assert.AreEqual(results.Info.Failed, 0);

			var d1 = results.Results.Select(d => d.Delivery).FirstOrDefault(d => d.Do == deliveryId1);
			var d2 = results.Results.Select(d => d.Delivery).FirstOrDefault(d => d.Do == deliveryId2);

			Assert.IsTrue(d1 != null && d1.Address != address1);
			Assert.IsTrue(d2 != null && d1.Address != address2);
		}

		[TestMethod]
		public void GetSignatureImageDelivery()
		{
			var repo = new DetrackRepository<Delivery>("ab1d456a7296733ce90501565eaf3583058b183cb7c6df80");
			const string deliveryId = "8897978";

			var delivery = new Delivery(new DateTime(2014, 8, 29), deliveryId);

			Assert.IsNotNull(repo.GetImage(delivery, ImageType.POD_PHOTO_6));
		}
		
		[TestMethod]
		public void GetSignatureImageCollection()
		{
			var repo = new DetrackRepository<Collection>("ab1d456a7296733ce90501565eaf3583058b183cb7c6df80");
			const string collectionId = "8897978";

			var collection = new Collection(new DateTime(2014, 8, 29), collectionId);

			Assert.IsNotNull(repo.GetSignatureImage(collection));
		}
	}
}
