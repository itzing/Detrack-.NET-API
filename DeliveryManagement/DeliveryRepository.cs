﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing;
using Detrack.Data.Interfaces;
using Detrack.Infrastructure;
using Detrack.Infrastructure.Tools;
using Detrack.Model.Deliveries;
using Detrack.Model;
using Newtonsoft.Json;

namespace Detrack.Data
{
	public class DeliveryRepository : IDeliveryRepository
	{
		private const string ViewAllDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/view/all.json";
		private const string ViewDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/view.json";
		private const string AddDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/create.json";
		private const string EditDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/update.json";
		private const string GetSignatureImageUrl = @"https://app.detrack.com/api/v1/deliveries/signature.json";
		private const string DeleteAllDeliveriesDateUrl = @"https://app.detrack.com/api/v1/deliveries/delete/all.json";
		private const string DeleteDeliveriesUrl = @"https://app.detrack.com/api/v1/deliveries/delete.json";

		private readonly JsonSerializerSettings serializeSettings = new JsonSerializerSettings
		{
			DateFormatString = "yyyy-MM-dd",
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			ContractResolver = new LowerCasePropertyNamesContractResolver(),
		};

		private readonly JsonSerializerSettings deserializeSettings = new JsonSerializerSettings();

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
			catch (Exception)
			{
				return new List<Delivery>();
			}
		}

		public AddResponse Add(List<Delivery> deliveries)
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

					if (deliveryAddResponse != null)
						return deliveryAddResponse;
					
					throw new Exception("Add deliveries failed.");
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

		public EditResponse EditDeliveries(List<Delivery> deliveries)
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

					return deliveryEditResponse;
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

		public DeleteResponse DeleteDeliveries(List<Delivery> deliveries)
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

					var respBytes = client.UploadValues(DeleteDeliveriesUrl, fields);
					var resp = client.Encoding.GetString(respBytes);

					var deliveryDeleteResponse = JsonConvert.DeserializeObject<DeleteResponse>(resp, deserializeSettings);

					return deliveryDeleteResponse;
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

		public DeleteResponse DeleteDeliveriesForDate(DateTime date)
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

					var respBytes = client.UploadValues(DeleteAllDeliveriesDateUrl, fields);
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

		public Delivery GetDelivery(DateTime deliveryDate, string deliveryDo)
		{
			var response =
				GetDeliveries(new List<Delivery> {new Delivery(deliveryDate, deliveryDo)});

			if (response.Info.Status == Status.failed.ToString())
				return null;

			return response.Results.Count() != 0 ? response.Results.Select(d => d.Delivery).FirstOrDefault(d => d.Do == deliveryDo) : null;
		}

		public ViewResponse GetDeliveries(IEnumerable<Delivery> deliveries)
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

	}
}
