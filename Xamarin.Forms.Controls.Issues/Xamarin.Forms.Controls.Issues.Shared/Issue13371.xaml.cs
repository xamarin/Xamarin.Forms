using System.Collections.ObjectModel;
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
	[Issue(IssueTracker.Github, 13371,
		"[Bug] CollectionView Header vs EmptyView conflict",
		PlatformAffected.iOS)]
	public partial class Issue13371 : TestContentPage
	{
		public Issue13371()
		{
#if APP
			InitializeComponent();
			BindingContext = this;
#endif
		}

		public ObservableCollection<string> Data { get; private set; }

		protected override void Init()
		{
			Data = new ObservableCollection<string>
			{
				"Item 1",
				"Item 2",
				"Item 3"
			};
		}
	}
}