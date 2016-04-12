using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
	[ContentProperty("Text")]
	public sealed class Span : INotifyPropertyChanged, IFontElement
	{
		Color _backgroundColor;

		Font _font;
		FontAttributes _fontAttributes;
		string _fontFamily;
		double _fontSize;

		Color _foregroundColor;
		bool _inUpdate; // if we ever make this thread safe we need to move to a mutex

		string _text;

		public Span()
		{
			_fontFamily = null;
			_fontAttributes = FontAttributes.None;
			_fontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label), true);
			_font = Font.SystemFontOfSize(_fontSize);
		}

		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				if (_backgroundColor == value)
					return;
				_backgroundColor = value;
				OnPropertyChanged();
			}
		}

		[Obsolete("Please use the Font properties directly. Obsolete in 1.3.0")]
		public Font Font
		{
			get { return _font; }
			set
			{
				if (_font == value)
					return;
				_font = value;
				OnPropertyChanged();
				UpdateFontPropertiesFromStruct();
			}
		}

		public Color ForegroundColor
		{
			get { return _foregroundColor; }
			set
			{
				if (_foregroundColor == value)
					return;
				_foregroundColor = value;
				OnPropertyChanged();
			}
		}

		public string Text
		{
			get { return _text; }
			set
			{
				if (_text == value)
					return;
				_text = value;
				OnPropertyChanged();
			}
		}

		public FontAttributes FontAttributes
		{
			get { return _fontAttributes; }
			set
			{
				if (_fontAttributes == value)
					return;
				_fontAttributes = value;
				OnPropertyChanged();
				UpdateStructFromFontProperties();
			}
		}

		public string FontFamily
		{
			get { return _fontFamily; }
			set
			{
				if (_fontFamily == value)
					return;
				_fontFamily = value;
				OnPropertyChanged();
				UpdateStructFromFontProperties();
			}
		}

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return _fontSize; }
			set
			{
				if (_fontSize == value)
					return;
				_fontSize = value;
				OnPropertyChanged();
				UpdateStructFromFontProperties();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

#pragma warning disable 0618 // retain until Span.Font removed
		void UpdateFontPropertiesFromStruct()
		{
			if (_inUpdate)
				return;
			_inUpdate = true;

			if (Font == Font.Default)
			{
				FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label), true);
				FontFamily = null;
				FontAttributes = FontAttributes.None;
			}
			else
			{
				FontSize = Font.UseNamedSize ? Device.GetNamedSize(Font.NamedSize, typeof(Label), true) : Font.FontSize;
				FontFamily = Font.FontFamily;
				FontAttributes = Font.FontAttributes;
			}

			_inUpdate = false;
		}

		void UpdateStructFromFontProperties()
		{
			if (_inUpdate)
				return;
			_inUpdate = true;

			if (FontFamily != null)
			{
				Font = Font.OfSize(FontFamily, FontSize).WithAttributes(FontAttributes);
			}
			else
			{
				Font = Font.SystemFontOfSize(FontSize).WithAttributes(FontAttributes);
			}

			_inUpdate = false;
		}
	}
#pragma warning restore
}