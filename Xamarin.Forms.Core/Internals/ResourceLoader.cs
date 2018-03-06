using System;
using System.Reflection;

namespace Xamarin.Forms.Internals
{
	public static class ResourceLoader
	{
		static Func<AssemblyName, string, string> resourceProvider;

		/// <devdoc>
		/// This property setter is used by LiveReload to apply replace XAML loaded 
		/// by the app with the XAML pushed from the IDE.
		/// NOTE: give the IDE teams a heads-up if the signature or location of 
		/// this method changes :)
		/// </devdoc>
		//takes a resource path, returns string content
		public static Func<AssemblyName, string, string> ResourceProvider {
			get => resourceProvider;
			internal set {
				DesignMode.IsDesignModeEnabled = true;
				resourceProvider = value;
			}
		}

		internal static Action<Exception> ExceptionHandler { get; set; }
	}
}
