using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12538, "[Bug] Shapes do not work with StaticResource Fill Colour", PlatformAffected.UWP)]
	public partial class Issue12538 : TestContentPage
	{
#if APP
		public Issue12538()
		{
			InitializeComponent();
		}
#endif

		protected override void Init()
		{

		}
	}
}