using System;

namespace Detrack.Infrastructure.Logging
{
	public static class SysLog
	{
		private static ILog log;

		public static ILog Log
		{
			get { return log ?? (log = LogManager.GetLog("SysLog")); }
		}

		public static void Debug<T>(T message)
		{
			Log.Debug(message);
		}

		public static void Debug<T>(Exception exception, T message)
		{
			Log.Debug(exception, message);
		}

		public static void Debug(string format, params object[] args)
		{
			Log.Debug(format, args);
		}

		public static void Debug(Exception exception, string format, params object[] args)
		{
			Log.Debug(exception, format, args);
		}

		public static void Info<T>(T message)
		{
			Log.Info(message);
		}

		public static void Info<T>(Exception exception, T message)
		{
			Log.Info(exception, message);
		}

		public static void Info(string format, params object[] args)
		{
			Log.Info(format, args);
		}

		public static void Info(Exception exception, string format, params object[] args)
		{
			Log.Info(exception, format, args);
		}

		public static void Warn<T>(T message)
		{
			Log.Warn(message);
		}

		public static void Warn<T>(Exception exception, T message)
		{
			Log.Warn(exception, message);
		}

		public static void Warn(string format, params object[] args)
		{
			Log.Warn(format, args);
		}

		public static void Warn(Exception exception, string format, params object[] args)
		{
			Log.Warn(exception, format, args);
		}

		public static void Error<T>(T message)
		{
			Log.Error(message);
		}

		public static void Error<T>(Exception exception, T message)
		{
			Log.Error(exception, message);
		}

		public static void Error(string format, params object[] args)
		{
			Log.Error(format, args);
		}

		public static void Error(Exception exception, string format, params object[] args)
		{
			Log.Error(exception, format, args);
		}

		public static void Fatal<T>(T message)
		{
			Log.Fatal(message);
		}

		public static void Fatal<T>(Exception exception, T message)
		{
			Log.Fatal(exception, message);
		}

		public static void Fatal(string format, params object[] args)
		{
			Log.Fatal(format, args);
		}

		public static void Fatal(Exception exception, string format, params object[] args)
		{
			Log.Fatal(exception, format, args);
		}
	}
}