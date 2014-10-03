using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Detrack.Infrastructure.Logging;

namespace Detrack.Infrastructure
{
	public static class ApplicationSettings
	{
		private static string applicationFolder;
		public static string ApplicationFolder
		{
			get
			{
				return applicationFolder ??
					   (applicationFolder =
						   (new FileInfo(Assembly.GetAssembly(typeof(ApplicationSettings)).Location)).DirectoryName);
			}
		}

		public static string LogsFolder
		{
			get { return Path.Combine(ApplicationFolder, "Logs"); }
		}

		private static LogLevel logLevelE = LogLevel.Error;
		private static bool logLevelLoaded;
		public static LogLevel LogLevelE
		{
			get
			{
				if (!logLevelLoaded)
				{
					try
					{
						logLevelE = (LogLevel)Enum.Parse(typeof(LogLevel), ConfigurationManager.AppSettings["LogLevel"], true);
					}
					catch
					{
					} // use the default value of LogLevel

					logLevelLoaded = true;
				}

				return logLevelE;
			}
		}

		public static int MaxLogSizeInBytes
		{
			get { return GetSettingsInt("MaxLogSizeInBytes"); }
		}

		public static int LogFilesCount
		{
			get { return GetSettingsInt("LogFilesCount"); }
		}

		private static int GetSettingsInt(string keyName)
		{
			int result;
			int.TryParse(ConfigurationManager.AppSettings[keyName], out result);
			return result;
		}
	}
}
