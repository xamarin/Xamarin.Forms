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
	[Category(UITestCategories.Button)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13103, "[Bug] [iOS] CollectionView EmptyView causes the application to crash",
		PlatformAffected.iOS)]
	public partial class Issue13103 : TestContentPage
	{
		public Issue13103()
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