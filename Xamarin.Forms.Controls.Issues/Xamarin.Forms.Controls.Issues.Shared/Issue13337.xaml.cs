using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13337,
		"[Bug] [5.0] [Android] Interacting with a SwipeView on a TabbedPage with IsSwipePagingEnabled=false re-enables page swiping",
		PlatformAffected.Android)]
	public partial class Issue13337 : TestTabbedPage
	{
		public Issue13337()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}
	}
}