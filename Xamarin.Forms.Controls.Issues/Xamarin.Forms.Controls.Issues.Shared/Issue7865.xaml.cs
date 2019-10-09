using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CarouselView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7865, "[iOS] CarouselView setting Position has dif behavior than ScrollTo")]
	public partial class Issue7865 : TestContentPage
	{
		public Issue7865()
		{
#if APP
			Device.SetFlags(new List<string> { CollectionView.CollectionViewExperimental });
			Title = "Issue 7865";
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
		
		}
	}
}