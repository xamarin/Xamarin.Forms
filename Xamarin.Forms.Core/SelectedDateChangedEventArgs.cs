using System;

namespace Xamarin.Forms
{
	public class SelectedDateChangedEventArgs : EventArgs
	{
		public DateTime? OldDate { get; }
		public DateTime? NewDate { get; }

		public SelectedDateChangedEventArgs(DateTime? oldDate, DateTime? newDate)
		{
			OldDate = oldDate;
			NewDate = newDate;
		}
	}
}
