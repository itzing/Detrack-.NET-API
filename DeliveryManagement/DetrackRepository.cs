using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using Detrack.Infrastructure;
using Detrack.Infrastructure.Logging;
using Detrack.Infrastructure.Tools;
using Detrack.Model;
using Detrack.Model.Collections;
using Detrack.Model.Deliveries;
using Newtonsoft.Json;

namespace Detrack.Data
{
	public class DetrackRepository<T> where T: BaseDataObject
	{
		private static readonly ILog log = LogManager.GetLog("DetrackRepository");

		private string ViewAllUrl;
		private string ViewUrl;
		private string AddUrl;
		private string EditUrl;
		private string GetSignatureImageUrl;
		private string GetPodImageUrl;
		private string GetPodPhoto1ImageUrl;
		private string GetPodPhoto2ImageUrl;
		private string GetPodPhoto3ImageUrl;
		private string GetPodPhoto4ImageUrl;
		private string DeleteAllDateUrl;
		private string DeleteUrl;

		private readonly JsonSerializerSettings serializeSettings = new JsonSerializerSettings
		{
			DateFormatString = "yyyy-MM-dd",
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			ContractResolver = new LowerCasePropertyNamesContractResolver(),
		};

		private readonly JsonSerializerSettings deserializeSettings = new JsonSerializerSettings();

		private readonly string Key;

		public DetrackRepository()
		{
			Key = ApiKey.Key;
			SetUrls();
		}

		public DetrackRepository(string key)
		{
			Key = key;
			SetUrls();
		}

		public ItemType ItemType
		{
			get { return typeof (Delivery).IsAssignableFrom(typeof (T)) ? ItemType.Delivery : ItemType.Collection; }
		}

		public IEnumerable<T> GetAllForDate(DateTime date)
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

