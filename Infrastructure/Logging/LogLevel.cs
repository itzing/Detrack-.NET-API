using System;

namespace Detrack.Infrastructure.Logging
{
	[Flags]
	public enum LogLevel
	{
		None  = 0,
		Fatal = 1,
		Error = 1 << 1 | Fatal,
		Warn  = 1 << 2 | Error,
		Info  = 1 << 3 | Warn,
		Debug = 1 << 4 | Info,
		Full  = Debug // do not remove, for backward compatibility
	}
}
