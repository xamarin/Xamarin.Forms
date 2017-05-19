using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 618, "Transparent Overlays", PlatformAffected.All)]
	public class TransparentOverlayTests : TestContentPage
	{
		// Need a grid with two rows, controls in each, and an overlaying stacklayout
		protected override void Init()
		{
			var grid = new Grid
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill
			};
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

			var button1 = new Button
			{
				Text = "Button 1",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			button1.Clicked += (sender, args) => { Debug.WriteLine($">>>>> TransparentOverlayTests Init 81: Button1"); };
			grid.Children.Add(button1);
			Grid.SetRow(button1, 0);


			var button2 = new Button
			{
				Text = "Button 2",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			button2.Clicked += (sender, args) => { Debug.WriteLine($">>>>> TransparentOverlayTests Init 81: Button2"); };
			grid.Children.Add(button2);
			Grid.SetRow(button2, 1);

			var layout1 = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			grid.Children.Add(layout1);
			Grid.SetRow(layout1, 0);

			var layout2 = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Blue,
				Opacity = 0.2
			};

			grid.Children.Add(layout2);
			Grid.SetRow(layout2, 1);

			Content = grid;
		}
	}
}