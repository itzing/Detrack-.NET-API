using System;

namespace Detrack.Infrastructure.Logging
{
	/// <summary>
	/// 
	/// A simple logging interface abstracting logging APIs.
	///
	/// <p> The five logging levels used by <code>Log</code> are (in order):
	/// <list type="numbered">
	/// <item>debug</item>
	/// <item>info</item>
	/// <item>warn</item>
	/// <item>error</item>
	/// <item>fatal (the most serious)</item>
	/// </list>
	/// 
	/// The mapping of these log levels to the concepts used by the underlying
	/// logging system is implementation dependent.
	/// The implemention should ensure, though, that this ordering behaves
	/// as expected.</p>
	///
	/// <p>Performance is often a logging concern.
	/// By examining the appropriate property,
	/// a component can avoid expensive operations (producing information
	/// to be logged).</p>
	///	
	/// Configuration of the underlying logging system will generally be done
	/// external to the Logging APIs, through whatever mechanism is supported by
	/// that system.	
	/// </summary>	
	/// 
	/// <example>
	/// For example,
	/// <code>
	///    if (log.IsDebugEnabled()) 
	///    {
	///        ... do something expensive ...
	///        log.debug(theResult);
	///    }
	/// </code>
	/// </example>
	public interface ILog
	{
		void Debug<T>(T message);
		void Debug<T>(Exception exception, T message);
		void Debug(string format, params object[] args);
		void Debug(Exception exception, string format, params object[] args);

		void Info<T>(T message);
		void Info<T>(Exception exception, T message);
		void Info(string format, params object[] args);
		void Info(Exception exception, string format, params object[] args);

		void Warn<T>(T message);
		void Warn<T>(Exception exception, T message);
		void Warn(string format, params object[] args);
		void Warn(Exception exception, string format, params object[] args);

		void Error<T>(T message);
		void Error<T>(Exception exception, T message);
		void Error(string format, params object[] args);
		void Error(Exception exception, string format, params object[] args);

		void Fatal<T>(T message);
		void Fatal<T>(Exception exception, T message);
		void Fatal(string format, params object[] args);
		void Fatal(Exception exception, string format, params object[] args);

		/// <summary>
		/// Property to determine if Debug logging level is enabled
		/// </summary>	
		bool IsDebugEnabled { get; }

		/// <summary>
		/// Property to determine if Info logging level is enabled
		/// </summary>	
		bool IsInfoEnabled { get; }

		/// <summary>
		/// Property to determine if Warn logging level is enabled
		/// </summary>	
		bool IsWarnEnabled { get; }

		/// <summary>
		/// Property to determine if Error logging level is enabled
		/// </summary>	
		bool IsErrorEnabled { get; }

		/// <summary>
		/// Property to determine if Fatal logging level is enabled
		/// </summary>		
		bool IsFatalEnabled { get; }
	}
}


