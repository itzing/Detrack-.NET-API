using System;
using System.Linq;

namespace DeliveryManagement.Tools
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
	}
}