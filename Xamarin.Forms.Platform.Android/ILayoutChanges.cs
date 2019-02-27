using System;
using ALayoutChangeEventArgs = Android.Views.View.LayoutChangeEventArgs;

namespace Xamarin.Forms.Platform.Android
{
	public interface ILayoutChanges
	{
		event EventHandler<ALayoutChangeEventArgs> LayoutChange;
	}
}