using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_EditorRenderer))]
	public class Editor : InputView, IEditorController, IFontElement, IPlaceholderElement, ITextElement, IElementConfiguration<Editor>
	{
		public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(Editor), null, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue)
			=> OnTextChanged((Editor)bindable, (string)oldValue, (string)newValue));

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public static readonly BindableProperty PlaceholderProperty = PlaceholderElement.PlaceholderProperty;

		public static readonly BindableProperty PlaceholderColorProperty = PlaceholderElement.PlaceholderColorProperty;

		public static readonly BindableProperty IsTextPredictionEnabledProperty = BindableProperty.Create(nameof(IsTextPredictionEnabled), typeof(bool), typeof(Editor), true, BindingMode.Default);

		public static readonly BindableProperty AutoSizeProperty = BindableProperty.Create(nameof(AutoSize), typeof(EditorAutoSizeOption), typeof(Editor), defaultValue: EditorAutoSizeOption.Disabled, propertyChanged: (bindable, oldValue, newValue)
			=> ((Editor)bindable)?.InvalidateMeasure());

		public static readonly BindableProperty CrossPlatformOptionProperty = BindableProperty.Create(nameof(CrossPlatformOption), typeof(EditorCrossPlatformOption), typeof(Editor), null, BindingMode.TwoWay);

		readonly Lazy<PlatformConfigurationRegistry<Editor>> _platformConfigurationRegistry;

		public EditorAutoSizeOption AutoSize
		{
			get { return (EditorAutoSizeOption)GetValue(AutoSizeProperty); }
			set { SetValue(AutoSizeProperty, value); }
		}

		public EditorCrossPlatformOption CrossPlatformOption
		{
			get { return (EditorCrossPlatformOption)GetValue(CrossPlatformOptionProperty); }
			set { SetValue(CrossPlatformOptionProperty, value); }
		}

		public string Text
		{
			get {
				// Return the desired format to the application, regardless of the platform or existing content
				switch (CrossPlatformOption)
				{
					case EditorCrossPlatformOption.PreferNewline:
						return Text?.Replace("\r", "\n");
					case EditorCrossPlatformOption.PreferCrLf:
						return Text?.Replace("\n", "\r");
					default:
						return (string)GetValue(TextProperty);
				}
			}
			set
			{
				// Preserve current behaviour
				if (CrossPlatformOption == EditorCrossPlatformOption.Default)
				{
					SetValue(TextProperty, value);
					return;
				}

				// The value of CrossPlatformOption doesn't matter as we can convert incoming new lines into what the editor expects.
				if (Device.RuntimePlatform == Device.UWP)
					// On UWP the editor expects \n for new lines. 
					SetValue(TextProperty, value?.Replace("\r", "\n"));
				else
					// On other platforms it's \r
					SetValue(TextProperty, value?.Replace("\n", "\r"));
			}
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		public string Placeholder {
			get => (string)GetValue(PlaceholderElement.PlaceholderProperty);
			set => SetValue(PlaceholderElement.PlaceholderProperty, value);
		}

		public Color PlaceholderColor {
			get => (Color)GetValue(PlaceholderElement.PlaceholderColorProperty);
			set => SetValue(PlaceholderElement.PlaceholderColorProperty, value);
		}

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		public bool IsTextPredictionEnabled
		{
			get { return (bool)GetValue(IsTextPredictionEnabledProperty); }
			set { SetValue(IsTextPredictionEnabledProperty, value); }
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

		protected void UpdateAutoSizeOption()
		{
			if (AutoSize == EditorAutoSizeOption.TextChanges)
			{
				InvalidateMeasure();
			}
		}

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue)
		{
			UpdateAutoSizeOption();
		}

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue)
		{
			UpdateAutoSizeOption();
		}

		void IFontElement.OnFontChanged(Font oldValue, Font newValue)
		{
			UpdateAutoSizeOption();
		}

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, (Editor)this);

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue)
		{
			UpdateAutoSizeOption();
		}

		public event EventHandler Completed;

		public event EventHandler<TextChangedEventArgs> TextChanged;

		public Editor()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Editor>>(() => new PlatformConfigurationRegistry<Editor>(this));
		}

		public IPlatformElementConfiguration<T, Editor> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendCompleted()
			=> Completed?.Invoke(this, EventArgs.Empty);

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		private static void OnTextChanged(Editor bindable, string oldValue, string newValue)
		{
			bindable.TextChanged?.Invoke(bindable, new TextChangedEventArgs(oldValue, newValue));
			if (bindable.AutoSize == EditorAutoSizeOption.TextChanges)
			{
				bindable.InvalidateMeasure();
			}
		}
	}
}