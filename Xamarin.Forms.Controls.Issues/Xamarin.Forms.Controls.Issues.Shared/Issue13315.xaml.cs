using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13315,
		"[Bug] Path renders incorrectly on iOS but correctly on Android",
		PlatformAffected.iOS)]
	public partial class Issue13315 : TestContentPage
	{
		public Issue13315()
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