using System;
using System.Collections.Generic;
using System.Linq;
using Detrack.Model;
using Detrack.Model.Deliveries;

namespace Detrack.Infrastructure.Tools
{
	public class DataHelper
	{
		const string AlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		const string NumericChars = "0123456789";
		static readonly Random random = new Random();
		private static readonly object SyncLock = new object();

		public static string GetAlphanumericString(int length)
		{
			return new string(Enumerable.Repeat(AlphanumericChars, length)
						  .Select(s => s[random.Next(s.Length)])
						  .ToArray()).ToLower();
		}

		public static string GetNumericString(int length)
		{
			return new string(Enumerable.Repeat(NumericChars, length)
						  .Select(s => s[random.Next(s.Length)])
						  .ToArray());
		}

		public static int GetNumber(int minValue = 0, int maxValue = 8)
		{
			return random.Next(minValue, maxValue);
		}

		public static bool GetBool()
		{
			return GetNumber(-3, 3) > 0;
		}

		public static Delivery GetRandomDelivery()
		{
			var delivery = new Delivery(DateTime.Now, string.Format("DO{0}", DataHelper.GetNumericString(9)), string.Format("{0} Ubi Avenue {1} Singapore {2}", DataHelper.GetNumber(1, 99), DataHelper.GetNumber(), DataHelper.GetNumber(0, 999)));

			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));
			delivery.Items.Add(new Item(DataHelper.GetAlphanumericString(5), DataHelper.GetAlphanumericString(50), DataHelper.GetNumber(1, 50)));

			return delivery;
		}

		public static IEnumerable<Delivery> GetRandomDeliveries(int n)
		{
			for (var i = 0; i < n; i++)
			{
				yield return GetRandomDelivery();
			}
		}

	}
}