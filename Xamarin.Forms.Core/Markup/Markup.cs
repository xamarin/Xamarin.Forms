using System.Runtime.CompilerServices;

namespace Xamarin.Forms.Core.Markup
{
	internal static class Markup
	{
		static bool isExperimentalFlagSet = false;

		internal static void VerifyExperimental([CallerMemberName] string memberName = "")
		{
			if (isExperimentalFlagSet)
				return;

			ExperimentalFlags.VerifyFlagEnabled(nameof(Markup), ExperimentalFlags.MarkupExperimental, memberName: memberName);

			isExperimentalFlagSet = true;
		}

		internal static void ClearExperimental() => isExperimentalFlagSet = false;
	}
}
