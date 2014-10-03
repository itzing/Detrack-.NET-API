using System;

namespace Detrack.Infrastructure.Logging
{
	public class NullLogger : ILog
	{
		void ILog.Debug<T>(T message)
		{

		}

		void ILog.Debug<T>(Exception exception, T message)
		{

		}

		void ILog.Debug(string format, params object[] args)
		{

		}

		void ILog.Debug(Exception exception, string format, params object[] args)
		{

		}

		void ILog.Info<T>(T message)
		{

		}

		void ILog.Info<T>(Exception exception, T message)
		{

		}

		void ILog.Info(string format, params object[] args)
		{
		}

		void ILog.Info(Exception exception, string format, params object[] args)
		{

		}

		void ILog.Warn<T>(T message)
		{

		}

		void ILog.Warn<T>(Exception exception, T message)
		{

		}

		void ILog.Warn(string format, params object[] args)
		{

		}

		void ILog.Warn(Exception exception, string format, params object[] args)
		{

		}

		void ILog.Error<T>(T message)
		{

		}

		void ILog.Error<T>(Exception exception, T message)
		{
		}

		void ILog.Error(string format, params object[] args)
		{
		}

		void ILog.Error(Exception exception, string format, params object[] args)
		{

		}

		void ILog.Fatal<T>(T message)
		{

		}

		void ILog.Fatal<T>(Exception exception, T message)
		{

		}

		void ILog.Fatal(string format, params object[] args)
		{

		}

		void ILog.Fatal(Exception exception, string format, params object[] args)
		{

		}

		/// <summary>
		/// Property to determine if Debug logging level is enabled
		/// </summary>	
		bool ILog.IsDebugEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Property to determine if Info logging level is enabled
		/// </summary>	
		bool ILog.IsInfoEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Property to determine if Warn logging level is enabled
		/// </summary>	
		bool ILog.IsWarnEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Property to determine if Error logging level is enabled
		/// </summary>	
		bool ILog.IsErrorEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Property to determine if Fatal logging level is enabled
		/// </summary>		
		bool ILog.IsFatalEnabled
		{
			get { return false; }
		}
	}
}