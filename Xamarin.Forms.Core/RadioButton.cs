using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;
using Xamarin.Forms.Shapes;

namespace Xamarin.Forms
{
	public class RadioButton : TemplatedView, IElementConfiguration<RadioButton>, ITextElement, IFontElement, IBorderElement
	{
		public const string IsCheckedVisualState = "IsChecked";

		// Template Parts
		TapGestureRecognizer _tapGestureRecognizer;
		Shape _normalEllipse;
		Shape _checkMark;
		static readonly Color RadioButtonCheckMarkThemeColor = ResolveThemeColor("RadioButtonCheckMarkThemeColor");
		static readonly Color RadioButtonThemeColor = ResolveThemeColor("RadioButtonThemeColor");
		static ControlTemplate s_defaultTemplate;

		readonly Lazy<PlatformConfigurationRegistry<RadioButton>> _platformConfigurationRegistry;

		public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

		public static readonly BindableProperty ContentProperty =
		  BindableProperty.Create(nameof(Content), typeof(object), typeof(RadioButton), null);

		public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(
			nameof(IsChecked), typeof(bool), typeof(RadioButton), false, 
			propertyChanged: (b, o, n) => ((RadioButton)b).OnIsCheckedPropertyChanged((bool)n), 
			defaultBindingMode: BindingMode.TwoWay);

		public static readonly BindableProperty GroupNameProperty = BindableProperty.Create(
			nameof(GroupName), typeof(string), typeof(RadioButton), null, 
			propertyChanged: (b, o, n) => ((RadioButton)b).OnGroupNamePropertyChanged((string)o, (string)n));

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;
		
		public static readonly BindableProperty CharacterSpacingProperty = TextElement.CharacterSpacingProperty;

		public static readonly BindableProperty TextTransformProperty = TextElement.TextTransformProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty BorderColorProperty = BorderElement.BorderColorProperty;

		public static readonly BindableProperty CornerRadiusProperty = BorderElement.CornerRadiusProperty;

		public static readonly BindableProperty BorderWidthProperty = BorderElement.BorderWidthProperty;

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		// Text is a convenience property for setting Content to a string
		public string Text
		{
			get => GetValue(ContentProperty)?.ToString();
			set => SetValue(ContentProperty, value);
		}

		public bool IsChecked
		{
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}

