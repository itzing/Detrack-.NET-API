using System.Collections.Generic;
using System.IO;
using Detrack.Infrastructure.Logging.log4net;

namespace Detrack.Infrastructure.Logging
{
	public abstract class LoggerFactoryBase
	{
		/// <summary>
		/// Maps types to their loggers.
		/// </summary>
		private readonly Dictionary<string, ILog> loggers = new Dictionary<string, ILog>();

		public virtual ICrashReportSender CrashReportSender { get; set; }

		/// <summary>
		/// Gets the logger for the specified name, creating it if necessary.
		/// </summary>
		/// <param name="name">The name to create the logger for.</param>
		/// <returns>The newly-created logger.</returns>
		public ILog GetLogger(string name)
		{
			lock (loggers)
			{
				if (loggers.ContainsKey(name))
				{
					return loggers[name];
				}

				ILog logger = CreateLogger(name);
				loggers.Add(name, logger);

				return logger;
			}
		}

		public ILog GetFileLogger(string name, string logFileName, LogLevel level)
		{
			lock (loggers)
			{
				if (loggers.ContainsKey(name))
				{
					return loggers[name];
				}

				ILog logger = new FileLogger(Path.GetDirectoryName(logFileName), Path.GetFileName(logFileName), level);
				loggers.Add(name, logger);

				return logger;
			}
		}

		/// <summary>
		/// Creates a logger for the specified name.
		/// </summary>
		/// <param>The name to create the logger for. <name>type</name> </param>
		/// <returns>The newly-created logger.</returns>
		protected abstract ILog CreateLogger(string name);

	 }
}