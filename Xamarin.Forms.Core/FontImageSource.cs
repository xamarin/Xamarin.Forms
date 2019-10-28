namespace Xamarin.Forms
{
	public class FontImageSource : ImageSource
	{
		public override bool IsEmpty => string.IsNullOrEmpty(Glyph);

		public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color", typeof(Color), typeof(FontImageSource), default(Color),
			propertyChanged: (b,o,n)=>((FontImageSource)b).OnSourceChanged());

		public Color Color {
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create("FontFamily", typeof(string), typeof(FontImageSource), default(string),
			propertyChanged: (b, o, n) => ((FontImageSource)b).OnSourceChanged());

		public string FontFamily {
			get => (string)GetValue(FontFamilyProperty);
			set => SetValue(FontFamilyProperty, value);
		}

		public static readonly BindableProperty GlyphProperty = BindableProperty.Create("Glyph", typeof(string), typeof(FontImageSource), default(string),
			propertyChanged: (b, o, n) => ((FontImageSource)b).OnSourceChanged());

		public string Glyph  {
			get => (string)GetValue(GlyphProperty);
			set => SetValue(GlyphProperty, value);
		}

		public static readonly BindableProperty SizeProperty = BindableProperty.Create("Size", typeof(double), typeof(FontImageSource), default(double),
			propertyChanged: (b, o, n) => ((FontImageSource)b).OnSourceChanged());

		[TypeConverter(typeof(FontSizeConverter))]
		public double Size { get => (double)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }
		public static readonly BindableProperty SizeProperty = CreateBindableProperty(nameof(Size), 30d);

		public string Glyph { get => (string)GetValue(GlyphProperty); set => SetValue(GlyphProperty, value); }
		public static readonly BindableProperty GlyphProperty = CreateBindableProperty<string>(nameof(Glyph));

		public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
		public static readonly BindableProperty ColorProperty = CreateBindableProperty<Color>(nameof(Color));

		public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }
		public static readonly BindableProperty FontFamilyProperty = CreateBindableProperty<string>(nameof(FontFamily));

		static BindableProperty[] BindableProperties = new[]
		{
			FontFamilyProperty,
			GlyphProperty,
			ColorProperty,
			SizeProperty,
		};
		protected override void OnPropertyChanged(string propertyName = null)
		{
			for (var i = 0; i < BindableProperties.Length; i++)
			{
				var bindableProperty = BindableProperties[i];
				if (propertyName == bindableProperty.PropertyName)
					OnSourceChanged();
			}
			
			base.OnPropertyChanged(propertyName);
		}
	}
}