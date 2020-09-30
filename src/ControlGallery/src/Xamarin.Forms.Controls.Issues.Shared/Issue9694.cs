﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9694, "Switch control OnColor property breaks application",
		PlatformAffected.UWP)]
#if UITEST
	[Category(UITestCategories.Switch)]
#endif
	public class Issue9694 : TestContentPage
	{
		readonly Color _customColor = Color.FromHex("#9999CCDD");
		readonly Color _newCustomColor = Color.Purple;

		public Issue9694()
		{
			Title = "Issue 9694";

			var layout = new StackLayout();

			var instructions = new Label
			{
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If there is no exception, the test has passed."
			};

			var defaultSwitch = new Switch();

			var onColorSwitch = new Switch
			{
				OnColor = Color.Orange
			};

			var switchLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal
			};

			var customSwitch = new Switch
			{
				OnColor = _customColor,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 0, 6, 0)
			};

			var updateButton = new Button
			{
				VerticalOptions = LayoutOptions.Center,
				Text = "Update Color"
			};

			var resetButton = new Button
			{
				VerticalOptions = LayoutOptions.Center,
				Text = "Reset Color"
			};

			switchLayout.Children.Add(customSwitch);
			switchLayout.Children.Add(updateButton);
			switchLayout.Children.Add(resetButton);

			layout.Children.Add(instructions);
			layout.Children.Add(defaultSwitch);
			layout.Children.Add(onColorSwitch);
			layout.Children.Add(switchLayout);

			Content = layout;

			updateButton.Clicked += (sender, args) =>
			{
				customSwitch.OnColor = _newCustomColor;
			};

			resetButton.Clicked += (sender, args) =>
			{
				customSwitch.OnColor = _customColor;
			};
		}

		protected override void Init()
		{

		}
	}
}