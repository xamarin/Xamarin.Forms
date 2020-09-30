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
	[Category(UITestCategories.DatePicker)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10909, "[Bug] UWP DatePicker and TimePicker Focus() function does not open the popup to set the date/time",
		PlatformAffected.UWP)]
	public class Issue10909 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Issue 10909";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				Text = "If pressing the buttons opens the popup of both DatePicker and TimePicker, the test has passed.",
				BackgroundColor = Color.Black,
				TextColor = Color.White
			};

			var datePickerFocusButton = new Button
			{
				Text = "Set focus on DatePicker"
			};

			var datepicker = new DatePicker();

			var timePickerFocusButton = new Button
			{
				Text = "Set focus on TimePicker"
			};

			var timePicker = new TimePicker();

			layout.Children.Add(instructions);
			layout.Children.Add(datePickerFocusButton);
			layout.Children.Add(datepicker);
			layout.Children.Add(timePickerFocusButton);
			layout.Children.Add(timePicker);

			Content = layout;

			datePickerFocusButton.Clicked += (sender, args) =>
			{
				datepicker.Focus();
			};

			timePickerFocusButton.Clicked += (sender, args) =>
			{
				timePicker.Focus();
			};
		}
	}
}