using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Detrack.Infrastructure.Logging
{
	public class FileLogger : ILog
	{
		private readonly LogLevel logLevel;
		private readonly string logFilePath;
		private readonly object sync = new object();
		private readonly Queue<string> messages;
		private const int messagesBuffer = 20;
		private const long maxFileSize = 2097152; // 2Mb

		public FileLogger(string outputDirectory, string fileName, LogLevel logLevel)
		{
			if (string.IsNullOrEmpty(outputDirectory))
				throw new ArgumentNullException("outputDirectory");

			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");

			this.logLevel = logLevel;
			messages = new Queue<string>(messagesBuffer + 1);

			if (!Directory.Exists(outputDirectory))
				Directory.CreateDirectory(outputDirectory);

			logFilePath = Path.Combine(outputDirectory, fileName);
		}

		#region ILog Members

		public bool IsDebugEnabled
		{
			get { return (logLevel & LogLevel.Debug) == LogLevel.Debug; }
		}

		public bool IsInfoEnabled
		{
			get { return (logLevel & LogLevel.Info) == LogLevel.Info; }
		}

		public bool IsWarnEnabled
		{
			get { return (logLevel & LogLevel.Warn) == LogLevel.Warn; }
		}

		public bool IsErrorEnabled
		{
			get { return (logLevel & LogLevel.Error) == LogLevel.Error; }
		}

		public bool IsFatalEnabled
		{
			get { return (logLevel & LogLevel.Fatal) == LogLevel.Fatal; }
		}

		#region Debug

		public void Debug<T>(Exception exception, T message)
		{
			var builder = new StringBuilder(message.ToString());
			BuildException(exception, builder);
			Debug(builder);
		}

		public void Debug(string format, params object[] args)
		{
			Debug<string>(FormatMessage(format, args));
		}

		public void Debug(Exception exception, string format, params object[] args)
		{
			var builder = new StringBuilder(FormatMessage(format, args));
			BuildException(exception, builder);
			Debug(builder);
		}

		public void Debug<T>(T message)
		{
			Append(LogLevel.Debug, message.ToString());
		}

		#endregion

		#region Info

		public void Info<T>(Exception exception, T message)
		{
			var builder = new StringBuilder(message.ToString());
			BuildException(exception, builder);
			Info(builder);
		}

		public void Info(string format, params object[] args)
		{
			Info<string>(FormatMessage(format, args));
		}

		public void Info(Exception exception, string format, params object[] args)
		{
			var builder = new StringBuilder(FormatMessage(format, args));
			BuildException(exception, builder);
			Info(builder);
		}

		public void Info<T>(T message)
		{
			Append(LogLevel.Info, message.ToString());
		}

		#endregion

		#region Warn

		public void Warn<T>(Exception exception, T message)
		{
			var builder = new StringBuilder(message.ToString());
			BuildException(exception, builder);
			Warn(builder);
		}

		public void Warn(string format, params object[] args)
		{
			Warn<string>(FormatMessage(format, args));
		}

		public void Warn(Exception exception, string format, params object[] args)
		{
			var builder = new StringBuilder(FormatMessage(format, args));
			BuildException(exception, builder);
			Warn(builder);
		}

		public void Warn<T>(T message)
		{
			Append(LogLevel.Warn, message.ToString());
		}

		#endregion

		#region Error

		public void Error<T>(Exception exception, T message)
		{
			var builder = new StringBuilder(message.ToString());
			BuildException(exception, builder);
			Error(builder);
		}

		public void Error(string format, params object[] args)
		{
			Error<string>(FormatMessage(format, args));
		}

		public void Error(Exception exception, string format, params object[] args)
		{
			var builder = new StringBuilder(FormatMessage(format, args));
			BuildException(exception, builder);
			Error(builder);
		}

		public void Error<T>(T message)
		{
			Append(LogLevel.Error, message.ToString());
		}

		#endregion

		#region Fatal

		public void Fatal<T>(Exception exception, T message)
		{
			var builder = new StringBuilder(message.ToString());
			BuildException(exception, builder);
			Fatal(builder);
		}

		public void Fatal(string format, params object[] args)
		{
			Fatal<string>(FormatMessage(format, args));
		}

		public void Fatal(Exception exception, string format, params object[] args)
		{
			var builder = new StringBuilder(FormatMessage(format, args));
			BuildException(exception, builder, true);
			Fatal(builder);
		}

		public void Fatal<T>(T message)
		{
			Append(LogLevel.Fatal, message.ToString());
		}

		#endregion

		#endregion

		private static string FormatMessage(string format, object[] args)
		{
			try
			{
				return string.Format(format, args);
			}
			catch (FormatException e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}

			return format;
		}

		private void Append(LogLevel level, string message)
		{
			string fullmsg = string.Format("{0} [{1}]\t{2}\t{3}",
										   DateTime.Now.ToString(CultureInfo.InvariantCulture),
										   Thread.CurrentThread.ManagedThreadId,
										   level,
										   message);
			try
			{
				lock (sync)
				{
					messages.Enqueue(fullmsg);

					if (messages.Count > messagesBuffer)
						messages.Dequeue();

					if ((logLevel & level) == level)
					{
						var file = new FileInfo(logFilePath);
						bool append = file.Exists && file.Length < maxFileSize;

						using (var writer = new StreamWriter(logFilePath, append, Encoding.Default))
						{
							while (messages.Count > 0)
							{
								writer.WriteLine(messages.Dequeue());
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine(exception);
			}
		}

		private void BuildException(Exception e, StringBuilder builder, bool includeEnvironmentStackTrace = false)
		{
			while (e != null)
			{
				builder.AppendFormat("{0}[{1}] Message = [{2}]{0}{3}",
										 Environment.NewLine,
										 e.GetType().FullName,
										 e.Message,
										 e.StackTrace);

				e = e.InnerException;

				if (e != null)
					builder.AppendFormat("INNER EXCEPTION:{0}", Environment.NewLine);
			}

			if (includeEnvironmentStackTrace)
			{
				builder.AppendFormat("{0}ENVIRONMENT STACKTRACE:{0}", Environment.NewLine);
				builder.AppendLine(Environment.StackTrace);
			}
		}
	}
}
