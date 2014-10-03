using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Detrack.Infrastructure
{
	[Serializable]
	public class LocalStorage
	{
		private const string FileName = "LocalStorage.xml";
		private static readonly object LockFlag = new object();
		private static LocalStorage _instance;

		public LocalStorage()
		{
			LastDoProcessed = 0;
		}

		public long LastDoProcessed { get; set; }

		public static LocalStorage Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (LockFlag)
					{
						if (_instance == null)
						{
							var localStorage = Load() ?? new LocalStorage();
							Thread.MemoryBarrier();
							_instance = localStorage;
						}
					}
				}

				return _instance;
			}
		}

		public static string CommonApplicationDataFolder
		{
			get
			{
				string result = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ApplicationId"]))
					result = Path.Combine(result, ConfigurationManager.AppSettings["ApplicationId"]);

				return Path.Combine(result, "CBS DetrackSync");
			}
		}

		private static LocalStorage Load()
		{
			lock (LockFlag)
			{
				try
				{
					string configFileName = Path.Combine(CommonApplicationDataFolder, FileName);
					if (File.Exists(configFileName))
					{
						using (var fileStream = new FileStream(configFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							var xmlSerializer = new XmlSerializer(typeof(LocalStorage));
							return (LocalStorage)xmlSerializer.Deserialize(fileStream);
						}
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.TraceError("Error in GetConfigFromFile() : {0}", ex.Message);
				}

				return null;
			}
		}

		public void Save()
		{
			lock (LockFlag)
			{
				using (var fileStream = new FileStream(Path.Combine(CommonApplicationDataFolder, FileName), FileMode.Create))
				{
					var xmlSerializer = new XmlSerializer(typeof(LocalStorage));
					var ns = new XmlSerializerNamespaces();
					ns.Add("", "");

					using (
						var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true }))
					{
						xmlSerializer.Serialize(xmlWriter, this, ns);
					}
				}
			}
		}
	}

	public interface IConfigProvider
	{
		LocalStorage LoadConfig();
	}

	public class ConfigProvider : IConfigProvider
	{
		public LocalStorage LoadConfig()
		{
			return LocalStorage.Instance;
		}
	}
}
