﻿using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10578,
		"[Bug][iOS] NavigationPage.HideNavigationBarSeparator=true doesn't work from XAML",
		PlatformAffected.iOS)]
	public partial class Issue10578 : NavigationPage
	{
		public Issue10578()
		{
#if APP
			InitializeComponent();
			PushAsync(new Issue10578Instructions(this));
#endif
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue10578Instructions : ContentPage
	{
		NavigationPage _navigationPage;

		public Issue10578Instructions(NavigationPage navigationPage)
		{
			_navigationPage = navigationPage;

			Title = "Issue 10578";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				Margin = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If the NavigationBar Separator is hidden, the test has passed."
			};

			var showButton = new Button
			{
				Text = "Show separator"
			};

			showButton.Clicked += OnShowButtonClicked;

			var hideButton = new Button
			{
				Text = "Hide separator"
			};

			hideButton.Clicked += OnHideButtonClicked;

			layout.Children.Add(instructions);
			layout.Children.Add(showButton);
			layout.Children.Add(hideButton);

			Content = layout;
		}

		void OnHideButtonClicked(object sender, EventArgs e)
		{
			_navigationPage.On<iOS>().SetHideNavigationBarSeparator(true);
		}

		void OnShowButtonClicked(object sender, EventArgs e)
		{
			_navigationPage.On<iOS>().SetHideNavigationBarSeparator(false);
		}
	}
}