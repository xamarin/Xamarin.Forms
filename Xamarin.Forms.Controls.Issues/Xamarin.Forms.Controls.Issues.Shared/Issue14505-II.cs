using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using NUnit.Framework;
using Xamarin.UITest;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "TabbedPage BarBackgroundColor not working on iOS 15", PlatformAffected.iOS)]
	public class Issue14505_II : TestTabbedPage
	{
		public Issue14505_II()
		{
			BarBackgroundColor = Color.Red;
			BarTextColor = Color.White;
			SelectedTabColor = Color.Green;
			UnselectedTabColor = Color.Blue;
		}

		protected override void Init()
		{
			Children.Add(new ContentPage() { Title = "Tab 1", Content = CreateTabContent() });
			Children.Add(new ContentPage() { Title = "Tab 2", Content = CreateTabContent() });
		}

		Grid CreateTabContent()
		{
			var layout = new Grid
			{
				Margin = 12
			};

			var instructions = new Label
			{
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If TabBar BarBackgroundColor is Red, the test has passed."
			};

			layout.Children.Add(instructions);

			return layout;
		}
	}
}
