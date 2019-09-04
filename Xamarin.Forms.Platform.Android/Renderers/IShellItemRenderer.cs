using Android.Support.V4.App;
using System;

namespace Xamarin.Forms.Platform.Android
{
	public interface IShellItemRenderer : IDisposable
	{
		Fragment Fragment { get; }

		ShellItem ShellItem { get; set; }

		[Obsolete("Destroyed event is obsolete as of version 4.2.0.")]
		event EventHandler Destroyed;
	}
}