using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Label)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11829,
		"[Bug] TextDecoration Strikethrough not working on iOS together with LineHeight",
		PlatformAffected.All)]
	public class Issue11829 : TestContentPage
	{
		public Issue11829()
		{

		}

		protected override void Init()
		{
			Title = "Issue 11829";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If the text below is underline & strike through, the test has passed."
			};

			var label = new Label
			{
				TextDecorations = TextDecorations.Underline | TextDecorations.Strikethrough,
				LineHeight = 2,
				Text = "Underline using LineHeight",
				Margin = new Thickness(0, 12)
			};

			var textDecorationsLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal
			};

			var textDecorationsCheckBox = new CheckBox
			{
				IsChecked = true,
				VerticalOptions = LayoutOptions.Center
			};

			var textDecorationsText = new Label
			{
				Text = "Underline",
				WidthRequest = 100,
				VerticalOptions = LayoutOptions.Center
			};

			textDecorationsLayout.Children.Add(textDecorationsText);
			textDecorationsLayout.Children.Add(textDecorationsCheckBox);

			var lineHeightLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal
			};

			var lineHeightSlider = new Slider
			{
				Maximum = 4,
				Minimum = 2,
				Value = 2,
				WidthRequest = 150,
				VerticalOptions = LayoutOptions.Center
			};

			var lineHeightText = new Label
			{
				Text = "LineHeight",
				WidthRequest = 100,
				VerticalOptions = LayoutOptions.Center
			};

			lineHeightLayout.Children.Add(lineHeightText);
			lineHeightLayout.Children.Add(lineHeightSlider);

			layout.Children.Add(instructions);
			layout.Children.Add(label);
			layout.Children.Add(textDecorationsLayout);
			layout.Children.Add(lineHeightLayout);

			Content = layout;

			textDecorationsCheckBox.CheckedChanged += (sender, args) =>
			{
				if (args.Value)
					label.TextDecorations = TextDecorations.Strikethrough | TextDecorations.Underline;
				else
					label.TextDecorations = TextDecorations.None;
			};

			lineHeightSlider.ValueChanged += (sender, args) =>
			{
				label.LineHeight = args.NewValue;
			};
		}
	}
}
