using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7898, "Null Reference Exception thrown when set FontFamily for label in Xamarin Forms macOS", PlatformAffected.macOS)]
	public class Issue7898 : TestNavigationPage
	{
		protected override void Init()
		{
			//BackgroundColor = Color.Yellow;
			Navigation.PushAsync(new ContentPage
			{
				BackgroundColor = Color.Yellow,
				Content = new StackLayout()
				{
					Children = {
						new Button
						{
							HorizontalOptions = LayoutOptions.Start,
							Text = "push page",
							Command = new Command(async () => await Navigation.PushAsync(new Page2(),false))
						}}
				}
			});
		}
		class Page2:TabbedPage
		{
			public Page2()
			{
				this.BackgroundColor = Color.Red;
				this.Children.Add(new ContentPage
				{
					Title = "Tab1",
					BackgroundColor = Color.Blue,
				});
			}
		}
	}
}