using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	static class BarElement
	{
		public static readonly BindableProperty BarBackgroundColorProperty =
			BindableProperty.Create(nameof(IBarElement.BarBackgroundColor), typeof(Color), typeof(IBarElement), default(Color));

		public static readonly BindableProperty BarBackgroundProperty =
			BindableProperty.Create(nameof(IBarElement.BarBackground), typeof(Brush), typeof(IBarElement), default(Brush));

		public static readonly BindableProperty BarTextColorProperty =
			BindableProperty.Create(nameof(IBarElement.BarTextColor), typeof(Color), typeof(IBarElement), default(Color));

		public static readonly BindableProperty BarFontFamilyProperty = 
			BindableProperty.Create(nameof(IBarElement.BarFontFamily), typeof(string), typeof(IBarElement), default(string));
		
		public static readonly BindableProperty BarFontSizeProperty =
			BindableProperty.Create(nameof(IBarElement.BarFontSize), typeof(double), typeof(IBarElement), Font.Default.FontSize, defaultValueCreator: FontSizeDefaultValueCreator);

		public static readonly BindableProperty BarFontAttributesProperty =
			BindableProperty.Create(nameof(IBarElement.BarFontAttributes), typeof(FontAttributes), typeof(IBarElement), FontAttributes.None);

		static object FontSizeDefaultValueCreator(BindableObject bindable)
			=> Device.GetNamedSize(NamedSize.Default, (Element)bindable);
	}
}
