using System;
using System.Globalization;
using System.IO;
using log4net.Appender;
using log4net.Core;

namespace Detrack.Infrastructure.Logging.log4net
{
	public class CrashLogAppender : BufferingAppenderSkeleton
	{
		private ICrashReportSender crashReportSender;

		public bool SendAsync { get; set; }

		protected override bool RequiresLayout
		{
			get { return true; }
		}

		public ICrashReportSender CrashReportSender
		{
			get { return crashReportSender; }
			set { crashReportSender = value; }
		}

		protected override void SendBuffer(LoggingEvent[] events)
		{
			if (crashReportSender == null || events == null || events.Length == 0)
				return;

			try
			{
				using (var writer = new StringWriter(CultureInfo.InvariantCulture))
				{
					for (int index = 0; index < events.Length; ++index)
						RenderLoggingEvent(writer, events[index]);

					LoggingEvent le = events[events.Length - 1];

					string crashMessage = le.ExceptionObject == null
										 ? string.Format("MSG - {0}", le.MessageObject)
										 : string.Format("EXC - {0} : {1}", le.ExceptionObject.GetType().Name, le.ExceptionObject.Message);

					if (SendAsync)
					{
						Action<string, string> action = crashReportSender.Send;
						action.BeginInvoke(writer.ToString(), crashMessage, Sent, action);
					}
					else
					{
						crashReportSender.Send(writer.ToString(), crashMessage);
					}
				}
			}
			catch (Exception ex)
			{
				ErrorHandler.Error("Error occurred while sending crash report to the web.", ex);
			}
		}

		private void Sent(IAsyncResult ar)
		{
			try
			{
				((Action<string, string>)ar.AsyncState).EndInvoke(ar);
			}
			catch (Exception e)
			{
				ErrorHandler.Error(string.Format("Error occurred while sending crash report to the web. {0}", e.Message));
			}
		}
	}
}