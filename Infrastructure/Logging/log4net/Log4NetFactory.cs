using System;
using System.Text;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Detrack.Infrastructure.Logging.log4net
{
	public class Log4NetFactory : LoggerFactoryBase
	{
		private CrashLogAppender crashLogAppender;
		private const int bufferSize = 20;
		private const FixFlags fixFlags = FixFlags.Message | FixFlags.ThreadName | FixFlags.Exception;

		public Log4NetFactory(LogLevel level, string outputFile)
		{
			var hierarchy = (Hierarchy)global::log4net.LogManager.GetRepository();
			hierarchy.Root.RemoveAllAppenders();

			var layout = new PatternLayout { ConversionPattern = "%utcdate{ISO8601} [%thread] %-7level %-15logger{1} - %message%newline" };
			layout.ActivateOptions();

			IAppender appender = level == LogLevel.Error
									 ? NewBufferingFileAppender(outputFile, layout)
									 : NewRollingFileAppender(outputFile, ToLog4NetLevel(level), layout);

			hierarchy.Root.AddAppender(appender);
			hierarchy.Root.AddAppender(NewCrashLogAppender(layout));

			hierarchy.Configured = true;
		}

		private IAppender NewBufferingFileAppender(string outputFile, PatternLayout layout)
		{
			var buffered = new BufferingForwardingAppender
			{
				BufferSize = bufferSize,
				Evaluator = new LevelEvaluator(Level.Error),
				Lossy = true,
				Fix = fixFlags,
				ErrorHandler = new NullErrorHandler()
			};

			buffered.AddAppender(NewRollingFileAppender(outputFile, Level.All, layout));
			buffered.ActivateOptions();

			return buffered;
		}

		private IAppender NewRollingFileAppender(string outputFile, Level threshold, PatternLayout layout)
		{
			var roll = new RollingFileAppender
						{
							AppendToFile = true,
							LockingModel = new FileAppender.MinimalLock(),
							File = outputFile,
							Layout = layout,
							Threshold = threshold,
							RollingStyle = RollingFileAppender.RollingMode.Size,
							MaxFileSize = ApplicationSettings.MaxLogSizeInBytes,
							MaxSizeRollBackups = ApplicationSettings.LogFilesCount,
							ImmediateFlush = true,
							StaticLogFileName = true,
							Encoding = Encoding.UTF8,
							ErrorHandler = new NullErrorHandler()
						};
			roll.ActivateOptions();
			return roll;
		}

		private IAppender NewCrashLogAppender(PatternLayout layout)
		{
			crashLogAppender = new CrashLogAppender
				{
					BufferSize = bufferSize,
					Evaluator = new LevelEvaluator(Level.Fatal),
					Lossy = true,
					Fix = fixFlags,
					Layout = layout,
					SendAsync = true,
					ErrorHandler = new NullErrorHandler()
				};

			crashLogAppender.ActivateOptions();

			return crashLogAppender;
		}

		public override ICrashReportSender CrashReportSender
		{
			get
			{
				return crashLogAppender == null
					? null
					: crashLogAppender.CrashReportSender;
			}
			set
			{
				if (crashLogAppender != null)
					crashLogAppender.CrashReportSender = value;
			}
		}

		protected override ILog CreateLogger(string name)
		{
			return new Logger4Net(name);
		}

		private Level ToLog4NetLevel(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Fatal: return Level.Fatal;
				case LogLevel.Error: return Level.Error;
				case LogLevel.Warn: return Level.Warn;
				case LogLevel.Info: return Level.Info;
				case LogLevel.Debug: return Level.Debug;
				case LogLevel.None: return Level.Off;
			}
			return Level.Error;
		}

		private class NullErrorHandler : IErrorHandler
		{
			public void Error(string message, Exception e, ErrorCode errorCode) { }

			public void Error(string message, Exception e) { }

			public void Error(string message) { }
		}
	}
}