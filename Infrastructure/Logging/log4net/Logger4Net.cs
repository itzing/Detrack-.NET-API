using System;

namespace Detrack.Infrastructure.Logging.log4net
{
	public class Logger4Net : ILog
	{
		private readonly global::log4net.ILog log;

		public Logger4Net(string name)
		{
			Guard.AgainstNullOrEmpty(name, "name");
			
			log = global::log4net.LogManager.GetLogger(name);
		}

		public void Debug<T>(T message)
		{
			log.Debug(message);
		}

		public void Debug<T>(Exception exception, T message)
		{
			log.Debug(message, exception);
		}

		public void Debug(string format, params object[] args)
		{
			log.DebugFormat(format, args);
		}

		public void Debug(Exception exception, string format, params object[] args)
		{
			try
			{
				log.Debug(string.Format(format, args), exception);
			}
			catch (FormatException)
			{
				log.Debug(BadFormat(format, args), exception);
			}
		}

		public void Info<T>(T message)
		{
			log.Info(message);
		}

		public void Info<T>(Exception exception, T message)
		{
			log.Info(message, exception);
		}

		public void Info(string format, params object[] args)
		{
			log.InfoFormat(format, args);
		}

		public void Info(Exception exception, string format, params object[] args)
		{
			try
			{
				log.Info(string.Format(format, args), exception);
			}
			catch (FormatException)
			{
				log.Info(BadFormat(format, args), exception);
			}
		}

		public void Warn<T>(T message)
		{
			log.Warn(message);
		}

		public void Warn<T>(Exception exception, T message)
		{
			log.Warn(message, exception);
		}

		public void Warn(string format, params object[] args)
		{
			log.WarnFormat(format, args);
		}

		public void Warn(Exception exception, string format, params object[] args)
		{
			try
			{
				log.Warn(string.Format(format, args), exception);
			}
			catch (FormatException)
			{
				log.Warn(BadFormat(format, args), exception);
			}
		}

		public void Error<T>(T message)
		{
			log.Error(message);
		}

		public void Error<T>(Exception exception, T message)
		{
			log.Error(message, exception);
		}

		public void Error(string format, params object[] args)
		{
			log.ErrorFormat(format, args);
		}

		public void Error(Exception exception, string format, params object[] args)
		{

			try
			{
				log.Error(string.Format(format, args), exception);
			}
			catch (FormatException)
			{
				log.Error(BadFormat(format, args), exception);
			}
		}

		public void Fatal<T>(T message)
		{
			log.Fatal(message);
		}

		public void Fatal<T>(Exception exception, T message)
		{
			log.Fatal(message, exception);
		}

		public void Fatal(string format, params object[] args)
		{
			log.FatalFormat(format, args);
		}

		public void Fatal(Exception exception, string format, params object[] args)
		{
			try
			{
				log.Fatal(string.Format(format, args), exception);
			}
			catch (FormatException)
			{
				log.Fatal(BadFormat(format, args), exception);
			}
		}

		public bool IsDebugEnabled
		{
			get { return log.IsDebugEnabled; }
		}

		public bool IsInfoEnabled
		{
			get { return log.IsInfoEnabled; }
		}

		public bool IsWarnEnabled
		{
			get { return log.IsWarnEnabled; }
		}

		public bool IsErrorEnabled
		{
			get { return log.IsErrorEnabled; }
		}

		public bool IsFatalEnabled
		{
			get { return log.IsFatalEnabled; }
		}

		private static string BadFormat(string format, object[] args)
		{
			string arguments = args == null
								? "NULL"
								: string.Join("; ", Array.ConvertAll(args, a => a.ToString()));

			return string.Format("{0}\r\nBad message format! Arguments: {1}", format, arguments);
		}
	}
}
