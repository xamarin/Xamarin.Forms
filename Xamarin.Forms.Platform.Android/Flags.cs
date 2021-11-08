using System.Linq;

namespace Xamarin.Forms
{
	internal static class Flags
	{
		internal const string UseLegacyRenderers = "UseLegacyRenderers";

		internal const string AccessibilityExperimental = "Accessibility_Experimental";

		public static bool IsFlagSet(string flagName)
		{
			return Device.Flags != null && Device.Flags.Contains(flagName);
		}

		public static bool IsAccessibilityExperimentalSet()
		{
			return IsFlagSet(AccessibilityExperimental);
		}
	}
}