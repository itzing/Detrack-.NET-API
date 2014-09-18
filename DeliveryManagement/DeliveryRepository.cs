using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing;
using Detrack.Data.Interfaces;
using Detrack.Infrastructure.Tools;
using Detrack.Model.Deliveries;
using Detrack.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Detrack.Data
{
	public class DeliveryRepository : IDeliveryRepository
	{
		private const string ViewAllDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/view/all.json";
		private const string ViewDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/view.json";
		private const string AddDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/create.json";
		private const string EditDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/update.json";
		private const string GetSignatureImageUrl = @"https://app.detrack.com/api/v1/deliveries/signature.json";
		private const string DeleteDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/delete/all.json";

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

		public DeliveryRepository()
		{
			Key = ApiKey.Key;
		}

		public DeliveryRepository(string key)
		{
			Key = key;
		}

		public IEnumerable<Delivery> GetAllForDate(DateTime date)
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

					var respBytes = client.UploadValues(ViewAllDeliveriesUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var response = JsonConvert.DeserializeObject<ListResponse>(resp, deserializeSettings);

					return response.Deliveries;
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public void Add(List<Delivery> deliveries)
		{
			try
			{
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(deliveries, serializeSettings)}
					};

					var respBytes = client.UploadValues(AddDeliveriesUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var deliveryAddResponse = JsonConvert.DeserializeObject<AddResponse>(resp, deserializeSettings);

					if (deliveryAddResponse.Info.Failed > 0 || !deliveryAddResponse.Info.Status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
						throw new Exception("Add deliveries failed.");
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public void EditDeliveries(List<Delivery> deliveries)
		{
			try
			{
				using (var client = new WebClient())
				{
					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(deliveries, serializeSettings)}
					};

					var respBytes = client.UploadValues(EditDeliveriesUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var deliveryEditResponse = JsonConvert.DeserializeObject<EditResponse>(resp, deserializeSettings);

					if (deliveryEditResponse.Info.Failed > 0 || !deliveryEditResponse.Info.Status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
						throw new Exception("Edit deliveries failed.");
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public Image GetSignatureImage(Delivery delivery)
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
						{"json", string.Format("{{\"date\":\"{0}-{1}-{2}\", \"do\":\"{3}\"}}", delivery.Date.Year, delivery.Date.Month, delivery.Date.Day, delivery.Do)}
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

				var imagePath = string.Format("{0}_{1}_Signature.jpg", delivery.Date.ToString("yyyy-MM-dd"), delivery.Do);

				ImageHelper.SaveImage(imagePath, result);

				return result;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public void DeleteDeliveriesForDate(DateTime date)
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

					var respBytes = client.UploadValues(DeleteDeliveriesUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var response = JsonConvert.DeserializeObject<DeleteResponse<Delivery>>(resp, deserializeSettings);

					if (response.Info.Failed > 0 || !response.Info.Status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
						throw new Exception("Add deliveries failed.");
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public Delivery GetDelivery(DateTime deliveryDate, string deliveryDo)
		{
			return GetDeliveries(new List<Delivery> {new Delivery(deliveryDate, deliveryDo)}).FirstOrDefault(d => d.Do == deliveryDo);
		}

		public IEnumerable<Delivery> GetDeliveries(IEnumerable<Delivery> deliveries)
		{
			try
			{
				using (var client = new WebClient())
				{

					var fields = new NameValueCollection
					{
						{"key", Key},
						{"json", JsonConvert.SerializeObject(deliveries, serializeSettings)}
					};

					var respBytes = client.UploadValues(ViewDeliveriesUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var response = JsonConvert.DeserializeObject<ViewResponse>(resp, deserializeSettings);

					return response.Results.Select(d => d.Delivery);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

	}
}
