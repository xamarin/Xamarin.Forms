using System;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7754, "Change navigation bar color",
		PlatformAffected.All)]
	public class Issue7754 : TestNavigationPage
	{
		Color _color = Color.Blue;
		const string buttonTitle = "Change NavBar Color";
		Color _navBarColor = Color.Default;

		protected override void Init()
		{
			this.Title = "Test";
			var layout = new StackLayout {VerticalOptions = LayoutOptions.Center };
			var button = new Button
			{
				Text = buttonTitle
			};
			layout.Children.Add(button);

			var page = new ContentPage { Content = layout };
			button.Clicked += (_, __) =>
			{

				_color = (_color == Color.Blue) ? Color.Fuchsia : Color.Blue;

				NavigationPage.SetBackgroundTitleView(page, _color);
			};

			PushAsync(page);
		}
	}
}
