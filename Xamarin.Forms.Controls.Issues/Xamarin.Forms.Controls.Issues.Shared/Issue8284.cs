using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8284, "Cannot add/remove menuitems at runtime to contextActions of a ViewCell", PlatformAffected.Android)]
	public class Issue8284 : TestNavigationPage
	{
		protected override void Init()
		{
			var contentPage = new ContentPage();

			var retainElementButton = new Button() { Text = $"{nameof(ListViewCachingStrategy.RetainElement)}" };
			retainElementButton.Clicked += CreateHandler(GetListViewPage(ListViewCachingStrategy.RetainElement));

			var recycleElementButton = new Button() { Text = $"{nameof(ListViewCachingStrategy.RecycleElement)}" };
			recycleElementButton.Clicked += CreateHandler(GetListViewPage(ListViewCachingStrategy.RecycleElement));

			var recycleElementAndDataTemplateButton = new Button() { Text = $"{nameof(ListViewCachingStrategy.RecycleElementAndDataTemplate)}" };
			recycleElementAndDataTemplateButton.Clicked += CreateHandler(GetListViewPage(ListViewCachingStrategy.RecycleElementAndDataTemplate));

			contentPage.Content = new StackLayout
			{
				Children = {
					new Label { Text = $"All three buttons create a listview with a different {nameof(ListViewCachingStrategy)}" +
					                   $"Click all Buttons and verify that you can add/remove ContextActions",
						TextColor = Color.Red
					}, 
					retainElementButton,
					recycleElementButton,
					recycleElementAndDataTemplateButton,

				}
			};

			Navigation.PushAsync(contentPage);
		}

		EventHandler CreateHandler(ContentPage contentPage)
		{
			return async (sender, args) =>
			{
				try
				{
					await Navigation.PushAsync(contentPage);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw;
				}
			};
		}

		static ContentPage GetListViewPage(ListViewCachingStrategy strategy)
		{
			var listView = new ListView(strategy);
			listView.ItemTemplate = new DataTemplate(typeof(CustomViewCell));

			var source = new ObservableCollection<string>();
			listView.ItemsSource = source;
			Array.ForEach(Enumerable.Range(0, 3).Select(i => i.ToString()).ToArray(), item => source.Add(item));

			var contentPage = new ContentPage();

			contentPage.Content = new StackLayout
			{
				Children = {
					new Label{Text=$"ListViewCachingStrategy is {strategy}", TextColor = Color.Red },
					new Label { Text = $"Press 'Add' or 'Remove' and check if the 'ContextActions' are being added." +
					                   $"Verify that by long-pressing an item in the {nameof(ListView)}" +
					                   $"The {nameof(ListView)} uses {nameof(ListViewCachingStrategy.RecycleElement)}"},
					listView
				}
			};

			return contentPage;
		}

		class CustomViewCell : ViewCell
		{
			public CustomViewCell()
			{
				var view = new StackLayout();
				View = view;
				view.Orientation = StackOrientation.Horizontal;
				view.Children.Add(new Label() { Text = "Add or remove!" });

				var menuItem = new MenuItem() { Text = "Item" };

				var addButton = new Button { Text = "Add" };
				addButton.Clicked += (sender, args) =>
				{
					ContextActions.Add(menuItem);
				};

				var removeButton = new Button { Text = "Remove" };
				removeButton.Clicked += (sender, args) =>
				{
					if (ContextActions.Count != 0)
						ContextActions.RemoveAt(0);
				};

				view.Children.Add(addButton);
				view.Children.Add(removeButton);

				ContextActions.Add(menuItem);
			}
		}

	}
}