					var respBytes = client.UploadValues(ViewAllUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var response = JsonConvert.DeserializeObject<ListResponse>(resp, deserializeSettings);


					return ItemType == ItemType.Delivery ? response.Deliveries.Cast<T>() : response.Collections.Cast<T>();
				}
			}
			catch (Exception)
			{
				return ItemType == ItemType.Delivery ? new List<Delivery>().Cast<T>() : new List<Collection>().Cast<T>();
			}
		}

		public AddResponse Add(List<T> items)
		{
			try
			{
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(items, serializeSettings)}
					};

					var respBytes = client.UploadValues(AddUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var addResponse = JsonConvert.DeserializeObject<AddResponse>(resp, deserializeSettings);

					if (addResponse != null)
						return addResponse;
					
					throw new Exception("Add failed.");
				}
			}
			catch (Exception ex)
			{
				return new AddResponse
				{
					Info = new Info
					{
						Status = Status.failed.ToString(),
						Error = new Error
						{
							Message = ex.Message
						}
					}
				};
			}
		}

		public EditResponse EditItems(List<T> items)
		{
			try
			{
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(items, serializeSettings)}
					};

					var respBytes = client.UploadValues(EditUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var editResponse = JsonConvert.DeserializeObject<EditResponse>(resp, deserializeSettings);

					return editResponse;
				}
			}
			catch (Exception ex)
			{
				return new EditResponse
				{
					Info = new Info
					{
						Status = Status.failed.ToString(),
						Error = new Error
						{
							Message = ex.Message
						}
					}
				};
			}
		}

		public DeleteResponse DeleteItems(List<T> items)
		{
			try
			{
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(items, serializeSettings)}
					};

					var respBytes = client.UploadValues(DeleteUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var deleteResponse = JsonConvert.DeserializeObject<DeleteResponse>(resp, deserializeSettings);

					return deleteResponse;
				}
			}
			catch (Exception ex)
			{
				return new DeleteResponse
				{
					Info = new Info
					{
						Status = Status.failed.ToString(),
						Error = new Error
						{
							Message = ex.Message
						}
					}
				};
			}
		}

		public Image GetSignatureImage(T item)
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
						{"json", string.Format("{{\"date\":\"{0}-{1}-{2}\", \"do\":\"{3}\"}}", item.Date.Year, item.Date.Month, item.Date.Day, item.Do)}
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

				var imagePath = string.Format("{0}_{1}_Signature.jpg", item.Date.ToString("yyyy-MM-dd"), item.Do);

				ImageHelper.SaveImage(imagePath, result);

				return result;
			}
			catch (Exception ex)
			{
				log.Error("Image get failed for DO#: {0} - {1}", item.Do, ex.Message);
				return null;
			}
		}

		private string GetImageUrl(ImageType imageType)
		{
			switch (imageType)
			{
				case ImageType.POD_PHOTO_1:
					return GetPodPhoto1ImageUrl;
				case ImageType.POD_PHOTO_2:
					return GetPodPhoto2ImageUrl;
				case ImageType.POD_PHOTO_3:
					return GetPodPhoto3ImageUrl;
				case ImageType.POD_PHOTO_4:
					return GetPodPhoto4ImageUrl;
				case ImageType.POD_PHOTO_5:
					return GetPodImageUrl;
				default:
					return GetSignatureImageUrl;
			}
		}

		public Image GetImage(object obj, ImageType imageType)
		{
			try
			{
				Image result;
				string imageUrl = GetImageUrl(imageType);
				byte[] respBytes;

				var item = obj as T;

				if (item == null)
					return null;

				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", string.Format("{{\"date\":\"{0}-{1}-{2}\", \"do\":\"{3}\"}}", item.Date.Year, item.Date.Month, item.Date.Day, item.Do)}
					};

					respBytes = client.UploadValues(imageUrl, fields);
				}

				using (var streamBitmap = new MemoryStream(respBytes))
				{
					using (var image = Image.FromStream(streamBitmap))
					{
						result = new Bitmap(image);
					}
				}

				var imagePath = string.Format("{0}_{1}.jpg", item.Do, imageType);

				ImageHelper.SaveImage(imagePath, result);

				return result;
			}
			catch (Exception ex)
			{
				var item = obj as T;
				if (item != null) log.Error("Image get failed for DO#: {0} - {1}", item.Do, ex.Message);
				return null;
			}
		}

		public DeleteResponse DeleteForDate(DateTime date)
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

					var respBytes = client.UploadValues(DeleteAllDateUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var response = JsonConvert.DeserializeObject<DeleteResponse>(resp, deserializeSettings);

					return response;
				}
			}
			catch (Exception ex)
			{
				return new DeleteResponse()
				{
					Info = new Info
					{
						Status = Status.failed.ToString(),
						Error = new Error()
						{
							Message = ex.Message
						}
					}
				};
			}
		}

		public ViewResponse GetItems(IEnumerable<T> items)
		{
			try
			{
				if (!items.Any())
					return new ViewResponse
					{
						Info = new Info
						{
							Status = Status.ok.ToString()
						},
						Results = new List<OperationResult>()
					};

				using (var client = new WebClient())
				{

					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(items, serializeSettings)}
					};

					var respBytes = client.UploadValues(ViewUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var response = JsonConvert.DeserializeObject<ViewResponse>(resp, deserializeSettings);

					return response;
				}
			}
			catch (Exception ex)
			{
				return new ViewResponse
				{
					Info = new Info 
					{
						Status = Status.failed.ToString(), 
						Error = new Error()
						{
							Message = ex.Message
						}
					}
				};
			}
		}

		private void SetUrls()
		{
			if (ItemType == ItemType.Delivery)
			{
				ViewAllUrl = @"https://app.detrack.com/api/v1/deliveries/view/all.json";
				ViewUrl = @"https://app.detrack.com/api/v1/deliveries/view.json";
				AddUrl = @"https://app.detrack.com/api/v1/deliveries/create.json";
				EditUrl = @"https://app.detrack.com/api/v1/deliveries/update.json";
				GetSignatureImageUrl = @"https://app.detrack.com/api/v1/deliveries/signature.json";
				GetPodImageUrl = @"https://app.detrack.com/api/v1/deliveries/pod.json";
				GetPodPhoto1ImageUrl = @"https://app.detrack.com/api/v1/deliveries/photo_1.json";
				GetPodPhoto2ImageUrl = @"https://app.detrack.com/api/v1/deliveries/photo_2.json";
				GetPodPhoto3ImageUrl = @"https://app.detrack.com/api/v1/deliveries/photo_3.json";
				GetPodPhoto4ImageUrl = @"https://app.detrack.com/api/v1/deliveries/photo_4.json";
				DeleteAllDateUrl = @"https://app.detrack.com/api/v1/deliveries/delete/all.json";
				DeleteUrl = @"https://app.detrack.com/api/v1/deliveries/delete.json";
			}
			else
			{
				ViewAllUrl = @"https://app.detrack.com/api/v1/collections/view/all.json";
				ViewUrl = @"https://app.detrack.com/api/v1/collections/view.json";
				AddUrl = @"https://app.detrack.com/api/v1/collections/create.json";
				EditUrl = @"https://app.detrack.com/api/v1/collections/update.json";
				GetSignatureImageUrl = @"https://app.detrack.com/api/v1/collections/signature.json";
				GetPodImageUrl = @"https://app.detrack.com/api/v1/collections/pod.json";
				GetPodPhoto1ImageUrl = @"https://app.detrack.com/api/v1/collections/photo_1.json";
				GetPodPhoto2ImageUrl = @"https://app.detrack.com/api/v1/collections/photo_2.json";
				GetPodPhoto3ImageUrl = @"https://app.detrack.com/api/v1/collections/photo_3.json";
				GetPodPhoto4ImageUrl = @"https://app.detrack.com/api/v1/collections/photo_4.json";
				DeleteAllDateUrl = @"https://app.detrack.com/api/v1/collections/delete/all.json";
				DeleteUrl = @"https://app.detrack.com/api/v1/collections/delete.json";
			}
		}
	}
}
