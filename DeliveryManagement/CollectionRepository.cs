using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using Detrack.Infrastructure.Tools;
using Detrack.Model.Collections;
using Detrack.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Detrack.Data
{
	public class CollectionRepository
	{
		private const string ViewAllCollectionsUrl = @"https://app.detrack.com/api/v1/collections/view/all.json";
		private const string ViewCollectionsUrl = @"https://app.detrack.com/api/v1/collections/view.json";
		private const string AddCollectionsUrl = @"https://app.detrack.com/api/v1/collections/create.json";
		private const string EditCollectionsUrl = @"https://app.detrack.com/api/v1/collections/update.json";
		private const string GetSignatureImageUrl = @"https://app.detrack.com/api/v1/collections/signature.json";
		private const string DeleteCollectionsUrl = @"https://app.detrack.com/api/v1/collections/delete/all.json";

		private readonly JsonSerializerSettings serializeSettings = new JsonSerializerSettings
		{
			DateFormatString = "yyyy-MM-dd",
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
		};

		private readonly JsonSerializerSettings deserializeSettings = new JsonSerializerSettings
		{
		
		};

		private readonly string Key;

		public CollectionRepository()
		{
			Key = ApiKey.Key;
		}

		public CollectionRepository(string key)
		{
			Key = key;
		}

		public IEnumerable<Collection> GetForDate(DateTime date)
		{
			try
			{
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", string.Format("{{\"date\":\"{0}-{1}-{2}\"}}", date.Year, date.Month, date.Day)}
					};

					var respBytes = client.UploadValues(ViewAllCollectionsUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var response = JsonConvert.DeserializeObject<ListResponse>(resp, deserializeSettings);

					return response.Collections;
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public void Add(List<Collection> collections)
		{
			try
			{
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(collections, serializeSettings)}
					};

					var respBytes = client.UploadValues(AddCollectionsUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var collectionAddResponse = JsonConvert.DeserializeObject<AddResponse>(resp, deserializeSettings);

					if (collectionAddResponse.Info.Failed > 0 || !collectionAddResponse.Info.Status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
						throw new Exception("Add collections failed.");
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public void EditCollections(List<Collection> collections)
		{
			try
			{
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(collections, serializeSettings)}
					};

					var respBytes = client.UploadValues(EditCollectionsUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var collectionEditResponse = JsonConvert.DeserializeObject<EditResponse>(resp, deserializeSettings);

					if (collectionEditResponse.Info.Failed > 0 || !collectionEditResponse.Info.Status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
						throw new Exception("Edit collections failed.");
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public Image GetSignatureImage(Collection collection)
		{
			try
			{
				Image result;
				byte[] respBytes;
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", string.Format("{{\"date\":\"{0}-{1}-{2}\", \"do\":\"{3}\"}}", collection.Date.Year, collection.Date.Month, collection.Date.Day, collection.Do)}
					};

					 respBytes = client.UploadValues(GetSignatureImageUrl, fields);
				}
				
				using (var streamBitmap = new MemoryStream(respBytes))
				{
					using (var image = Image.FromStream(streamBitmap))
					{
						result = new Bitmap(image);
					}
				}

				var imagePath = string.Format("{0}_{1}_Signature.jpg", collection.Date.ToString("yyyy-MM-dd"), collection.Do);

				ImageHelper.SaveImage(imagePath, result);

				return result;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public void DeleteCollectionsForDate(DateTime date)
		{
			try
			{
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
				{
					{"key", Key},
					{"json", string.Format("{{\"date\":\"{0}-{1}-{2}\"}}", date.Year, date.Month, date.Day)}
				};

					var respBytes = client.UploadValues(DeleteCollectionsUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var response = JsonConvert.DeserializeObject<DeleteResponse<Collection>>(resp, deserializeSettings);

					if (response.Info.Failed > 0 || !response.Info.Status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
						throw new Exception("Add collections failed.");
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public Collection GetCollection(DateTime collectionDate, string collectionDo)
		{
			return GetCollections(new List<Collection> {new Collection(collectionDate, collectionDo)}).FirstOrDefault(d => d.Do == collectionDo);
		}

		public IEnumerable<Collection> GetCollections(IEnumerable<Collection> collections)
		{
			try
			{
				using (var client = new WebClient())
				{

					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(collections, serializeSettings)}
					};

					var respBytes = client.UploadValues(ViewCollectionsUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var response = JsonConvert.DeserializeObject<ViewResponse>(resp, deserializeSettings);

					return response.Results.Select(d => d.Collection);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

	}
}
