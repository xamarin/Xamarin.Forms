using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14880, "[Bug] CollectionView item renderers never get disposed on Android (Memory Leak)", PlatformAffected.Android)]
	public partial class Issue14880 : TestContentPage
	{
		public Issue14880()
		{
#if APP
			InitializeComponent();
			BindingContext = new Issue14880ViewModel();
#endif
		}

		protected override void Init()
		{
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue14880ViewModel
	{
		public ObservableCollection<Issue14880Model> Items { get; set; }

		public Issue14880ViewModel()
		{
			var collection = new ObservableCollection<Issue14880Model>();
			var pageSize = 5;

			for (var i = 0; i < pageSize; i++)
			{
				collection.Add(new Issue14880Model()
				{
					Text = "Item " + i,
				});
			}

			Items = collection;
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue14880Model
	{
		public string Text { get; set; }
	}
}