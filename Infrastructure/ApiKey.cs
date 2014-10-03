using System.Configuration;

namespace Detrack.Infrastructure
{
	public static class ApiKey
	{
		public static string Key = ConfigurationManager.AppSettings["ApiKey"];
	}
}
