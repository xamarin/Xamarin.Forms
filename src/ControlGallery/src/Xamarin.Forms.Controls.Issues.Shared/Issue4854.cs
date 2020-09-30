﻿using System;
using System.Linq.Expressions;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4854, "[macOS] Visual glitch when exiting the full screen with ScrollViewer", PlatformAffected.macOS)]
	public class Issue4854 : TestContentPage
	{

		protected override void Init()
		{
			var gMain = new Grid { BackgroundColor = Color.LightBlue };
			gMain.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			gMain.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			var label = new Label
			{
				Text = "Enter full screen and exit and see the artifacts on the screen.",
				FontSize = 14
			};
			var sl = new StackLayout { HorizontalOptions = LayoutOptions.Center, WidthRequest = 300, Padding = new Thickness(15) };
			sl.Children.Add(label);
			gMain.Children.Add(sl);

			var button = new Button { Text = "Test", BackgroundColor = Color.Gray, HorizontalOptions = LayoutOptions.Center };
			var g = new Grid { BackgroundColor = Color.LightGray, Padding = new Thickness(20) };
			g.Children.Add(button);
			Grid.SetRow(g, 1);
			gMain.Children.Add(g);

			Content = new ScrollView { Content = gMain, BackgroundColor = Color.LightGreen };
		}

	}
}
