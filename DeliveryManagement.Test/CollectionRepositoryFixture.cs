using System;
using System.Collections.Generic;
using System.Linq;
using DeliveryManagement.Data;
using DeliveryManagement.Model;
using DeliveryManagement.Model.Collections;
using DeliveryManagement.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DeliveryManagement.Test
{
	[TestClass]
	public class CollectionRepositoryFixture
	{
		[TestInitialize]
		public void Setup()
		{
			var repository = new CollectionRepository();

			repository.DeleteCollectionsForDate(DateTime.Now);
		}
		
		[TestMethod]
		public void GetCollections()
		{
			AddCollection();

			var manager = new CollectionRepository();

			var result = manager.GetForDate(DateTime.Now);

			Assert.AreEqual(result.Count(), 2);
		}

		[TestMethod]
		public void AddCollection()
		{
			var manager = new CollectionRepository();

			var delivery = new Collection(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));
			var delivery1 = new Collection(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));

			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			delivery1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			manager.Add(new List<Collection>{delivery, delivery1});

			Assert.AreEqual(manager.GetForDate(DateTime.Now).Count(), 2);
		}

		[TestMethod]
		public void DeleteAllCollections()
		{
			var repo = new CollectionRepository();
			
			AddCollection();

			repo.DeleteCollectionsForDate(DateTime.Now);

			Assert.AreEqual(repo.GetForDate(DateTime.Now).Count(), 0);
		}

		[TestMethod]
		public void EditCollections()
		{
			var deliveryId1 = string.Format("DO{0}", DataHelper.GetNumericString(9));
			var address1 = DataHelper.GetAlphanumericString(50);

			var delivery1 = new Collection(DateTime.Now, deliveryId1, address1);

			var deliveryId2 = string.Format("DO{0}", DataHelper.GetNumericString(9));
			var address2 = DataHelper.GetAlphanumericString(50);

			var delivery2 = new Collection(DateTime.Now, deliveryId2, address2);

			var repo = new CollectionRepository();

			repo.Add(new List<Collection>{delivery1, delivery2});

			delivery1.Address = DataHelper.GetAlphanumericString(50);
			delivery2.Address = DataHelper.GetAlphanumericString(50);

			repo.EditCollections(new List<Collection> { delivery1, delivery2 });

			var results =
				repo.GetCollections(new List<Collection>
				{
					new Collection(DateTime.Now, deliveryId1),
					new Collection(DateTime.Now, deliveryId2)
				}).ToList();

			var d1 = results.FirstOrDefault(d => d.Do == deliveryId1);
			var d2 = results.FirstOrDefault(d => d.Do == deliveryId2);

			Assert.IsTrue(d1 != null && d1.Address != address1);
			Assert.IsTrue(d2 != null && d1.Address != address2);
		}

		[TestMethod]
		public void GetCollection()
		{
			var repo = new CollectionRepository();
			var deliveryId1 = string.Format("DO{0}", DataHelper.GetNumericString(9));
			var address1 = DataHelper.GetAlphanumericString(50);

			var delivery1 = new Collection(DateTime.Now, deliveryId1, address1);

			AddCollection();
			repo.Add(new List<Collection>{delivery1});

			Assert.AreEqual(repo.GetCollection(DateTime.Now, deliveryId1).Do, deliveryId1);
		}

		[TestMethod]
		public void GetSignatureImage()
		{
			var repo = new CollectionRepository("c9fa6fc31cf7ef373a9925f330ad52ea59244cabda6ef46e");
			var deliveryId1 = "8897978";

			var delivery1 = new Collection(new DateTime(2014, 8, 29), deliveryId1);

			Assert.IsNotNull(repo.GetSignatureImage(delivery1));
		}
	}
}
