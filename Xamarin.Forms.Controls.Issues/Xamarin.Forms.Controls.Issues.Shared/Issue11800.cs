﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Brush)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11800,
		"[Bug] LinearGradientBrush doesn't fill the pages upon orientation change on iOS",
		PlatformAffected.iOS)]
	public class Issue11800 : TestContentPage
	{
		public Issue11800()
		{

		}

		protected override void Init()
		{
			var background = new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0),
				GradientStops = new GradientStopCollection
				{
					new GradientStop { Color = Color.Red, Offset = 0.1f },
					new GradientStop { Color = Color.Yellow, Offset = 0.9f }
				}
			};

			Background = background;

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Rotate the emulator or device. If the gradient takes all the available space, the test has passed."
			};

			layout.Children.Add(instructions);

			Content = layout;
		}
	}
}