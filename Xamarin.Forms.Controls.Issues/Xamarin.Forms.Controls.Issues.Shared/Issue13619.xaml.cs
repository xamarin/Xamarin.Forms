using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13619,
		"[Bug] Threshold property only works for the first swipe item and will hide other items",
		PlatformAffected.Android | PlatformAffected.iOS)]
	public partial class Issue13619 : TestContentPage
	{ 
		public Issue13619()
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