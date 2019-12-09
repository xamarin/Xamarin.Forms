using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7393, "[Bug] CollectionView problems and crashes with IsGrouped=\"true\"",
		PlatformAffected.iOS)]
	public class Issue7393 : TestContentPage
	{
		ObservableCollection<_7393Group> _source;

		protected override void Init()
		{
			var cv = new CollectionView();

			_source = new ObservableCollection<_7393Group>();

			var groupCount = 3;
			var itemCount = 3;

			//for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
			//{
			//	var group = new _7393Group { Header = $"{groupIndex} Header", Footer = $"{groupIndex} Footer" };

			//	for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
			//	{
			//		var item = new _7393Item { Name = $"{groupIndex}.{itemIndex} Item" };
			//		group.Add(item);
			//	}

			//	_source.Add(group);
			//}

			cv.GroupHeaderTemplate = new DataTemplate(() => {
				var label = new Label();

				label.SetBinding(Label.TextProperty, new Binding("Header"));

				return label;
			});

			cv.GroupFooterTemplate = new DataTemplate(() => {
				var label = new Label();

				label.SetBinding(Label.TextProperty, new Binding("Footer"));

				return label;
			});

			cv.ItemTemplate = new DataTemplate(() => {
				var label = new Label();

				label.SetBinding(Label.TextProperty, new Binding("Name"));

				return label;
			});

			cv.ItemsSource = _source;
			cv.IsGrouped = true;

			var layout = new StackLayout();
			layout.Children.Add(cv);

			Content = layout;

			Appearing += Issue7393Appearing;	
		}

		async void Issue7393Appearing(object sender, EventArgs e)
		{
			await AddItems();	
		}

		async Task AddItems() 
		{
			await Task.Delay(1000);

			var groupIndex = _source.Count + 1;

			var group = new _7393Group { Header = $"{groupIndex} Header (added)", Footer = $"{groupIndex} Footer (added)" };

			for (int itemIndex = 0; itemIndex < 3; itemIndex++)
			{
				var item = new _7393Item { Name = $"{groupIndex}.{itemIndex} Item (added)" };
				group.Add(item);
			}

			_source.Add(group);
		}

		class _7393Item 
		{
			public string Name { get; set; }
		}

		class _7393Group : ObservableCollection<_7393Item> 
		{ 
			public string Header { get; set; }
			public string Footer { get; set; }
		}

#if UITEST
		[Test]
		public void AddingItemsToGroupedCollectionViewShouldNotCrash()
		{
			
		}
#endif
	}
}
