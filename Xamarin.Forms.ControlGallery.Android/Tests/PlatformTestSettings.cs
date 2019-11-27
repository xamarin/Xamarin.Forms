using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls.Tests;
using Xamarin.Forms.Internals;

[assembly: Dependency(typeof(PlatformTestSettings))]
namespace Xamarin.Forms.ControlGallery.Android
{
	[Preserve(AllMembers = true)]
	public class PlatformTestSettings : IPlatformTestSettings
	{
		public Assembly Assembly { get => Assembly.GetExecutingAssembly(); }
		public Dictionary<string, object> TestRunSettings { get => new Dictionary<string, object>(); }
	}
}