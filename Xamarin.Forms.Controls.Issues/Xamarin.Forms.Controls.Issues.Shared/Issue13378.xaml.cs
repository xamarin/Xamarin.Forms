using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Brush)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13378, "[Bug] iOS LinearGradientBrush Transparent Color",
		PlatformAffected.iOS)]
	public partial class Issue13378 : TestContentPage
	{
		public Issue13378()
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