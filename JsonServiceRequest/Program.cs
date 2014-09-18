using System;
using System.Threading;
using System.Windows.Forms;

namespace Detrack.Main
{
	static class Program
	{
		static readonly Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

		[STAThread]
		static void Main(string[] args)
		{
			if (mutex.WaitOne(TimeSpan.Zero, true))
			{
				var synchronizer = new DetrackSynchronizer();
				Application.Run(synchronizer);
				mutex.ReleaseMutex();
			}
			else 
			{
				Application.Exit();
			}
		}
	}
}
