using System;

namespace Xamarin.Forms.Xaml
{
	[AcceptEmptyServiceProvider]
	[ContentProperty(nameof(Glyph))]
	public class FontImageExtension : BindableObject, IMarkupExtension<ImageSource>
	{
		public static BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(FontImageExtension), Color.Default);

		public string FontFamily { get; set; }
		public string Glyph { get; set; }
		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		[TypeConverter(typeof(FontSizeConverter))]
		public double Size { get; set; } = (double)FontImageSource.SizeProperty.DefaultValue;

		public ImageSource ProvideValue(IServiceProvider serviceProvider)
		{
			return new FontImageSource
			{
				FontFamily = FontFamily,
				Glyph = Glyph,
				Color = Color,
				Size = Size
			};
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<ImageSource>).ProvideValue(serviceProvider);
		}
	}
}