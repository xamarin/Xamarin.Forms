using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.WindowsUniversal.Tests;
using Xamarin.Forms.Controls.Tests;

[assembly: Dependency(typeof(PlatformTestSettings))]
namespace Xamarin.Forms.ControlGallery.WindowsUniversal.Tests
{
	public class PlatformTestSettings : IPlatformTestSettings
	{
		public Assembly Assembly { get => Assembly.GetExecutingAssembly(); }
		public Dictionary<string, object> TestRunSettings { get => new Dictionary<string, object>(); }
	}
}
