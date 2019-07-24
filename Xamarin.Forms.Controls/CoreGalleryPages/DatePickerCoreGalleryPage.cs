using System;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls
{
	internal class DatePickerCoreGalleryPage : CoreGalleryPage<DatePicker>
	{
		protected override bool SupportsTapGestureRecognizer => false;

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);

			var dateContainer = new ViewContainer<DatePicker>(Test.DatePicker.Date,
#pragma warning disable 0618 // Retain until Date is removed
				new DatePicker { Date = new DateTime(1987, 9, 13) });
#pragma warning restore
			var dateSelectedContainer = new EventViewContainer<DatePicker>(Test.DatePicker.DateSelected, new DatePicker());

#pragma warning disable 0618 // Retain until Date is removed
			dateSelectedContainer.View.DateSelected += (sender, args) => dateSelectedContainer.EventFired();
#pragma warning restore

			var selectedDateContainer = new ViewContainer<DatePicker>(Test.DatePicker.SelectedDate,
				new DatePicker { SelectedDate = new DateTime(1980, 11, 1) });
			var selectedDateChangedContainer = new EventViewContainer<DatePicker>(Test.DatePicker.SelectedDateChanged, new DatePicker());
			selectedDateChangedContainer.View.SelectedDateChanged += (sender, args) => selectedDateChangedContainer.EventFired();

			var formatDateContainer = new ViewContainer<DatePicker>(Test.DatePicker.Format, new DatePicker { Format = "ddd" });
			var minimumDateContainer = new ViewContainer<DatePicker>(Test.DatePicker.MinimumDate,
				new DatePicker { MinimumDate = new DateTime(1987, 9, 13) });
			var maximumDateContainer = new ViewContainer<DatePicker>(Test.DatePicker.MaximumDate,
				new DatePicker { MaximumDate = new DateTime(2087, 9, 13) });
#pragma warning disable 0618 // Retain until Date is removed
			var textColorContainer = new ViewContainer<DatePicker>(Test.DatePicker.TextColor,
				new DatePicker { Date = new DateTime(1978, 12, 24), TextColor = Color.Lime });
#pragma warning restore
			var fontAttributesContainer = new ViewContainer<DatePicker>(Test.DatePicker.FontAttributes,
				new DatePicker { FontAttributes = FontAttributes.Bold });

			var fontFamilyContainer = new ViewContainer<DatePicker>(Test.DatePicker.FontFamily,
				new DatePicker());
			// Set font family based on available fonts per platform
			switch(Device.RuntimePlatform)
			{
				case Device.Android:
					fontFamilyContainer.View.FontFamily = "sans-serif-thin";
					break;
				case Device.iOS:
					fontFamilyContainer.View.FontFamily = "Courier";
					break;
				default:
					fontFamilyContainer.View.FontFamily = "Garamond";
					break;
			}

			var fontSizeContainer = new ViewContainer<DatePicker>(Test.DatePicker.FontSize, 
				new DatePicker { FontSize = 24 });

			Add(dateContainer);
			Add(dateSelectedContainer);
			Add(selectedDateContainer);
			Add(selectedDateChangedContainer);
			Add(formatDateContainer);
			Add(minimumDateContainer);
			Add(maximumDateContainer);
			Add(textColorContainer);
			Add(fontAttributesContainer);
			Add(fontFamilyContainer);
			Add(fontSizeContainer);
		}
	}
}