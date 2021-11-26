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
	[Category(UITestCategories.TabbedPage)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14917, "TabbedPage backgrouncolor on Ios not set in IoS 15", PlatformAffected.iOS)]
	public partial class Issue14917 : TabbedPage
	{
		public Issue14917()
		{
#if APP
			InitializeComponent();
#endif
		}
	}
}