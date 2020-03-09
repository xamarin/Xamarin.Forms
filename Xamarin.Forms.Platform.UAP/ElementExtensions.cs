using System;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Platform.UWP
{
	public static class ElementExtensions
	{
		internal static T FindParent<T>(this Element self)
			where T : class
		{
			Element parent = null;
			T returnvalue = default(T);

			do
			{
				parent = self?.Parent;
				returnvalue = parent as T;

			}
			while (returnvalue == null && parent != null);

			return returnvalue;
		}
	}
}