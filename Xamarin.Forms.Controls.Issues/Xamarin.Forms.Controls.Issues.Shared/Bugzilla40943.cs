using System;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40943, "[UWP/8.1] Buttons are not respecting a BorderWidth of 0")]
	public class Bugzilla40943 : TestContentPage
	{
		protected override void Init()
		{
			Content = new StackLayout
			{
				Children =
				{
					new Label
					{
						Text = "An explicit BorderWidth of 0 should be respected on UWP and 8.1"
					},
					new Button
					{
						Text = "Button with BorderWidth of 0 and Lime BorderColor",
						FontSize = 10,
						BackgroundColor = Color.Gray,
						BorderColor = Color.Lime,
						BorderWidth = 0
					},
					new Button
					{
						Text = "Button without explicit BorderWidth and Default BorderColor",
						FontSize = 10
					},
					new Button
					{
						Text = "Button with BorderWidth of 5 and Blue BorderColor",
						FontSize = 10,
						BorderColor = Color.Blue,
						BorderWidth = 5
					}
				},
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

		}
	}
}
