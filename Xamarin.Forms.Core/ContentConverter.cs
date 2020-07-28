using System;
using System.Globalization;

namespace Xamarin.Forms
{
	internal class ContentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is View view)
			{
				return view;
			}

			if (value is string textContent)
			{
				var label = new Label
				{
					Text = textContent
				};

				label.SetBinding(Label.TextColorProperty, 
					new Binding("TextColor", source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestor, typeof(ITextElement))));

				return label;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}