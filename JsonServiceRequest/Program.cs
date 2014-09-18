using System;
using System.Threading;
using System.Windows.Forms;
using Detrack.Infrastructure.Exceptions;

namespace Detrack.Main
{
	static class Program
	{
		static readonly Mutex mutex = new Mutex(true, "{90CFA72E-0CCB-4950-9F1C-B627B7C5D57A}");

		[STAThread]
		static void Main(string[] args)
		{
			if (mutex.WaitOne(TimeSpan.Zero, true))
			{
				try
				{
					Application.Run(new DetrackSynchronizer());
				}
				catch (ApplicationShutdownException)
				{
					mutex.ReleaseMutex();
					Application.Exit();
				}
			}
			else 
			{
				Application.Exit();
			}
		}
	}
}
