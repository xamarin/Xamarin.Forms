using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11963, "Time and Date Picker is broken in Xamarin.Forms as of iOS 14 Public Beta 6", PlatformAffected.iOS)]
	public class Issue11963 : TestContentPage
	{
		protected override void Init()
		{
			var timePicker = new TimePicker();
			var timePickerCompact = new TimePicker();
			var timePickerInline = new TimePicker();

			timePickerCompact.On<iOS>().SetUIPickerStyle(UIDatePickerStyle.Compact);
			timePickerInline.On<iOS>().SetUIPickerStyle(UIDatePickerStyle.Inline);

			var layout = new Grid
			{
				ColumnSpacing = 5,
				RowSpacing = 10,
				VerticalOptions = LayoutOptions.Center
			};

			layout.ColumnDefinitions.Add(new ColumnDefinition());
			layout.ColumnDefinitions.Add(new ColumnDefinition());
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			layout.Children.Add(new Label { Text = "None", VerticalOptions = LayoutOptions.Center }, 0, 0);
			layout.Children.Add(timePicker, 1, 0);

			layout.Children.Add(new Label { Text = nameof(UIDatePickerStyle.Compact), VerticalOptions = LayoutOptions.Center }, 0, 1);
			layout.Children.Add(timePickerCompact, 1, 1);

			layout.Children.Add(new Label { Text = nameof(UIDatePickerStyle.Inline), VerticalOptions = LayoutOptions.Center }, 0, 2);
			layout.Children.Add(timePickerInline, 1, 2);

			layout.Children.Add(new Label { Text = "DatePickers", VerticalOptions = LayoutOptions.Center }, 0, 3);

			var datePicker = new DatePicker();
			var datePickerCompact = new DatePicker();
			var ddatePickerInline = new DatePicker();

			datePickerCompact.On<iOS>().SetUIPickerStyle(UIDatePickerStyle.Compact);
			ddatePickerInline.On<iOS>().SetUIPickerStyle(UIDatePickerStyle.Inline);

			layout.Children.Add(new Label { Text = "None", VerticalOptions = LayoutOptions.Center }, 0, 4);
			layout.Children.Add(datePicker, 1, 4);

			layout.Children.Add(new Label { Text = nameof(UIDatePickerStyle.Compact), VerticalOptions = LayoutOptions.Center }, 0, 5);
			layout.Children.Add(datePickerCompact, 1, 5);

			layout.Children.Add(new Label { Text = nameof(UIDatePickerStyle.Inline), VerticalOptions = LayoutOptions.Center }, 0, 6);
			layout.Children.Add(ddatePickerInline, 1, 6);


			Content = layout;
		}
	}
}