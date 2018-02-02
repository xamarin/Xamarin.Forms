using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	[ContentProperty("Text")]
	public sealed class Span : Element, IFontElement
	{
		internal readonly MergedStyle _mergedStyle;

		public Span()
		{
			_mergedStyle = new MergedStyle(GetType(), this);
		}

		public static readonly BindableProperty StyleProperty = BindableProperty.Create("Style", typeof(Style), typeof(Span), default(Style),
			propertyChanged: (bindable, oldvalue, newvalue) => ((Span)bindable)._mergedStyle.Style = (Style)newvalue);

		public Style Style
		{
			get { return (Style)GetValue(StyleProperty); }
			set { SetValue(StyleProperty, value); }
		}

		public static readonly BindableProperty BackgroundColorProperty
			= BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(Span), default(Color));

		public Color BackgroundColor
		{
			get { return (Color)GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}

		[Obsolete("Foreground is obsolete as of version 2.6.0. Please use the TextColor property instead.")]
		public static readonly BindableProperty ForegroundColorProperty
			= BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(Span), default(Color), propertyChanged: ForegroundColorOnPropertyChanged);

		static void ForegroundColorOnPropertyChanged(object bindable, object oldvalue, object newvalue)
		{
			((Span)bindable).TextColor = newvalue as Color? ?? default(Color);
		}

#pragma warning disable 618
		public Color ForegroundColor
		{
			get { return (Color)GetValue(ForegroundColorProperty); }
			set { SetValue(ForegroundColorProperty, value); }
		}
#pragma warning restore 618

		public static readonly BindableProperty TextColorProperty
			= BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(Span), default(Color));

		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		public static readonly BindableProperty TextProperty
			= BindableProperty.Create(nameof(Text), typeof(string), typeof(Span), "");

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set	{ SetValue(TextProperty, value); }
		}

		public static readonly BindableProperty FontProperty = FontElement.FontProperty;

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		[Obsolete("Font is obsolete as of version 1.3.0. Please use the Font properties directly.")]
		public Font Font
		{
			get { return (Font)GetValue(FontElement.FontProperty); }
			set { SetValue(FontElement.FontProperty, value); }
		}

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontElement.FontAttributesProperty); }
			set { SetValue(FontElement.FontAttributesProperty, value); }
		}

		public string FontFamily
		{
			get { return (string)GetValue(FontElement.FontFamilyProperty); }
			set { SetValue(FontElement.FontFamilyProperty, value); }
		}

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontElement.FontSizeProperty); }
			set { SetValue(FontElement.FontSizeProperty, value); }
		}

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue)
		{
		}

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue)
		{
		}

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, new Label());

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue)
		{
			
		}

		void IFontElement.OnFontChanged(Font oldValue, Font newValue)
		{
		}
	}
}