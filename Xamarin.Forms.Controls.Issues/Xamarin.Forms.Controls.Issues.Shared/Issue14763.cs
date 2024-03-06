using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14763, "[iOS] Resize on back-swipe stays when going back is cancelled", PlatformAffected.iOS)]
	public class Issue14763 : TestNavigationPage
	{
		public Issue14763() : base()
		{ }

		class HomePage : ContentPage
		{
			public HomePage()
			{
				Title = "Home";
				var grd = new Grid { BackgroundColor = Color.Brown };
				grd.RowDefinitions.Add(new RowDefinition());
				grd.RowDefinitions.Add(new RowDefinition());
				grd.RowDefinitions.Add(new RowDefinition());

				NavigationPage.SetHasNavigationBar(this, false);

				var boxView = new BoxView { BackgroundColor = Color.Blue };
				grd.Children.Add(boxView, 0, 0);
				var stackLayout = new StackLayout()
				{
					BackgroundColor = Color.Yellow,
					AutomationId = "ChildLayout"
				};
				var btnPop = new Button {
					Text = "Back (I get cut off)", AutomationId = "PopButtonId", Command = new Command(async () => await Navigation.PopAsync()),
					BackgroundColor = Color.Red
				};
				stackLayout.Children.Add(btnPop);
				btnPop.VerticalOptions = new LayoutOptions(LayoutAlignment.End, expands: true);
				var btn = new Button()
				{
					BackgroundColor = Color.Pink,
					Text = "NextButtonID",
					AutomationId = "NextButtonID",
					
					Command = new Command(async () =>
					{
						
						var page = new ContentPage
						{
							Title = "Detail",
							Content = stackLayout,
							AutomationId = "ChildPage"
						};

						NavigationPage.SetHasNavigationBar(page, true);
						await Navigation.PushAsync(page, false);
					})
				};

				grd.Children.Add(btn, 0, 1);
				var image = new Image() { Source = "coffee.png", AutomationId = "CoffeeImageId", BackgroundColor = Color.Yellow };
				image.VerticalOptions = LayoutOptions.End;
				grd.Children.Add(image, 0, 2);
				Content = grd;

			}
		}

		protected override void Init()
		{
			PushAsync(new HomePage());
		}

#if UITEST && __IOS__
		[Test]
		public async Task PopButtonStillVisibleAfterSwipeBack()
		{
			RunningApp.WaitForElement("NextButtonID");
			// Navigate to child page
			RunningApp.Tap("NextButtonID");
			var childLayout = RunningApp.WaitForElement("ChildLayout").First();
			RunningApp.WaitForElement("PopButtonId");
			await Task.Delay(1000);
			var origLayoutRect = childLayout.Rect;

			// Swipe-back/Swipe in from left across 1/2 of screen width to start back nav without completing it
			var screenBounds = RunningApp.ScreenBounds();
			RunningApp.DragCoordinates(0, screenBounds.CenterY, screenBounds.CenterX, screenBounds.CenterY);			
			// Wait for swipe-back and UI to settle
			await Task.Delay(3000);
			childLayout = RunningApp.WaitForElement("ChildLayout").First();
			var newLayoutRect = childLayout.Rect;

			// Assert that child page has same height as before
			Assert.That(newLayoutRect.Height, Is.EqualTo(origLayoutRect.Height));
		}
#endif
	}
}