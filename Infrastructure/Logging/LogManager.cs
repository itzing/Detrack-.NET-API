using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Detrack.Infrastructure.Logging.log4net;

namespace Detrack.Infrastructure.Logging
{
	public static class LogManager
	{
		// why logger configured in code instead of config file?????????
		public volatile static ILog NullLog = new NullLogger();
		private static readonly object sync = new object();
		private static readonly Dictionary<string, LoggerFactoryBase> factories = new Dictionary<string, LoggerFactoryBase>();
		private static LoggerFactoryBase factory;

		static LogManager()
		{
			lock (sync)
			{
				Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
				AssemblyName aname = assembly.GetName();
				string output = Path.Combine(ApplicationSettings.LogsFolder, string.Format("{0}_v{1}.log", aname.Name, aname.Version.ToString(3)));

				factory = new Log4NetFactory(ApplicationSettings.LogLevelE, output);
				
			}
		}


		public static void AddLogFactory(string factoryName, string logFileName)
		{
			Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
			AssemblyName aname = assembly.GetName();
			string output = Path.Combine(ApplicationSettings.LogsFolder, string.Format("{0}_{1}_v{2}.log", aname.Name, logFileName, aname.Version.ToString(3)));
			factories.Add(factoryName, new Log4NetFactory(ApplicationSettings.LogLevelE, output));
		}

		

		public static LoggerFactoryBase Factory
		{
			get { return factory; }
			set
			{
				factory = value ?? new NullFactory();
			}
		}

		public static void SetCrashReportSender(ICrashReportSender sender)
		{
			if (sender == null)
				throw new ArgumentNullException("sender");

			lock (sync)
			{
				if (factory != null)
					factory.CrashReportSender = sender;
			}
		}

		public static ILog GetLog<T>()
		{
			return GetLog(typeof(T).Name);
		}

		public static ILog GetLog(Type type)
		{
			return type == null
				? NullLog
				: GetLog(type.Name);
		}

		public static ILog GetLog(string name)
		{
			lock (sync)
			{
				if (name.StartsWith("."))
					name = name.TrimStart('.');

				return factory == null
					? NullLog
					: factory.GetLogger(name);
			}
		}

		public static string GetLogFileName(string additionName)
		{
			Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
			AssemblyName aname = assembly.GetName();
			return Path.Combine(ApplicationSettings.LogsFolder, string.Format("{0}_{1}_v{2}.log", aname.Name, additionName, aname.Version.ToString(3)));
		}

		public static ILog GetFileLog(string name)
		{
			lock (sync)
			{
				if (string.IsNullOrEmpty(name))
					return NullLog;
		
				if (name.StartsWith("."))
					name = name.TrimStart('.');

				if (!string.IsNullOrEmpty(name))
				{
					var loglevel = ApplicationSettings.LogLevelE;
					return factory.GetFileLogger(name, GetLogFileName(name), loglevel);
				}
				
				return factory == null
					? NullLog
					: factory.GetLogger(name);
			}
		}

	   
	}
}