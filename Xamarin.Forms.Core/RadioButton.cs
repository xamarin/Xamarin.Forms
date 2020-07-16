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
				return new Label
				{
					Text = textContent
				};
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RadioButton : TemplatedView, IElementConfiguration<RadioButton>, ITextElement, IFontElement, IBorderElement
	{
		#region ControlTemplate Stuff

		TapGestureRecognizer _tapGestureRecognizer;
		Shape _normalEllipse;
		Shape _checkMark;
		static Color _radioButtonCheckMarkThemeColor = ResolveThemeColor("RadioButtonCheckMarkThemeColor");
		static Color _radioButtonThemeColor = ResolveThemeColor("RadioButtonThemeColor");

		void Initialize()
		{
			if (ControlTemplate == null)
			{
				return;
			}
			
			UpdateIsEnabled();
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

		void UpdateIsEnabled()
		{
			if (ControlTemplate == null)
			{
				return;
			}

			if (_tapGestureRecognizer == null)
			{
				_tapGestureRecognizer = new TapGestureRecognizer();
			}

			if (IsEnabled)
			{
				_tapGestureRecognizer.Tapped += OnRadioButtonChecked;
				GestureRecognizers.Add(_tapGestureRecognizer);
			}
			else
			{
				_tapGestureRecognizer.Tapped -= OnRadioButtonChecked;
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

		void OnRadioButtonChecked(object sender, EventArgs e)
		{
			IsChecked = !IsChecked;

			if (IsChecked)
			{
				if (_normalEllipse != null)
					_normalEllipse.Stroke = _radioButtonCheckMarkThemeColor;

				if (_checkMark != null)
					_checkMark.Opacity = 1;
			}
			else
			{
				if (_normalEllipse != null)
					_normalEllipse.Stroke = _radioButtonThemeColor;

				if (_checkMark != null)
					_checkMark.Opacity = 0;
			}
		}

		static ControlTemplate s_defaultTemplate;
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
				HeightRequest = 21,
				WidthRequest = 21,
				StrokeThickness = 2,
				Stroke = _radioButtonThemeColor,
				InputTransparent = true
			};

			var checkMark = new Ellipse
			{
				Fill = _radioButtonCheckMarkThemeColor,
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

		#endregion

		public static readonly BindableProperty ContentProperty =
		  BindableProperty.Create(nameof(Content), typeof(object), typeof(RadioButton), null);

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		public string Text
		{
			get => GetValue(ContentProperty)?.ToString();
			set => SetValue(ContentProperty, value);
		}

		[Obsolete]
		protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
		{
			if (ControlTemplate == null)
			{
				return Device.PlatformServices.GetNativeSize(this, widthConstraint, heightConstraint);
			}

			return base.OnSizeRequest(widthConstraint, heightConstraint);
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			VerifyExperimental(nameof(RadioButton));
			return base.OnMeasure(widthConstraint, heightConstraint);
		}

		#region ITextElement

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		public double CharacterSpacing
		{
			get { return (double)GetValue(TextElement.CharacterSpacingProperty); }
			set { SetValue(TextElement.CharacterSpacingProperty, value); }
		}

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		void ITextElement.OnCharacterSpacingPropertyChanged(double oldValue, double newValue)
		{
		}

		public virtual string UpdateFormsText(string source, TextTransform textTransform)
			=> TextTransformUtilites.GetTransformedText(source, textTransform);

		public TextTransform TextTransform
		{
			get { return (TextTransform)GetValue(TextElement.TextTransformProperty); }
			set { SetValue(TextElement.TextTransformProperty, value); }
		}

		void ITextElement.OnTextTransformChanged(TextTransform oldValue, TextTransform newValue)
		{
		}

		#endregion

		#region IFontElement

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;
		
		public string FontFamily
		{
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

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

		#endregion

		#region IBorderElement

		public static readonly BindableProperty BorderColorProperty = BorderElement.BorderColorProperty;

		public Color BorderColor
		{
			get { return (Color)GetValue(BorderColorProperty); }
			set { SetValue(BorderColorProperty, value); }
		}

		public static readonly BindableProperty CornerRadiusProperty = BorderElement.CornerRadiusProperty;

		public int CornerRadius
		{
			get { return (int)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		public static readonly BindableProperty BorderWidthProperty = BorderElement.BorderWidthProperty;

		public double BorderWidth
		{
			get { return (double)GetValue(BorderWidthProperty); }
			set { SetValue(BorderWidthProperty, value); }
		}

		int IBorderElement.CornerRadiusDefaultValue => (int)BorderElement.CornerRadiusProperty.DefaultValue;

		Color IBorderElement.BorderColorDefaultValue => (Color)BorderElement.BorderColorProperty.DefaultValue;

		double IBorderElement.BorderWidthDefaultValue => (double)BorderElement.BorderWidthProperty.DefaultValue;

		void IBorderElement.OnBorderColorPropertyChanged(Color oldValue, Color newValue)
		{

		}

		bool IBorderElement.IsCornerRadiusSet() => IsSet(BorderElement.CornerRadiusProperty);
		bool IBorderElement.IsBackgroundColorSet() => IsSet(BackgroundColorProperty);
		bool IBorderElement.IsBorderColorSet() => IsSet(BorderElement.BorderColorProperty);
		bool IBorderElement.IsBorderWidthSet() => IsSet(BorderElement.BorderWidthProperty);

		#endregion

		readonly Lazy<PlatformConfigurationRegistry<RadioButton>> _platformConfigurationRegistry;

		public const string IsCheckedVisualState = "IsChecked";

		public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(
			nameof(IsChecked), typeof(bool), typeof(RadioButton), false, propertyChanged: (b, o, n) => ((RadioButton)b).OnIsCheckedPropertyChanged((bool)n), defaultBindingMode: BindingMode.TwoWay);

		public static readonly BindableProperty GroupNameProperty = BindableProperty.Create(
			nameof(GroupName), typeof(string), typeof(RadioButton), null, propertyChanged: (b, o, n) => ((RadioButton)b).OnGroupNamePropertyChanged((string)o, (string)n));

		// TODO Needs implementations beyond Android
		//public static readonly BindableProperty ButtonSourceProperty = BindableProperty.Create(
		//	nameof(ButtonSource), typeof(ImageSource), typeof(RadioButton), null);

		public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

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

		// TODO Needs implementations beyond Android
		//public ImageSource ButtonSource
		//{
		//	get { return (ImageSource)GetValue(ButtonSourceProperty); }
		//	set { SetValue(ButtonSourceProperty, value); }
		//}

		public RadioButton()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<RadioButton>>(() => new PlatformConfigurationRegistry<RadioButton>(this));
			Initialize();
		}

		static bool isExperimentalFlagSet = false;
		internal static void VerifyExperimental([CallerMemberName] string memberName = "", string constructorHint = null)
		{
			if (isExperimentalFlagSet)
				return;

			ExperimentalFlags.VerifyFlagEnabled(nameof(RadioButton), ExperimentalFlags.RadioButtonExperimental, constructorHint, memberName);

			isExperimentalFlagSet = true;
		}

		public IPlatformElementConfiguration<T, RadioButton> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		protected internal override void ChangeVisualState()
		{
			if (IsEnabled && IsChecked)
				VisualStateManager.GoToState(this, IsCheckedVisualState);
			else
				base.ChangeVisualState();
		}

		#region Group Stuff

		void OnIsCheckedPropertyChanged(bool isChecked)
		{
			if (isChecked)
				UpdateRadioButtonGroup();

			CheckedChanged?.Invoke(this, new CheckedChangedEventArgs(isChecked));
			ChangeVisualState();
		}

		static Dictionary<string, List<WeakReference<RadioButton>>> _groupNameToElements;

		void OnGroupNamePropertyChanged(string oldGroupName, string newGroupName)
		{
			// Unregister the old group name if set
			if (!string.IsNullOrEmpty(oldGroupName))
				Unregister(this, oldGroupName);

			// Register the new group name is set
			if (!string.IsNullOrEmpty(newGroupName))
				Register(this, newGroupName);
		}

		void UpdateRadioButtonGroup()
		{
			string groupName = GroupName;
			if (!string.IsNullOrEmpty(groupName))
			{
				Element rootScope = GetVisualRoot(this);

				if (_groupNameToElements == null)
					_groupNameToElements = new Dictionary<string, List<WeakReference<RadioButton>>>(1);

				// Get all elements bound to this key and remove this element
				List<WeakReference<RadioButton>> elements = _groupNameToElements[groupName];
				for (int i = 0; i < elements.Count;)
				{
					WeakReference<RadioButton> weakRef = elements[i];
					if (weakRef.TryGetTarget(out RadioButton rb))
					{
						// Uncheck all checked RadioButtons different from the current one
						if (rb != this && (rb.IsChecked == true) && rootScope == GetVisualRoot(rb))
							rb.SetValueFromRenderer(IsCheckedProperty, false);

						i++;
					}
					else
					{
						// Remove dead instances
						elements.RemoveAt(i);
					}
				}
			}
			else // Logical parent should be the group
			{
				Element parent = Parent;
				if (parent != null)
				{
					// Traverse logical children
					IEnumerable children = parent.LogicalChildren;
					IEnumerator itor = children.GetEnumerator();
					while (itor.MoveNext())
					{
						var rb = itor.Current as RadioButton;
						if (rb != null && rb != this && string.IsNullOrEmpty(rb.GroupName) && (rb.IsChecked == true))
							rb.SetValueFromRenderer(IsCheckedProperty, false);
					}
				}
			}
		}

		static void Register(RadioButton radioButton, string groupName)
		{
			if (_groupNameToElements == null)
				_groupNameToElements = new Dictionary<string, List<WeakReference<RadioButton>>>(1);

			if (_groupNameToElements.TryGetValue(groupName, out List<WeakReference<RadioButton>> elements))
			{
				// There were some elements there, remove dead ones
				PurgeDead(elements, null);
			}
			else
			{
				elements = new List<WeakReference<RadioButton>>(1);
				_groupNameToElements[groupName] = elements;
			}

			elements.Add(new WeakReference<RadioButton>(radioButton));
		}

		static void Unregister(RadioButton radioButton, string groupName)
		{
			if (_groupNameToElements == null)
				return;

			// Get all elements bound to this key and remove this element
			if (_groupNameToElements.TryGetValue(groupName, out List<WeakReference<RadioButton>> elements))
			{
				PurgeDead(elements, radioButton);

				if (elements.Count == 0)
					_groupNameToElements.Remove(groupName);
			}
		}

		static void PurgeDead(List<WeakReference<RadioButton>> elements, object elementToRemove)
		{
			for (int i = 0; i < elements.Count;)
			{
				if (elements[i].TryGetTarget(out RadioButton rb) && rb == elementToRemove)
					elements.RemoveAt(i);
				else
					i++;
			}
		}

		static Element GetVisualRoot(Element element)
		{
			Element parent = element.Parent;
			while (parent != null && !(parent is Page))
				parent = parent.Parent;
			return parent;
		}

	

		#endregion
	}

	internal class RadioButtonGroupController
	{
		readonly WeakReference<Layout<View>> _layoutWeakReference;

		public RadioButtonGroupController(Layout<View> layout)
		{
			_layoutWeakReference = new WeakReference<Layout<View>>(layout);
		}

		string _groupName;
		RadioButton _selection;

		public string GroupName { get => _groupName; set => SetGroupName(value); }
		public RadioButton Selection { get => _selection; set => SetSelection(value); }

		// TODO this might not be necessary, simple setter might be fine
		void SetSelection(RadioButton radioButton)
		{
			_selection = radioButton;
		}

		void SetGroupName(string groupName)
		{
			_groupName = groupName;
		}
	}

	public static class RadioButtonGroup
	{
		static readonly BindableProperty RadioButtonGroupControllerProperty =
			BindableProperty.CreateAttached("RadioButtonGroupController", typeof(RadioButtonGroupController), typeof(Layout<View>), default(RadioButtonGroupController),
			defaultValueCreator: (b) => new RadioButtonGroupController((Layout<View>)b),
			propertyChanged: (b, o, n) => OnControllerChanged(b, (RadioButtonGroupController)o, (RadioButtonGroupController)n));

		static RadioButtonGroupController GetRadioButtonGroupController(BindableObject b)
		{
			return (RadioButtonGroupController)b.GetValue(RadioButtonGroupControllerProperty);
		}

		public static readonly BindableProperty GroupNameProperty =
			BindableProperty.Create("GroupName", typeof(string), typeof(Layout<View>), null, propertyChanged: (b, o, n) => { GetRadioButtonGroupController(b).GroupName = (string)n; });

		public static string GetGroupName(BindableObject b)
		{
			return (string)b.GetValue(GroupNameProperty);
		}

		public static readonly BindableProperty SelectionProperty =
			BindableProperty.Create("Selection", typeof(RadioButton), typeof(Layout<View>), null, propertyChanged: (b, o, n) => { GetRadioButtonGroupController(b).Selection = (RadioButton)n; });

		public static RadioButton GetSelection(BindableObject b)
		{
			return (RadioButton)b.GetValue(SelectionProperty);
		}

		static void OnControllerChanged(BindableObject b, RadioButtonGroupController oldC, RadioButtonGroupController newC)
		{
			if (newC == null)
			{
				return;
			}

			newC.GroupName = GetGroupName(b);
			newC.Selection = GetSelection(b);
		}
	}
}