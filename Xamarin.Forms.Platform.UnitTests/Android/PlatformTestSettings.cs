using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UnitTests.Android;

[assembly: Dependency(typeof(PlatformTestSettings))]
namespace Xamarin.Forms.Platform.UnitTests.Android
{
	public class PlatformTestSettings : IPlatformTestSettings
	{
		public PlatformTestSettings()
		{
			TestRunSettings = new Dictionary<string, object>{};
		}

		public Assembly Assembly { get => Assembly.Load("Xamarin.Forms.Platform.UnitTests.Android, Version = 2.0.0.0, Culture = neutral, PublicKeyToken = null"); }
		public Dictionary<string, object> TestRunSettings { get; }
	}
}