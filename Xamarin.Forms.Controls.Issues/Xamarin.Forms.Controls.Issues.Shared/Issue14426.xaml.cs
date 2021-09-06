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
	[Category(UITestCategories.Frame)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14426,
		"[Bug] [Regression] iOS Frame.Background not working",
		PlatformAffected.iOS)]
	public partial class Issue14426 : TestContentPage
	{
		public Issue14426()
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