using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Internals;
using System.ComponentModel;
using System.Linq;

namespace Xamarin.Forms
{
	[DebuggerDisplay("Title = {Title}, Route = {Route}")]
	public class BaseShellItem : NavigableElement, IPropertyPropagationController, IVisualController, IFlowDirectionController, ITabStopElement
	{
		public event EventHandler Appearing;
		public event EventHandler Disappearing;

		bool _hasAppearing;
		Grid _defaultFlyoutItemCell;
		const string DefaultFlyoutItemLabelStyle = "IMPL_DefaultFlyoutItemLabelStyle";
		const string DefaultFlyoutItemImageStyle = "IMPL_DefaultFlyoutItemImageStyle";
		const string DefaultFlyoutItemStyle = "IMPL_DefaultFlyoutItemStyle";

		#region PropertyKeys

		internal static readonly BindablePropertyKey IsCheckedPropertyKey = BindableProperty.CreateReadOnly(nameof(IsChecked), typeof(bool), typeof(BaseShellItem), false);

		#endregion PropertyKeys

		public static readonly BindableProperty FlyoutIconProperty =
			BindableProperty.Create(nameof(FlyoutIcon), typeof(ImageSource), typeof(BaseShellItem), null, BindingMode.OneTime);

		public static readonly BindableProperty IconProperty =
			BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(BaseShellItem), null, BindingMode.OneWay,
				propertyChanged: OnIconChanged);

		public static readonly BindableProperty IsCheckedProperty = IsCheckedPropertyKey.BindableProperty;

