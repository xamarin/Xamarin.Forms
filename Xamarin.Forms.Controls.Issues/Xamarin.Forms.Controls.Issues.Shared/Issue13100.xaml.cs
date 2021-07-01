using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Brush)]
#endif
	[Issue(IssueTracker.Github, 13100,
		"[Bug] LinearGradientBrush GradientStop offset (Android)",
		PlatformAffected.Android)]
	public partial class Issue13100 : TestContentPage
	{
		public Issue13100()
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