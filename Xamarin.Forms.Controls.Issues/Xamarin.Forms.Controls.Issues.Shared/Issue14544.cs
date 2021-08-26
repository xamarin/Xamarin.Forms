﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.RadioButton)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14544, "RadioButton does not respond to taps after changing IsEnabled from false to true", PlatformAffected.iOS)]
	public class Issue14544 : TestContentPage 
	{
		StackLayout _layout;
		RadioButton _radioButton;
		Button _button;
		Label _statusLabel;

		const string RadioButtonID = "DisabledRadioButton";
		const string ToggleButtonID = "ToggleButton";
		const string StatusLabelID = "StatusLabel";

		const string CheckedLabel = "Checked";
		const string UncheckedLabel = "Unchecked";

		protected override void Init()
		{
			_statusLabel = new Label { Text = UncheckedLabel, AutomationId = StatusLabelID };

			_radioButton = new RadioButton
			{
				AutomationId = RadioButtonID,
				Content = "Disabled Radio Button",
				IsEnabled = false,
			};
			_radioButton.CheckedChanged += (s, e) => _statusLabel.Text = e.Value ? CheckedLabel : UncheckedLabel;

			_button = new Button()
			{
				AutomationId = ToggleButtonID,
				Text = "Enable Radio Button",
				Command = new Command(() => _radioButton.IsEnabled = true)
			};

			_button.Command.Execute(null);

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children =
				{
					_radioButton,
					_button,
					_statusLabel
				}
			};
		}

#if UITEST
		[Test]
		public void Issue14544Test()
		{
			RunningApp.WaitForElement(RadioButtonID);

			//Radio button should be unchecked initially
			var labelElem = RunningApp.Query(c => c.Marked(StatusLabelID))[0];
			Assert.AreEqual(UncheckedLabel, labelElem.Text);

			RunningApp.Tap(ToggleButtonID);
			RunningApp.Tap(RadioButtonID);

			labelElem = RunningApp.Query(c => c.Marked(StatusLabelID))[0];
			Assert.AreEqual(CheckedLabel, labelElem.Text);

			RunningApp.Screenshot("Checked RadioButton");
		}
#endif
	}
}