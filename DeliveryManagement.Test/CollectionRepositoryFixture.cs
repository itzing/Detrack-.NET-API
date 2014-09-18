using System;
using System.Collections.Generic;
using System.Linq;
using Detrack.Infrastructure.Tools;
using Detrack.Model;
using Detrack.Model.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Detrack.Data.Test
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

			var result = manager.GetAllForDate(DateTime.Now);

			Assert.AreEqual(result.Count(), 2);
		}

		[TestMethod]
		public void AddCollection()
		{
			var manager = new CollectionRepository();

			var collection = new Collection(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));
			var collection1 = new Collection(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));

			collection.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			collection.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			collection1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			collection1.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			var response = manager.Add(new List<Collection>{collection, collection1});

			Assert.AreEqual(response.Info.Status, Status.ok.ToString()); 
			Assert.AreEqual(response.Info.Failed, 0);

			var listResponse = manager.GetAllForDate(DateTime.Now);

			Assert.AreEqual(listResponse.Count(), 2);
		}

		[TestMethod]
		public void DeleteAllCollections()
		{
			var repo = new CollectionRepository();
			
			AddCollection();

			var response = repo.DeleteCollectionsForDate(DateTime.Now);

			Assert.AreEqual(response.Info.Status, Status.ok.ToString());
			Assert.AreEqual(response.Info.Failed, 0);

			var listResponse = repo.GetAllForDate(DateTime.Now);

			Assert.AreEqual(listResponse.Count(), 0);
		}

		[TestMethod]
		public void EditCollections()
		{
			var collectionId1 = string.Format("DO{0}", DataHelper.GetNumericString(9));
			var address1 = DataHelper.GetAlphanumericString(50);

			var collection1 = new Collection(DateTime.Now, collectionId1, address1);

			var collectionId2 = string.Format("DO{0}", DataHelper.GetNumericString(9));
			var address2 = DataHelper.GetAlphanumericString(50);

			var collection2 = new Collection(DateTime.Now, collectionId2, address2);

			var repo = new CollectionRepository();

			repo.Add(new List<Collection>{collection1, collection2});

			collection1.Address = DataHelper.GetAlphanumericString(50);
			collection2.Address = DataHelper.GetAlphanumericString(50);

			var response = repo.EditCollections(new List<Collection> { collection1, collection2 });

			Assert.AreEqual(response.Info.Status, Status.ok.ToString());
			Assert.AreEqual(response.Info.Failed, 0);

			var results =
				repo.GetCollections(new List<Collection>
				{
					new Collection(DateTime.Now, collectionId1),
					new Collection(DateTime.Now, collectionId2)
				});

			Assert.AreEqual(results.Info.Status, Status.ok.ToString());
			Assert.AreEqual(results.Info.Failed, 0);

			var d1 = results.Results.Select(d => d.Collection).FirstOrDefault(d => d.Do == collectionId1);
			var d2 = results.Results.Select(d => d.Collection).FirstOrDefault(d => d.Do == collectionId2);

			Assert.IsTrue(d1 != null && d1.Address != address1);
			Assert.IsTrue(d2 != null && d1.Address != address2);
		}

		[TestMethod]
		public void GetCollection()
		{
			var repo = new CollectionRepository();
			var collectionId1 = string.Format("DO{0}", DataHelper.GetNumericString(9));
			var address1 = DataHelper.GetAlphanumericString(50);

			var collection1 = new Collection(DateTime.Now, collectionId1, address1);

			AddCollection();
			var response = repo.Add(new List<Collection>{collection1});

			Assert.AreEqual(response.Info.Status, Status.ok.ToString());
			Assert.AreEqual(response.Info.Failed, 0);

			Assert.AreEqual(repo.GetCollection(DateTime.Now, collectionId1).Do, collectionId1);
		}
	}
}
