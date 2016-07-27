using System;
namespace Xamarin.Forms
{
	class NativeViewEventFiredEventArgs : EventArgs
	{
		public object NativeEventArgs { get; set; }
		public string PropertyName { get; set; }
		public string EventName { get; set; }
	}
}