		public static readonly BindableProperty IsEnabledProperty =
			BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(BaseShellItem), true, BindingMode.OneWay);

		public static readonly BindableProperty TitleProperty =
			BindableProperty.Create(nameof(Title), typeof(string), typeof(BaseShellItem), null, BindingMode.OneTime);

		public static readonly BindableProperty TabIndexProperty =
			BindableProperty.Create(nameof(TabIndex),
							typeof(int),
							typeof(BaseShellItem),
							defaultValue: 0,
							propertyChanged: OnTabIndexPropertyChanged,
							defaultValueCreator: TabIndexDefaultValueCreator);

		public static readonly BindableProperty IsTabStopProperty =
			BindableProperty.Create(nameof(IsTabStop),
									typeof(bool),
									typeof(BaseShellItem),
									defaultValue: true,
									propertyChanged: OnTabStopPropertyChanged,
									defaultValueCreator: TabStopDefaultValueCreator);

		static void OnTabIndexPropertyChanged(BindableObject bindable, object oldValue, object newValue) =>
			((BaseShellItem)bindable).OnTabIndexPropertyChanged((int)oldValue, (int)newValue);

		static object TabIndexDefaultValueCreator(BindableObject bindable) =>
			((BaseShellItem)bindable).TabIndexDefaultValueCreator();

		static void OnTabStopPropertyChanged(BindableObject bindable, object oldValue, object newValue) =>
			((BaseShellItem)bindable).OnTabStopPropertyChanged((bool)oldValue, (bool)newValue);

		static object TabStopDefaultValueCreator(BindableObject bindable) =>
			((BaseShellItem)bindable).TabStopDefaultValueCreator();

		public ImageSource FlyoutIcon
		{
			get { return (ImageSource)GetValue(FlyoutIconProperty); }
			set { SetValue(FlyoutIconProperty, value); }
		}

		public ImageSource Icon
		{
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public bool IsChecked => (bool)GetValue(IsCheckedProperty);

		public bool IsEnabled
		{
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		public string Route
		{
			get { return Routing.GetRoute(this); }
			set { Routing.SetRoute(this, value); }
		}

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public int TabIndex
		{
			get => (int)GetValue(TabIndexProperty);
			set => SetValue(TabIndexProperty, value);
		}

		protected virtual void OnTabIndexPropertyChanged(int oldValue, int newValue) { }

		protected virtual int TabIndexDefaultValueCreator() => 0;

		public bool IsTabStop
		{
			get => (bool)GetValue(IsTabStopProperty);
			set => SetValue(IsTabStopProperty, value);
		}

		internal virtual void SendAppearing()
		{
			if (_hasAppearing)
				return;

			_hasAppearing = true;
			OnAppearing();
			Appearing?.Invoke(this, EventArgs.Empty);
		}

		internal virtual void SendDisappearing()
		{
			if (!_hasAppearing)
				return;

			_hasAppearing = false;
			OnDisappearing();
			Disappearing?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnAppearing()
		{
		}

		protected virtual void OnDisappearing()
		{
		}

		internal void OnAppearing(Action action)
		{
			if (_hasAppearing)
				action();
			else
			{
				if(Navigation.ModalStack.Count > 0)
				{
					Navigation.ModalStack[Navigation.ModalStack.Count - 1]
						.OnAppearing(action);
					
					return;
				}
				else if(Navigation.NavigationStack.Count > 1)
				{
					Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]
						.OnAppearing(action);

					return;
				}

				EventHandler eventHandler = null;
				eventHandler = (_, __) =>
				{
					this.Appearing -= eventHandler;
					action();
				};

				this.Appearing += eventHandler;
			}
		}

		protected virtual void OnTabStopPropertyChanged(bool oldValue, bool newValue) { }

		protected virtual bool TabStopDefaultValueCreator() => true;

		IVisual _effectiveVisual = Xamarin.Forms.VisualMarker.Default;
		IVisual IVisualController.EffectiveVisual
		{
			get { return _effectiveVisual; }
			set
			{
				if (value == _effectiveVisual)
					return;

				_effectiveVisual = value;
				OnPropertyChanged(VisualElement.VisualProperty.PropertyName);
			}
		}
		IVisual IVisualController.Visual => Xamarin.Forms.VisualMarker.MatchParent;

		static void OnIconChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (newValue == null || bindable.IsSet(FlyoutIconProperty))
				return;

			var shellItem = (BaseShellItem)bindable;
			shellItem.FlyoutIcon = (ImageSource)newValue;
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);
			if (Parent != null)
			{
				if (propertyName == Shell.ItemTemplateProperty.PropertyName || propertyName == nameof(Parent))
					Propagate(Shell.ItemTemplateProperty, this, Parent, true);
			}
		}

		internal static void PropagateFromParent(BindableProperty property, Element me)
		{
			if (me == null || me.Parent == null)
				return;

			Propagate(property, me.Parent, me, false);
		}

		internal static void Propagate(BindableProperty property, BindableObject from, BindableObject to, bool onlyToImplicit)
		{
			if (from == null || to == null)
				return;

			if (onlyToImplicit && Routing.IsImplicit(from))
				return;

			if (to is Shell)
				return;

			if (from.IsSet(property) && !to.IsSet(property))
				to.SetValue(property, from.GetValue(property));
		}

		void IPropertyPropagationController.PropagatePropertyChanged(string propertyName)
		{
			PropertyPropagationExtensions.PropagatePropertyChanged(propertyName, this, LogicalChildren);
		}

		EffectiveFlowDirection _effectiveFlowDirection = default(EffectiveFlowDirection);
		EffectiveFlowDirection IFlowDirectionController.EffectiveFlowDirection
		{
			get { return _effectiveFlowDirection; }
			set
			{
				if (value == _effectiveFlowDirection)
					return;

				_effectiveFlowDirection = value;

				var ve = (Parent as VisualElement);
				ve?.InvalidateMeasureInternal(InvalidationTrigger.Undefined);
				OnPropertyChanged(VisualElement.FlowDirectionProperty.PropertyName);
			}
		}
		bool IFlowDirectionController.ApplyEffectiveFlowDirectionToChildContainer => true;
		double IFlowDirectionController.Width => (Parent as VisualElement)?.Width ?? 0;


		internal virtual void ApplyQueryAttributes(IDictionary<string, string> query)
		{
		}


		internal override void OnStyleClassChanged()
		{
			if (_defaultFlyoutItemCell == null)
				return;

			base.OnStyleClassChanged();
			List<string> bindableObjectStyle = new List<string>() {
				DefaultFlyoutItemLabelStyle,
				DefaultFlyoutItemImageStyle,
				DefaultFlyoutItemStyle };

			if (StyleClass != null)
				foreach (var styleClass in StyleClass)
					bindableObjectStyle.Add(styleClass);

			_defaultFlyoutItemCell.StyleClass = bindableObjectStyle;
			_defaultFlyoutItemCell.Children.OfType<Label>().First()
				.StyleClass = bindableObjectStyle;
			_defaultFlyoutItemCell.Children.OfType<Image>().First()
				.StyleClass = bindableObjectStyle;
		}

		internal DataTemplate CreateDefaultFlyoutItemCell(string textBinding, string iconBinding)
		{
			return new DataTemplate(() =>
			{
				if (_defaultFlyoutItemCell != null)
					return _defaultFlyoutItemCell;

				var grid = _defaultFlyoutItemCell = new Grid();
				
				grid.Resources = new ResourceDictionary();

				var defaultLabelClass = new Style(typeof(Label))
				{
					Setters = {
						new Setter { Property = Label.VerticalTextAlignmentProperty, Value = TextAlignment.Center }
					},
					Class = DefaultFlyoutItemLabelStyle,
				};

				var defaultImageClass = new Style(typeof(Image))
				{
					Setters = {
						new Setter { Property = Image.VerticalOptionsProperty, Value = LayoutOptions.Center }
					},
					Class = DefaultFlyoutItemImageStyle,
				};

				var defaultGridClass = new Style(typeof(Grid))
				{
					Class = DefaultFlyoutItemStyle,
				};

				List<string> bindableObjectStyle = new List<string>() { defaultLabelClass.Class, defaultImageClass.Class, defaultGridClass.Class };

				if (StyleClass != null)
					foreach (var styleClass in StyleClass)
						bindableObjectStyle.Add(styleClass);

				var groups = new VisualStateGroupList();

				var commonGroup = new VisualStateGroup();
				commonGroup.Name = "CommonStates";
				groups.Add(commonGroup);

				var normalState = new VisualState();
				normalState.Name = "Normal";
				commonGroup.States.Add(normalState);

				var selectedState = new VisualState();
				selectedState.Name = "Selected";
				selectedState.Setters.Add(new Setter
				{
					Property = VisualElement.BackgroundColorProperty,
					Value = new Color(0.95)
				});

				commonGroup.States.Add(selectedState);

				defaultGridClass.Setters.Add(new Setter { Property = VisualStateManager.VisualStateGroupsProperty, Value = groups });

				if (Device.RuntimePlatform == Device.Android)
					defaultGridClass.Setters.Add(new Setter { Property = Grid.HeightRequestProperty, Value = 50 });

				ColumnDefinitionCollection columnDefinitions = new ColumnDefinitionCollection();

				if (Device.RuntimePlatform == Device.Android)
					columnDefinitions.Add(new ColumnDefinition { Width = 54 });
				else
					columnDefinitions.Add(new ColumnDefinition { Width = 50 });

				columnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
				defaultGridClass.Setters.Add(new Setter { Property = Grid.ColumnDefinitionsProperty, Value = columnDefinitions });

				var image = new Image();

				double sizeRequest = 22;
				if (Device.RuntimePlatform == Device.Android)
					sizeRequest = 24;

				defaultImageClass.Setters.Add(new Setter() { Property = Image.HeightRequestProperty, Value = sizeRequest });
				defaultImageClass.Setters.Add(new Setter() { Property = Image.WidthRequestProperty, Value = sizeRequest });

				image.SetBinding(Image.SourceProperty, iconBinding);
				grid.Children.Add(image);

				var label = new Label();
				label.SetBinding(Label.TextProperty, textBinding);
				grid.Children.Add(label, 1, 0);

				if (Device.RuntimePlatform == Device.Android)
				{
					defaultLabelClass.Setters.Add(new Setter { Property = Label.FontSizeProperty, Value = 14 });
					defaultLabelClass.Setters.Add(new Setter { Property = Label.TextColorProperty, Value = Color.Black.MultiplyAlpha(0.87) });
					defaultLabelClass.Setters.Add(new Setter { Property = Label.FontFamilyProperty, Value = "sans-serif-medium" });
					defaultLabelClass.Setters.Add(new Setter { Property = Label.MarginProperty, Value = new Thickness(20, 0, 0, 0) });
				}
				else
				{
					defaultLabelClass.Setters.Add(new Setter { Property = Label.FontSizeProperty, Value = Device.GetNamedSize(NamedSize.Small, label) });
					defaultLabelClass.Setters.Add(new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold });
				}

				grid.StyleClass = bindableObjectStyle;
				label.StyleClass = bindableObjectStyle;
				image.StyleClass = bindableObjectStyle;

				grid.Resources = new ResourceDictionary() { defaultGridClass, defaultLabelClass, defaultImageClass };
				return grid;
			});
		}
	}

	public interface IQueryAttributable
	{
		void ApplyQueryAttributes(IDictionary<string, string> query);
	}
}