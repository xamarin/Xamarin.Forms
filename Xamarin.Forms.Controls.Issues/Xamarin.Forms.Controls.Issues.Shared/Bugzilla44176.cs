using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 44176, "InputTransparent fails if BackgroundColor not explicitly set on Android", PlatformAffected.Android)]
	public class Bugzilla44176 : TestContentPage
	{
		protected override void Init()
		{
			var result = new Label
			{
				Text = "Success"
			};

			var grid = new Grid
			{
				InputTransparent = false,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = "grid"
			};
			AddTapGesture(result, grid);

			var contentView = new ContentView
			{
				InputTransparent = false,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = "contentView"
			};
			AddTapGesture(result, contentView);

			var stackLayout = new StackLayout
			{
				InputTransparent = false,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = "stackLayout"
			};
			AddTapGesture(result, stackLayout);

			var parent = new StackLayout
			{
				Spacing = 10,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					result,
					grid,
					contentView,
					stackLayout
				}
			};

			Content = parent;
		}

		void AddTapGesture(Label result, View view)
		{
			var tapGestureRecognizer = new TapGestureRecognizer
			{
				Command = new Command(() => { result.Text = "Fail"; })
			};
			view.GestureRecognizers.Add(tapGestureRecognizer);
		}

#if UITEST
		[Test]
		public void Test()
		{
			RunningApp.WaitForElement(q => q.Marked("grid"));
			RunningApp.Tap(q => q.Marked("grid"));
			RunningApp.WaitForElement(q => q.Marked("Success"));

			RunningApp.WaitForElement(q => q.Marked("contentView"));
			RunningApp.Tap(q => q.Marked("contentView"));
			RunningApp.WaitForElement(q => q.Marked("Success"));

			RunningApp.WaitForElement(q => q.Marked("stackLayout"));
			RunningApp.Tap(q => q.Marked("stackLayout"));
			RunningApp.WaitForElement(q => q.Marked("Success"));
		}
#endif
	}
}