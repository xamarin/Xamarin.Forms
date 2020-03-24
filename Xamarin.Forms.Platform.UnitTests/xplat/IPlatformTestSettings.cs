using System.Collections.Generic;
using System.Reflection;

namespace Xamarin.Forms.Platform.UnitTests
{
	public interface IPlatformTestSettings
	{
		Assembly Assembly { get; }
		Dictionary<string, object> TestRunSettings { get; }
	}
}
