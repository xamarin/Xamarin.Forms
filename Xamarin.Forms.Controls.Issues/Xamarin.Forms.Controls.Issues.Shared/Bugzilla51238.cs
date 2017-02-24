using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 51238, "Transparent Grid causes Java.Lang.IllegalStateException: Unable to create layer for Platform_DefaultRenderer", PlatformAffected.Android)]
	public class Bugzilla51238 : TestContentPage
	{
		protected override void Init()
		{
			var grid = new Grid();
			grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
			grid.RowDefinitions.Add(new RowDefinition() { Height = 50 });

			var transparentLayer = new Grid();
			transparentLayer.IsVisible = false;
			transparentLayer.BackgroundColor = Color.Lime;
			transparentLayer.Opacity = 0.5;

			Grid.SetRow(transparentLayer, 0);

			var button = new Button() { Text = "Tap Me!", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

			Grid.SetRow(button, 0);

			button.Clicked += (sender, args) =>
			{
				transparentLayer.IsVisible = !transparentLayer.IsVisible;
			};

			grid.Children.Add(transparentLayer);
			grid.Children.Add(button);

			Content = grid;
		}

#if UITEST
		[Test]
		public void Issue1Test()
		{
			RunningApp.WaitForElement("Tap Me!");
			RunningApp.Tap("Tap Me!"); // Crashes the app if the issue isn't fixed
			RunningApp.WaitForElement("Tap Me!");
		}
#endif
	}
}