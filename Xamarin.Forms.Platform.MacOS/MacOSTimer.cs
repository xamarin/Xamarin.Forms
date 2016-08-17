using System;
using System.Threading;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class MacOSTimer : ITimer
	{
		readonly Timer _timer;

		public MacOSTimer(Timer timer)
		{
			_timer = timer;
		}

		public void Change(int dueTime, int period)
		{
			_timer.Change(dueTime, period);
		}

		public void Change(long dueTime, long period)
		{
			_timer.Change(dueTime, period);
		}

		public void Change(TimeSpan dueTime, TimeSpan period)
		{
			_timer.Change(dueTime, period);
		}

		public void Change(uint dueTime, uint period)
		{
			_timer.Change(dueTime, period);
		}
	}
}

