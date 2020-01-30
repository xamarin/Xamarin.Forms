﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6474, "Border and CornerRadius are not respected in UWP when button is disabled", PlatformAffected.UWP)]
	public class Issue6474 : TestContentPage
	{
		protected override void Init()
		{
			var stackLayout = new StackLayout() { Margin = 25 };
			stackLayout.Children.Add(new Label { Text = "Visually verify that all buttons have a corner radius and that no background color extends beyond the border." });
			stackLayout.Children.Add(new Button { CornerRadius = 10, BorderColor = Color.Red, Text = "Enabled" });
			stackLayout.Children.Add(new Button { CornerRadius = 10, BorderColor = Color.Red, Text = "Disabled", IsEnabled = false });
			Content = stackLayout;
		}
	}
}
