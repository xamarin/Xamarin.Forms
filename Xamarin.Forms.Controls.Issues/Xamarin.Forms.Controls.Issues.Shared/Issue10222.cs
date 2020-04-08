using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10222, "[CollectionView] ObjectDisposedException if the page is closed during scrolling", PlatformAffected.iOS)]
	public class Issue10222 : TestNavigationPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Navigation.PushAsync(new ContentPage
			{
				Content = new Button
				{
					Text = "Go",
					Command = new Command(async () => await Navigation.PushAsync(new CarouselViewTestPage()))
				}
			});
		}

		class CarouselViewTestPage : ContentPage
		{
			CollectionView cv;
			public CarouselViewTestPage()
			{
				cv = new CollectionView
				{
					Margin = new Thickness(0,40),
					ItemTemplate = new DataTemplate(() =>
					{
						var label = new Label
						{
							HorizontalTextAlignment = TextAlignment.Center,
							Margin = new Thickness(0, 100)
						};
						label.SetBinding(Label.TextProperty, new Binding("."));
						return label;
					})
				};
				Content = cv;
				InitCV();
			}

			async void InitCV()
			{
				var items = new List<string>();
				for (int i = 0; i < 10; i++)
				{
					items.Add($"items{i}");
				}

				cv.ItemsSource = items;

				//give the cv time to draw the items
				await Task.Delay(1000);

				cv.ScrollTo(items.Count - 1);

				//give the cv time to scroll
				var rand = new Random();
				await Task.Delay(rand.Next(10, 200));

				await Navigation.PopAsync(false);

			}
		}

#if UITEST
		[Test]
		public void Issue10222Test() 
		{
			// Delete this and all other UITEST sections if there is no way to automate the test. Otherwise, be sure to rename the test and update the Category attribute on the class. Note that you can add multiple categories.
			RunningApp.Screenshot("I am at Issue1");
			RunningApp.WaitForElement(q => q.Marked("Issue1Label"));
			RunningApp.Screenshot("I see the Label");
		}
#endif
	}
}