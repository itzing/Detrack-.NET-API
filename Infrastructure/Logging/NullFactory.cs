namespace Detrack.Infrastructure.Logging
{
	class NullFactory :  LoggerFactoryBase
	{
		private volatile static ILog log = new NullLogger();

		protected override ILog CreateLogger(string name)
		{
			return log;
		}
	}
}