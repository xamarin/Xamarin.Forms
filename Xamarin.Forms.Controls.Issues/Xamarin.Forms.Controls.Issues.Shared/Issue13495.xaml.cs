using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13495,
		"[Bug] Regression in XF5: SwipeView glitches when pulled out",
		PlatformAffected.iOS)]
	public partial class Issue13495 : TestContentPage
	{
		public Issue13495()
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