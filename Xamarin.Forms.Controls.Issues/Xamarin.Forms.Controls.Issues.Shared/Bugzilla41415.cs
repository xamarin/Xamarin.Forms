using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 41415, "ScrollX and ScrollY values are not consistent with iOS", PlatformAffected.Android)]
	public class Bugzilla41415 : TestContentPage
	{
		const string ButtonText = "Click Me";
		float _x = 0;
		float _y = 0;

		protected override void Init()
		{
			var grid = new Grid
			{
				BackgroundColor = Color.Yellow,
				WidthRequest = 1000,
				HeightRequest = 1000,
				Children =
				{
					new BoxView
					{
						WidthRequest =  200,
						HeightRequest = 200,
						BackgroundColor = Color.Red,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center
					}
				}
			};

			var labelx = new Label();
			var labely = new Label();

			var scrollView = new ScrollView
			{
				Orientation = ScrollOrientation.Both,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			scrollView.Content = grid;
			scrollView.Scrolled += (sender, args) =>
			{
				labelx.Text = $"x: {args.ScrollX}";
				labely.Text = $"y: {args.ScrollY}";
			};

			var button = new Button { Text = ButtonText };
			button.Clicked += async (sender, e) =>
			{
				await scrollView.ScrollToAsync(_x + 100, _y + 100, true);
				_x = 100;
			};

			Content = new StackLayout { Children = { button, labelx, labely, scrollView } };
		}

#if UITEST

		[Test]
		public void Bugzilla41415Test()
		{
			RunningApp.WaitForElement(q => q.Marked(ButtonText));
			RunningApp.Tap(q => q.Marked(ButtonText));
			RunningApp.WaitForElement(q => q.Marked("x: 100"));
			RunningApp.WaitForElement(q => q.Marked("y: 100"));
			RunningApp.Tap(q => q.Marked(ButtonText));
			RunningApp.WaitForElement(q => q.Marked("x: 200"));
			RunningApp.WaitForElement(q => q.Marked("y: 100"));
		}

#endif
	}
}