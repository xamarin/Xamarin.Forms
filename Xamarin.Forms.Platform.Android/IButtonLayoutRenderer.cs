using System;
using Android.Support.V7.Widget;
using AButton = Android.Widget.Button;

namespace Xamarin.Forms.Platform.Android
{
	public interface IButtonLayoutRenderer
	{
		AButton View { get; }		
		Button Element { get; }
		event EventHandler<VisualElementChangedEventArgs> ElementChanged;
	}
}