		public string GroupName
		{
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		public double CharacterSpacing
		{
			get { return (double)GetValue(CharacterSpacingProperty); }
			set { SetValue(CharacterSpacingProperty, value); }
		}

		public TextTransform TextTransform
		{
			get { return (TextTransform)GetValue(TextTransformProperty); }
			set { SetValue(TextTransformProperty, value); }
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

		public double BorderWidth
		{
			get { return (double)GetValue(BorderWidthProperty); }
			set { SetValue(BorderWidthProperty, value); }
		}

		public Color BorderColor
		{
			get { return (Color)GetValue(BorderColorProperty); }
			set { SetValue(BorderColorProperty, value); }
		}

		public int CornerRadius
		{
			get { return (int)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		public RadioButton()
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(RadioButton), ExperimentalFlags.RadioButtonExperimental, nameof(RadioButton));

			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<RadioButton>>(() => 
				new PlatformConfigurationRegistry<RadioButton>(this));
		}

		public IPlatformElementConfiguration<T, RadioButton> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		void ITextElement.OnTextTransformChanged(TextTransform oldValue, TextTransform newValue)
			=> InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
			// TODO ezhart Probably need to apply this to the content if we're using templates
			// same for these other text property changes
		}

		void ITextElement.OnCharacterSpacingPropertyChanged(double oldValue, double newValue)
			=> InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontChanged(Font oldValue, Font newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, this);


		public virtual string UpdateFormsText(string source, TextTransform textTransform)
			=> TextTransformUtilites.GetTransformedText(source, textTransform);

		int IBorderElement.CornerRadiusDefaultValue => (int)BorderElement.CornerRadiusProperty.DefaultValue;

		Color IBorderElement.BorderColorDefaultValue => (Color)BorderElement.BorderColorProperty.DefaultValue;

		double IBorderElement.BorderWidthDefaultValue => (double)BorderElement.BorderWidthProperty.DefaultValue;

		void IBorderElement.OnBorderColorPropertyChanged(Color oldValue, Color newValue)
		{
			// TODO ezhart This probably need to be applied to the content
		}

		bool IBorderElement.IsCornerRadiusSet() => IsSet(BorderElement.CornerRadiusProperty);
		bool IBorderElement.IsBackgroundColorSet() => IsSet(BackgroundColorProperty);
		bool IBorderElement.IsBorderColorSet() => IsSet(BorderElement.BorderColorProperty);
		bool IBorderElement.IsBorderWidthSet() => IsSet(BorderElement.BorderWidthProperty);

		protected internal override void ChangeVisualState()
		{
			if (IsEnabled && IsChecked)
				VisualStateManager.GoToState(this, IsCheckedVisualState);
			else
				base.ChangeVisualState();
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			if (UsingRenderer)
			{
				return Device.PlatformServices.GetNativeSize(this, widthConstraint, heightConstraint);
			}

			return base.OnMeasure(widthConstraint, heightConstraint);
		}

		protected override void OnParentSet()
		{
			base.OnParentSet();

			if (ControlTemplate == null)
			{
				var rendererType = Internals.Registrar.Registered.GetHandlerType(typeof(RadioButton));
				if (rendererType == null)
				{
					ControlTemplate = DefaultTemplate;
					UpdateIsEnabled();
				}
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_normalEllipse = GetTemplateChild("NormalEllipse") as Shape;
			_checkMark = GetTemplateChild("CheckMark") as Shape;
		}

		bool UsingRenderer => ControlTemplate == null;

		void UpdateIsEnabled()
		{
			if (UsingRenderer)
			{
				return;
			}

			if (_tapGestureRecognizer == null)
			{
				_tapGestureRecognizer = new TapGestureRecognizer();
			}

			if (IsEnabled)
			{
				_tapGestureRecognizer.Tapped += SelectRadioButton;
				GestureRecognizers.Add(_tapGestureRecognizer);
			}
			else
			{
				_tapGestureRecognizer.Tapped -= SelectRadioButton;
				GestureRecognizers.Remove(_tapGestureRecognizer);
			}
		}

		static Color ResolveThemeColor(string key) 
		{
			try
			{
				return (Color)Application.Current.Resources[key];
			}
			catch 
			{ 
				// TODO ezhart check for reasonable exceptions here (missing key, key isn't actually a color)
				// and maybe log them, re-throw for everything else

				// I'm not entirely convinced this is the right way to handle these colors.
			}

			// TODO ezhart We need to make this return the appropriate default color based on the mode
			// (obviously black isn't a good choice in dark mode)
			return Color.Black;
		}

		void SelectRadioButton(object sender, EventArgs e)
		{
			if (IsEnabled)
			{
				IsChecked = true;
			}
		}

		void UpdateDisplay()
		{
			if (UsingRenderer)
			{
				return;
			}

			if (IsChecked)
			{
				if (_normalEllipse != null)
					_normalEllipse.Stroke = RadioButtonCheckMarkThemeColor;

				if (_checkMark != null)
					_checkMark.Opacity = 1;
			}
			else
			{
				if (_normalEllipse != null)
					_normalEllipse.Stroke = RadioButtonThemeColor;

				if (_checkMark != null)
					_checkMark.Opacity = 0;
			}
		}

		public static ControlTemplate DefaultTemplate
		{
			get 
			{
				if (s_defaultTemplate == null)
				{
					s_defaultTemplate = new ControlTemplate(() => BuildDefaultTemplate());
				}

				return s_defaultTemplate;
			}
		}

		void OnIsCheckedPropertyChanged(bool isChecked)
		{
			if (isChecked)
				RadioButtonGroup.UpdateRadioButtonGroup(this);

			UpdateDisplay();
			ChangeVisualState();
			CheckedChanged?.Invoke(this, new CheckedChangedEventArgs(isChecked));
		}

		void OnGroupNamePropertyChanged(string oldGroupName, string newGroupName)
		{
			// Unregister the old group name if set
			if (!string.IsNullOrEmpty(oldGroupName))
				RadioButtonGroup.Unregister(this, oldGroupName);

			// Register the new group name is set
			if (!string.IsNullOrEmpty(newGroupName))
				RadioButtonGroup.Register(this, newGroupName);
		}

		static View BuildDefaultTemplate()
		{
			var frame = new Frame
			{
				HasShadow = false,
				BackgroundColor = Color.Transparent,
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Start,
				Margin = new Thickness(6),
				Padding = new Thickness(0)
			};

			var grid = new Grid
			{
				RowSpacing = 0,
				ColumnDefinitions = new ColumnDefinitionCollection {
					new ColumnDefinition { Width = GridLength.Star },
					new ColumnDefinition { Width = GridLength.Auto }
				}
			};

			var normalEllipse = new Ellipse
			{
				Fill = Color.Transparent,
				Aspect = Stretch.Uniform,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				HeightRequest = 21,
				WidthRequest = 21,
				StrokeThickness = 2,
				Stroke = RadioButtonThemeColor,
				InputTransparent = true
			};

			var checkMark = new Ellipse
			{
				Fill = RadioButtonCheckMarkThemeColor,
				Aspect = Stretch.Uniform,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				HeightRequest = 11,
				WidthRequest = 11,
				Opacity = 0,
				Margin = new Thickness(1, 1, 0, 0),
				InputTransparent = true
			};

			var contentPresenter = new ContentPresenter
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			contentPresenter.SetBinding(ContentPresenter.ContentProperty,
				new Binding("Content", source: RelativeBindingSource.TemplatedParent, converter: new ContentConverter()));
			contentPresenter.SetBinding(MarginProperty, new Binding("Padding", source: RelativeBindingSource.TemplatedParent));

			grid.Children.Add(normalEllipse);
			grid.Children.Add(checkMark);
			grid.Children.Add(contentPresenter, 1, 0);

			frame.Content = grid;

			INameScope nameScope = new NameScope();
			NameScope.SetNameScope(frame, nameScope);
			nameScope.RegisterName("NormalEllipse", normalEllipse);
			nameScope.RegisterName("CheckMark", checkMark);
			nameScope.RegisterName("ContentPresenter", contentPresenter);

			return frame;
		}
	}
}