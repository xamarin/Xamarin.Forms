using System;

namespace Xamarin.Forms.Platform.Android
{
	internal static class JavaObjectExtensions
	{
		public static bool IsJavaDisposed(this Java.Lang.Object obj)
		{
			return obj.Handle == IntPtr.Zero;
		}
	}
}