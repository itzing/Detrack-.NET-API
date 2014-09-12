using System;
using System.Collections.Specialized;
using System.Net;
using DeliveryManagement;
using DeliveryManagement.Data;
using DeliveryManagement.Model;
using Newtonsoft.Json;


namespace JsonServiceRequest
{
	class Program
	{
		static void Main(string[] args)
		{
			var manager = new DeliveryRepository();

			var result = manager.GetForDate(new DateTime(2014, 8, 29));

			//const string url = @"https://app.detrack.com/api/v1/deliveries/view/all.json";

			//using (var client = new WebClient())
			//{
			//	var fields = new NameValueCollection
			//	{
			//		{"key", ApiKey.Key},
			//		{"json", "{\"date\":\"2014-08-29\"}"}
			//	};

			//	var respBytes = client.UploadValues(url, fields);
			//	var resp = client.Encoding.GetString(respBytes);

			//	var myData = JsonConvert.DeserializeObject<DeliveryListResponse>(resp);

			//	Console.ReadLine();
			//}
		}
	}
}
