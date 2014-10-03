namespace Detrack.Infrastructure.Logging.log4net
{
	public interface ICrashReportSender
	{
		void Send(string crashLog, string crashMessage);
	}
}