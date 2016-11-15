using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 38480, "How to disable cut,copy, paste options for textFields in Xamarin.forms", PlatformAffected.iOS)]
	public class Bugzilla38480 : TestContentPage
	{
		protected override void Init()
		{
			var scrollView = new ScrollView();
			var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Spacing = 15
			};

			var datePicker = new Xamarin.Forms.DatePicker
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var picker = new Xamarin.Forms.Picker
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			picker.Items.Add("item1");
			picker.Items.Add("item2");
			picker.Items.Add("item3");
			picker.Items.Add("item4");
			picker.Items.Add("item5");

			var timePicker = new Xamarin.Forms.TimePicker
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};


			var entry = new Xamarin.Forms.Entry
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var button = new Button
			{
				Text = "Change",
				Command = new Command(() => { Change(datePicker, picker, timePicker, entry); })
			};

			stackLayout.Children.Add(button);
			stackLayout.Children.Add(datePicker);
			stackLayout.Children.Add(picker);
			stackLayout.Children.Add(timePicker);
			stackLayout.Children.Add(entry);

			SetInitialValues(datePicker, picker, timePicker, entry);
			scrollView.Content = stackLayout;

			Content = scrollView;
		}

		void SetInitialValues(Xamarin.Forms.DatePicker datePicker, Xamarin.Forms.Picker picker, Xamarin.Forms.TimePicker timePicker, Xamarin.Forms.Entry entry)
		{
			datePicker.On<iOS>().SetDisabledSelectorActions(new List<SelectorAction>
			{
				SelectorAction.AddShortcut,
				SelectorAction.Cut,
				SelectorAction.Copy,
				SelectorAction.Define,
				SelectorAction.Delete,
				SelectorAction.Lookup,
				SelectorAction.Select,
				SelectorAction.SelectAll,
				SelectorAction.Replace,
				SelectorAction.Share,
				SelectorAction.Paste
			});

			picker.On<iOS>().SetDisabledSelectorActions(new List<SelectorAction>
			{
				SelectorAction.AddShortcut,
				SelectorAction.Cut,
				SelectorAction.Copy,
				SelectorAction.Define,
				SelectorAction.Delete,
				SelectorAction.Lookup,
				SelectorAction.Select,
				SelectorAction.SelectAll,
				SelectorAction.Replace,
				SelectorAction.Share,
				SelectorAction.Paste
			});

			timePicker.On<iOS>().SetDisabledSelectorActions(new List<SelectorAction>
			{
				SelectorAction.AddShortcut,
				SelectorAction.Cut,
				SelectorAction.Copy,
				SelectorAction.Define,
				SelectorAction.Delete,
				SelectorAction.Lookup,
				SelectorAction.Select,
				SelectorAction.SelectAll,
				SelectorAction.Replace,
				SelectorAction.Share,
				SelectorAction.Paste
			});

			entry.On<iOS>().SetDisabledSelectorActions(new List<SelectorAction>
			{
				SelectorAction.AddShortcut,
				SelectorAction.Cut,
				SelectorAction.Copy,
				SelectorAction.Define,
				SelectorAction.Delete,
				SelectorAction.Lookup,
				SelectorAction.Select,
				SelectorAction.SelectAll,
				SelectorAction.Replace,
				SelectorAction.Share,
				SelectorAction.Paste
			});
		}

		void Change(Xamarin.Forms.DatePicker datePicker, Xamarin.Forms.Picker picker, Xamarin.Forms.TimePicker timePicker, Xamarin.Forms.Entry entry)
		{
			datePicker.On<iOS>().SetDisabledSelectorActions(new List<SelectorAction>
			{
				SelectorAction.AddShortcut,
				SelectorAction.Cut,
				SelectorAction.Copy
			});

			picker.On<iOS>().SetDisabledSelectorActions(new List<SelectorAction>
			{
				SelectorAction.Cut,
				SelectorAction.Replace,
				SelectorAction.Share,
				SelectorAction.Paste
			});

			timePicker.On<iOS>().SetDisabledSelectorActions(new List<SelectorAction>
			{
				SelectorAction.Copy,
				SelectorAction.Define,
				SelectorAction.Delete,
				SelectorAction.Replace,
				SelectorAction.Share,
				SelectorAction.Paste
			});

			entry.On<iOS>().SetDisabledSelectorActions(new List<SelectorAction>
			{
				SelectorAction.AddShortcut,
				SelectorAction.Cut,
				SelectorAction.Lookup, // not the same as "Look Up" which happens to be Define on iOS 10.
				SelectorAction.Select,
				SelectorAction.SelectAll,
				SelectorAction.Share
			});
		}
	}
}