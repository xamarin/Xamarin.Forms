using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_EntryRenderer))]
	public class Entry : InputView, IFontElement, IEntryController
	{
		public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create("Placeholder", typeof(string), typeof(Entry), default(string));

		public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create("IsPassword", typeof(bool), typeof(Entry), default(bool));

		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(Entry), null, BindingMode.TwoWay, propertyChanged: OnTextChanged);

		public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(Entry), Color.Default);

		public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create("HorizontalTextAlignment", typeof(TextAlignment), typeof(Entry), TextAlignment.Start);

		public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create("PlaceholderColor", typeof(Color), typeof(Entry), Color.Default);

		public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create("FontFamily", typeof(string), typeof(Entry), default(string));

		public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(Entry), -1.0,
			defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, (Entry)bindable));

		public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create("FontAttributes", typeof(FontAttributes), typeof(Entry), FontAttributes.None);

		public static new readonly BindableProperty IsFocusedProperty =
				BindableProperty.Create (nameof (IsFocused), typeof (bool), typeof (Entry), false, BindingMode.TwoWay, propertyChanged: OnIsFocusedChanged);

		public TextAlignment HorizontalTextAlignment
		{
			get { return (TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
			set { SetValue(HorizontalTextAlignmentProperty, value); }
		}

		public bool IsPassword
		{
			get { return (bool)GetValue(IsPasswordProperty); }
			set { SetValue(IsPasswordProperty, value); }
		}

		public string Placeholder
		{
			get { return (string)GetValue(PlaceholderProperty); }
			set { SetValue(PlaceholderProperty, value); }
		}

		public Color PlaceholderColor
		{
			get { return (Color)GetValue(PlaceholderColorProperty); }
			set { SetValue(PlaceholderColorProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		public string FontFamily
		{
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		public new bool IsFocused {
			get { return (bool)GetValue (IsFocusedProperty); }
			set { SetValue (IsFocusedProperty, value); }
		}

		public event EventHandler Completed;

		public event EventHandler<TextChangedEventArgs> TextChanged;

		void IEntryController.SendCompleted()
		{
			Completed?.Invoke(this, EventArgs.Empty);
		}

		static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var entry = (Entry)bindable;

			entry.TextChanged?.Invoke(entry, new TextChangedEventArgs((string)oldValue, (string)newValue));
		}

		public static void OnIsFocusedChanged (object sender, object oldValue, object newvalue)
		{
			OnIsFocusedChanged ((VisualElement)sender, (bool)oldValue, (bool)newvalue);
		}

		public static void OnIsFocusedChanged (VisualElement sender, bool oldValue, bool newValue)
		{
			if (newValue)
				sender.Focus ();
			sender.SetValue (IsFocusedProperty, false);
		}
	}
}