using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 33559, "Scaled image escapes the bounds of its container", PlatformAffected.Android)]
	public class Bugzilla33559 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		public Image Image = new Image { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, Source = "icon.png", Opacity = .85 };

		protected override void Init()
		{
			var top = new StackLayout
			{
				BackgroundColor = Color.Green,
				Opacity = .85,
				Children = {
					new Label { Text = "Test\nLabel", HorizontalTextAlignment = TextAlignment.Center }
				}
			};

			var middle = new StackLayout
			{
				BackgroundColor = Color.Teal,
				VerticalOptions = LayoutOptions.FillAndExpand,
				IsClippedToBounds = true
			};
			middle.Children.Add(Image);

			var bottom = new StackLayout
			{
				BackgroundColor = Color.Blue,
				VerticalOptions = LayoutOptions.End,
				Opacity = .85,
				Children = {
					new Button { Text = "Scale", Command = new Command(() => { Image.ScaleTo(Image.Scale == 1 ? 20 : 1, 750); }) }
				}
			};

			Content = new StackLayout
			{
				IsClippedToBounds = true,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = 20,
				Spacing = 20,
				Children =
				{
					top,
					middle,
					bottom
				}
			};
		}
	}
}