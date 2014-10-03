using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrack.Infrastructure
{
	public static class StringExtensionMethods
	{
		public static string Replace(this string completeString, string find, string replace, int occurence)
		{
			const int checkOccurence = 1;
			string newString = completeString;
			while (newString.IndexOf(find, StringComparison.Ordinal) != -1)
			{
				if (occurence == checkOccurence)
				{
					return newString.Substring(0, completeString.Length - newString.Length) + newString.Replace(find, replace);
				}
				
				newString = newString.Substring(newString.IndexOf(find, StringComparison.Ordinal));
			}

			return completeString;
		} 

	}
}
