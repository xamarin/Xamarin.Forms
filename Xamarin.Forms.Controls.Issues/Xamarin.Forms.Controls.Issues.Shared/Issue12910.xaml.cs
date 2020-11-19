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
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12910,
		"[Bug] 'Cannot access a disposed object. Object name: 'DefaultRenderer' - on ios with CollectionView and EmptyView",
		PlatformAffected.iOS)]
	public partial class Issue12910 : TestContentPage
	{
		public Issue12910()
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