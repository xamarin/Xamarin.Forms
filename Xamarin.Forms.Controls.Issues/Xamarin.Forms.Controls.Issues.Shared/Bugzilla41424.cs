﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 41424, "[Android] Clicking cancel on a DatePicker does not cause it to unfocus", PlatformAffected.Android)]
	public class Bugzilla41424 : TestContentPage
	{
		const string DatePicker = "DatePicker";

		protected override void Init()
		{
			var stepsTitleLabel = new Label() { Text = "Test steps:" };
			var step1Label = new Label() { Text = "• Click 'Click to focus DatePicker'" };
			var step2Label = new Label() { Text = "• Click 'Cancel' or back button" };
			var step3Label = new Label() { Text = "• Click 'Click to focus DatePicker'" };
			var step4Label = new Label() { Text = "• Check that date selector is appear" };
			var datePickerFocusStateLabel = new Label();
			var datePicker = new DatePicker
			{
				AutomationId = DatePicker
			};
			datePicker.Focused += (sender, args) => { datePickerFocusStateLabel.Text = "focused"; };

			var datePickerFocusButton = new Button
			{
				Text = "Click to focus DatePicker",
				Command = new Command(() => datePicker.Focus())
			};

			var getDatePickerFocusStateButton = new Button
			{
				Text = "Click to view focus state",
				Command = new Command(() =>
				{
					datePickerFocusStateLabel.Text = datePicker.IsFocused ? "focused" : "unfocused";
				})
			};

			Content = new StackLayout
			{
				Children =
				{
					stepsTitleLabel,
					step1Label,
					step2Label,
					step3Label,
					step4Label,
					datePicker,
					datePickerFocusButton,
					getDatePickerFocusStateButton,
					datePickerFocusStateLabel
				}
			};
		}

#if UITEST

#if __ANDROID__
		[Test]
		public void DatePickerCancelShouldUnfocus()
		{
			RunningApp.Tap(q => q.Marked(DatePicker));
			RunningApp.Back();

			RunningApp.WaitForElement(q => q.Marked("Click to view focus state"));
			RunningApp.Tap(q => q.Marked("Click to view focus state"));
			RunningApp.WaitForElement(q => q.Marked("unfocused"));

			RunningApp.Tap(q => q.Marked("Click to focus DatePicker"));
			RunningApp.Back();

			RunningApp.WaitForElement(q => q.Marked("Click to view focus state"));
			RunningApp.Tap(q => q.Marked("Click to view focus state"));
			RunningApp.WaitForElement(q => q.Marked("unfocused"));
		}
#endif

#endif
	}
}
