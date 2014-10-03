using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Detrack.Infrastructure.Logging.log4net
{
	public class Log4NetConsoleFactory : LoggerFactoryBase
	{
		public Log4NetConsoleFactory(LogLevel level)
		{
			var hierarchy = (Hierarchy)global::log4net.LogManager.GetRepository();
			hierarchy.Root.RemoveAllAppenders();

			var layout = new PatternLayout { ConversionPattern = "%utcdate{ISO8601} [%thread] %-3level %-8logger{1} - %message%newline" };
			layout.ActivateOptions();

			var appender = new ColoredConsoleAppender
			{
				Layout = layout,
				Threshold = ToLog4NetLevel(level)
			};

			appender.AddMapping(new ColoredConsoleAppender.LevelColors { ForeColor = ColoredConsoleAppender.Colors.HighIntensity, Level = Level.Debug });
			appender.AddMapping(new ColoredConsoleAppender.LevelColors { ForeColor = ColoredConsoleAppender.Colors.Cyan, Level = Level.Info });
			appender.AddMapping(new ColoredConsoleAppender.LevelColors { ForeColor = ColoredConsoleAppender.Colors.White, BackColor = ColoredConsoleAppender.Colors.Yellow, Level = Level.Warn });
			appender.AddMapping(new ColoredConsoleAppender.LevelColors { ForeColor = ColoredConsoleAppender.Colors.White, BackColor = ColoredConsoleAppender.Colors.Red, Level = Level.Error });
			appender.AddMapping(new ColoredConsoleAppender.LevelColors { ForeColor = ColoredConsoleAppender.Colors.White, BackColor = ColoredConsoleAppender.Colors.Red, Level = Level.Fatal });

			appender.ActivateOptions();

			hierarchy.Root.AddAppender(appender);
			hierarchy.Configured = true;
		}

		protected override ILog CreateLogger(string name)
		{
			return new Logger4Net(name);
		}

		private Level ToLog4NetLevel(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Debug: return Level.Debug;
				case LogLevel.Error: return Level.Error;
				case LogLevel.Info: return Level.Info;
				case LogLevel.None: return Level.Off;
			}
			return Level.Error;
		}
	}
}