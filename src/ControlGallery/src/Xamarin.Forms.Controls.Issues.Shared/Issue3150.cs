﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3150, "IsClippedToBounds on (Fast Renderer) Frame not working", PlatformAffected.Android)]

	class Issue3150 : TestContentPage
	{
		protected override void Init()
		{
			const string buttonText = "Toggle IsClippedToBounds: ";
			var frame = new Frame
			{
				BackgroundColor = Color.Blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				IsClippedToBounds = false,
				Content = new BoxView
				{
					BackgroundColor = Color.Yellow,
					TranslationX = 50
				}
			};

			Button button = null;
			button = new Button()
			{
				Text = $"{buttonText}{frame.IsClippedToBounds}",
				Command = new Command(() =>
				{
					frame.IsClippedToBounds = !frame.IsClippedToBounds;
					button.Text = $"{buttonText}{frame.IsClippedToBounds}";
				})
			};

			Content = new StackLayout
			{
				Children = {
					new Label { Text = "If the yellow box extends past the end of the blue box, the test has passed" },
					frame,
					button
				}
			};
		}
	}
}
