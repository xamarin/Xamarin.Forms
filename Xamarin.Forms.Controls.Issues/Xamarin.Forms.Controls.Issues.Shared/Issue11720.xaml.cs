using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.SwipeView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11720, "[Bug] Crash: Value cannot be null. Parameter name: descriptor <-- suddenly started crashing without editing code",
		PlatformAffected.iOS)]
	public partial class Issue11720 : ContentPage
	{
		public Issue11720()
		{
#if APP
			InitializeComponent();
#endif
		}
	}
}