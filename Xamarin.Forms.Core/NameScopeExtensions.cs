using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public static class NameScopeExtensions
	{
		public static T FindByName<T>(this Element element, string name)
		{
			try {
				return (T)element.FindByName(name);
			}
			catch (InvalidCastException) {
				if (DesignMode.IsDesignModeEnabled)
					return default(T);
				throw;
			}
		}

		internal static T FindByName<T>(this INameScope namescope, string name)
			=> (T)namescope.FindByName(name);
	}
}