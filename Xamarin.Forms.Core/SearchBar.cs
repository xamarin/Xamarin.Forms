using System;
using System.Windows.Input;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_SearchBarRenderer))]
	public class SearchBar : View, IFontElement, ISearchBarController, IElementConfiguration<SearchBar>
	{
		public static readonly BindableProperty SearchCommandProperty = BindableProperty.Create("SearchCommand", typeof(ICommand), typeof(SearchBar), null, propertyChanged: OnCommandChanged);

		public static readonly BindableProperty SearchCommandParameterProperty = BindableProperty.Create("SearchCommandParameter", typeof(object), typeof(SearchBar), null);

		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(SearchBar), default(string), BindingMode.TwoWay,
			propertyChanged: (bindable, oldValue, newValue) =>
			{
				var searchBar = (SearchBar)bindable;
				searchBar.TextChanged?.Invoke(searchBar, new TextChangedEventArgs((string)oldValue, (string)newValue));
			});

		public static readonly BindableProperty CancelButtonColorProperty = BindableProperty.Create("CancelButtonColor", typeof(Color), typeof(SearchBar), default(Color));

		public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create("Placeholder", typeof(string), typeof(SearchBar), null);

		public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create("FontFamily", typeof(string), typeof(SearchBar), default(string));

		public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(SearchBar), -1.0,
			defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, (SearchBar)bindable));

		public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create("FontAttributes", typeof(FontAttributes), typeof(SearchBar), FontAttributes.None);

		public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create("HorizontalTextAlignment", typeof(TextAlignment), typeof(SearchBar), TextAlignment.Start);

		public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(SearchBar), Color.Default);

		public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create("PlaceholderColor", typeof(Color), typeof(SearchBar), Color.Default);

		readonly Lazy<PlatformConfigurationRegistry<SearchBar>> _platformConfigurationRegistry;

		public Color CancelButtonColor
		{
			get { return (Color)GetValue(CancelButtonColorProperty); }
			set { SetValue(CancelButtonColorProperty, value); }
		}

		public TextAlignment HorizontalTextAlignment
		{
			get { return (TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
			set { SetValue(HorizontalTextAlignmentProperty, value); }
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

		public ICommand SearchCommand
		{
			get { return (ICommand)GetValue(SearchCommandProperty); }
			set { SetValue(SearchCommandProperty, value); }
		}

		public object SearchCommandParameter
		{
			get { return GetValue(SearchCommandParameterProperty); }
			set { SetValue(SearchCommandParameterProperty, value); }
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

		bool IsEnabledCore
		{
			set { SetValueCore(IsEnabledProperty, value); }
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

		public event EventHandler SearchButtonPressed;

		public event EventHandler<TextChangedEventArgs> TextChanged;

		public SearchBar()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<SearchBar>>(() => new PlatformConfigurationRegistry<SearchBar>(this));
		}

		void ISearchBarController.OnSearchButtonPressed()
		{
			ICommand cmd = SearchCommand;

			if (cmd != null && !cmd.CanExecute(SearchCommandParameter))
				return;

			cmd?.Execute(SearchCommandParameter);
			SearchButtonPressed?.Invoke(this, EventArgs.Empty);
		}

		void CommandCanExecuteChanged(object sender, EventArgs eventArgs)
		{
			ICommand cmd = SearchCommand;
			if (cmd != null)
				IsEnabledCore = cmd.CanExecute(SearchCommandParameter);
		}

		static void OnCommandChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var self = (SearchBar)bindable;
			var newCommand = (ICommand)newValue;
			var oldCommand = (ICommand)oldValue;

			if (oldCommand != null)
			{
				oldCommand.CanExecuteChanged -= self.CommandCanExecuteChanged;
			}

			if (newCommand != null)
			{
				newCommand.CanExecuteChanged += self.CommandCanExecuteChanged;
				self.CommandCanExecuteChanged(self, EventArgs.Empty);
			}
			else
			{
				self.IsEnabledCore = true;
			}
		}

		public IPlatformElementConfiguration<T, SearchBar> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}
	}
}