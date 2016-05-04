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
    [Issue(IssueTracker.Bugzilla, 40485, "Entries with a set/custom text color are not the same color when disabled")]
    public class Bugzilla40485 : TestContentPage
    {
        protected override void Init()
        {
			var entrySwitch = new Switch();
			var boundEntry = new Entry
			{
				BindingContext = entrySwitch,
				Text = "Default Color Entry - Bound"
			};
			boundEntry.SetBinding(Entry.IsEnabledProperty, new Binding("IsToggled"));
			var boundEntrySetColor = new Entry
			{
				BindingContext = entrySwitch,
				Text = "Set Color Entry - Bound",
				TextColor = Color.Blue
			};
			boundEntrySetColor.SetBinding(Entry.IsEnabledProperty, new Binding("IsToggled"));
			Content = new StackLayout
			{ 
				Children =
				{
					new Entry
					{
						Text = "Default Color Entry - Enabled"
					},
					new Entry
					{
						IsEnabled = false,
						Text = "Default Color Entry - Disabled"
					},
					new Entry
					{
						Text = "Red Entry - Enabled",
						TextColor = Color.Red
					},
					new Entry
					{
						IsEnabled = false,
						Text = "Red Entry - Disabled",
						TextColor = Color.Red
					},
					new Entry
					{
						Text = "Green Entry - Disabled",
						TextColor = Color.Green
					},
					new Entry
					{
						IsEnabled = false,
						Text = "Green Entry - Disabled",
						TextColor = Color.Green
					},
					boundEntry,
					boundEntrySetColor,
					new Label
					{
						Text = "Toggle to enable/disable bound entries"
					},
					entrySwitch
				}
            };
        }
    }
}
