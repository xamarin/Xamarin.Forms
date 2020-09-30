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
	[Category(UITestCategories.Frame)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11031,
		"[Bug] Regression in 4.7-pre4: Frames are broken",
		PlatformAffected.Android)]
	public class Issue11031 : TestContentPage
	{
		public Issue11031()
		{
		}

		protected override void Init()
		{
			Title = "Issue 11031";

			var layout = new StackLayout();

			var frame = new Frame
			{
				BackgroundColor = Color.Red,
				BorderColor = Color.Black,
				CornerRadius = 24,
				Margin = 12
			};

			var label = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				Text = "Issue 11031"
			};

			frame.Content = label;

			layout.Children.Add(frame);

			Content = layout;
		}
	}
}
