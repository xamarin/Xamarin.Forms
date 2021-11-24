using System;
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
	[Issue(IssueTracker.Github, 11954,
		"[Bug] iOS label can't un-set text decorations when updating text at the same time",
		PlatformAffected.iOS)]
	public class Issue11954 : TestContentPage
	{
		public Issue11954()
		{
			UpdateLabel();
		}

		private Label NiceLabel;

		protected override void Init()
		{
			Title = "Issue 11954";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If press the toggle button and the label enables and disables strikethrough correctly, the test has passed"
			};

			NiceLabel = new Label
			{
				FontSize = 16,
				Padding = new Thickness(30, 24, 30, 0),
				Text = "Default text"
			};

			var button = new Button
			{
				Text = "Toggle"
			};

			button.Clicked += Button_Clicked;

			layout.Children.Add(NiceLabel);
			layout.Children.Add(button);

			Content = layout;
		}

		private bool _isStrikethrough;

		private void Button_Clicked(object sender, EventArgs e)
		{
			// Toggle strike-through and update label
			_isStrikethrough = !_isStrikethrough;
			UpdateLabel();
		}

		private void UpdateLabel()
		{
			NiceLabel.TextDecorations = _isStrikethrough ? TextDecorations.Strikethrough : TextDecorations.None;

			// Comment out the next line and it starts working again
			NiceLabel.Text = "Is it strikethrough? " + _isStrikethrough.ToString();
		}

#if UITEST
		[Test]
		[Category(UITestCategories.ManualReview)]
		public void Issue11954Test()
		{
			RunningApp.Tap("Toggle");
			RunningApp.Screenshot("Label should have strikethrough");
			RunningApp.Tap("Toggle");
			RunningApp.Screenshot("Label should NOT have strikethrough");

			Assert.Inconclusive("For visual review only");
		}
#endif
	}
}
