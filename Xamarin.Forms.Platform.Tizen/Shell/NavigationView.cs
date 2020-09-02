using System;
using System.Collections.Generic;
using ElmSharp;
using Xamarin.Forms.Internals;
using EColor = ElmSharp.Color;
using EImage = ElmSharp.Image;

namespace Xamarin.Forms.Platform.Tizen
{
	public class NavigationView : Background, INavigationView
	{
		static EColor s_defaultBackgroundColor = EColor.White;

		Box _mainLayout;
		Scroller _scroller;

		EImage _backgroundImage;
		Aspect _bgImageAspect;
		ImageSource _bgImageSource;

		View _header;
		EvasObject _nativeHeader;

		GenList _menu;
		GenItemClass _templateClass;

		EColor _backgroundColor;

		Element _lastSelected;
		Dictionary<Element, View> _cachedView = new Dictionary<Element, View>();

		List<List<Element>> _cachedGroups;

		public NavigationView(EvasObject parent, Shell shell) : base(parent)
		{
			Shell = shell;
			InitializeComponent(parent);
		}

		public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

		public EvasObject NativeView => this;

		public IShellController ShellController => Shell;
		public Shell Shell { get; }

		FlyoutHeaderBehavior _headerBehavior = FlyoutHeaderBehavior.Fixed;
		public FlyoutHeaderBehavior HeaderBehavior
		{
			get => _headerBehavior;
			set
			{
				_headerBehavior = value;
				UpdateHeaderBehavior();
			}
		}

		public override EColor BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set
			{
				_backgroundColor = value;
				EColor effectiveColor = _backgroundColor.IsDefault ? s_defaultBackgroundColor : _backgroundColor;
				base.BackgroundColor = effectiveColor;
			}
		}

		public Aspect BackgroundImageAspect
		{
			get
			{
				return _bgImageAspect;
			}
			set
			{
				_bgImageAspect = value;
				_backgroundImage?.ApplyAspect(_bgImageAspect);
			}
		}

		public ImageSource BackgroundImageSource
		{
			get
			{
				return _bgImageSource;
			}
			set
			{
				_bgImageSource = value;
				UpdateBackgroundImage();
			}
		}

		public View Header
		{
			get
			{
				return _header;
			}
			set
			{
				UpdateHeader(value);
			}
		}

		public void BuildMenu(List<List<Element>> flyoutGroups)
		{
			if (!IsMenuItemChanged(flyoutGroups))
			{
				return;
			}
			_cachedGroups = flyoutGroups;

			_menu.Clear();
			_cachedView.Clear();
			_lastSelected = null;
			foreach (var group in flyoutGroups)
			{
				bool isFirst = true;
				foreach (var element in group)
				{
					var item = _menu.Append(_templateClass, element);
					if (isFirst)
					{
						isFirst = false;
					}
					else
					{
						item.SetBottomlineColor(EColor.Transparent);
					}
					item.SetBackgroundColor(EColor.Transparent);
				}
			}
		}

		void InitializeComponent(EvasObject parent)
		{
			base.BackgroundColor = s_defaultBackgroundColor;

			_scroller = new Scroller(parent)
			{
				AlignmentX = -1,
				AlignmentY = -1,
				WeightX = 1,
				WeightY = 1,
				ScrollBlock = ScrollBlock.Horizontal,
			};
			_scroller.Show();

			_mainLayout = new Box(parent)
			{
				AlignmentX = -1,
				AlignmentY = -1,
				WeightX = 1,
				WeightY = 1
			};
			_mainLayout.SetLayoutCallback(OnLayout);
			_mainLayout.Show();

			_scroller.SetContent(_mainLayout);
			SetContent(_scroller);

			_menu = new GenList(parent)
			{
				Homogeneous = false,
				SelectionMode = GenItemSelectionMode.Always,
				BackgroundColor = EColor.Transparent,
				Style = "solid/default",
				ListMode = GenListMode.Scroll,
			};

			_menu.ItemSelected += (s, e) =>
			{
				if (_lastSelected != null)
				{
					VisualStateManager.GoToState(_cachedView[_lastSelected], "Normal");
				}
				var item = e.Item.Data as Element;
				_lastSelected = item;
				VisualStateManager.GoToState(_cachedView[_lastSelected], "Selected");
				SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(item, -1));
			};

			_menu.Show();
			_mainLayout.PackEnd(_menu);

			_templateClass = new GenItemClass(ThemeConstants.GenItemClass.Styles.Full)
			{
				GetContentHandler = GetTemplatedContent,
			};
		}

		EvasObject GetTemplatedContent(object data, string part)
		{
			Element item = data as Element;
			View view;
			if (!_cachedView.TryGetValue(item, out view))
			{ 
				view = (View)GetFlyoutItemDataTemplate(item).CreateContent(item, Shell);
				view.Parent = Shell;
				view.BindingContext = item;
				_cachedView[item] = view;
			}
			if (item == _lastSelected)
			{
				VisualStateManager.GoToState(view, "Selected");
			}
			else
			{
				VisualStateManager.GoToState(view, "Normal");
			}

			return GetNativeView(view);
		}

		EvasObject GetNativeView(View view)
		{
			var measuredSize = view.Measure(Forms.ConvertToScaledDP(Geometry.Width), Forms.ConvertToScaledDP(Geometry.Height));
			var renderer = Platform.GetOrCreateRenderer(view);
			(renderer as LayoutRenderer)?.RegisterOnLayoutUpdated();
			renderer.NativeView.MinimumHeight = Forms.ConvertToScaledPixel(measuredSize.Request.Height);
			return renderer.NativeView;
		}

		DataTemplate GetFlyoutItemDataTemplate(BindableObject bo)
		{
			string textBinding;
			string iconBinding;
			if (bo is IMenuItemController)
			{

				if (bo is MenuItem mi && mi.Parent != null && mi.Parent.IsSet(Shell.MenuItemTemplateProperty))
				{
					return Shell.GetMenuItemTemplate(mi.Parent);
				}
				else if (bo.IsSet(Shell.MenuItemTemplateProperty))
				{
					return Shell.GetMenuItemTemplate(bo);
				}

				if (Shell.MenuItemTemplate != null)
					return Shell.MenuItemTemplate;

				textBinding = "Text";
				iconBinding = "Icon";
			}
			else
			{
				if (Shell.GetItemTemplate(bo) != null)
					return Shell.GetItemTemplate(bo);
				else if (Shell.ItemTemplate != null)
					return Shell.ItemTemplate;

				textBinding = "Title";
				iconBinding = "FlyoutIcon";
			}
			return new DataTemplate(() =>
			{
				var grid = new Grid
				{
					HeightRequest = this.GetFlyoutItemHeight(),
				};

				ColumnDefinitionCollection columnDefinitions = new ColumnDefinitionCollection();
				columnDefinitions.Add(new ColumnDefinition { Width = this.GetFlyoutIconColumnSize() });
				columnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
				grid.ColumnDefinitions = columnDefinitions;

				var image = new Image
				{
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center,
					HeightRequest = this.GetFlyoutIconSize(),
					WidthRequest = this.GetFlyoutIconSize(),
					Margin = new Thickness(this.GetFlyoutMargin(), 0, 0, 0),
				};
				image.SetBinding(Image.SourceProperty, new Binding(iconBinding));
				grid.Children.Add(image);

				var label = new Label
				{
					FontSize = this.GetFlyoutItemFontSize(),
					VerticalTextAlignment = TextAlignment.Center,
					TextColor = Xamarin.Forms.Color.Black.MultiplyAlpha(0.87),
					Margin = new Thickness(this.GetFlyoutMargin(), 0, 0, 0),
				};
				label.SetBinding(Label.TextProperty, new Binding(textBinding));

				grid.Children.Add(label, 1, 0);

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
				VisualStateManager.SetVisualStateGroups(grid, groups);
				return grid;
			});
		}

		void UpdateBackgroundImage()
		{
			if (BackgroundImageSource == null)
			{
				if (_backgroundImage != null)
				{
					this.SetBackgroundPart(null);
					_backgroundImage = null;
				}
			}
			else
			{
				if (_backgroundImage == null)
				{
					_backgroundImage = new EImage(this);
					this.SetBackgroundPart(_backgroundImage);
				}
				_backgroundImage.LoadFromImageSourceAsync(BackgroundImageSource).GetAwaiter().OnCompleted(() =>
				{
					_backgroundImage.ApplyAspect(_bgImageAspect);
				});
			}
		}

		void UpdateHeader(View header)
		{
			if (_header != null)
			{
				_header.MeasureInvalidated -= OnHeaderSizeChanged;
			}
			if (_nativeHeader != null)
			{
				_mainLayout.UnPack(_nativeHeader);
				_nativeHeader.Unrealize();
				_nativeHeader = null;
			}

			if (header != null)
			{
				header.MeasureInvalidated += OnHeaderSizeChanged;
				var renderer = Platform.GetOrCreateRenderer(header);
				(renderer as LayoutRenderer)?.RegisterOnLayoutUpdated();
				_nativeHeader = renderer.NativeView;
				_mainLayout.PackEnd(_nativeHeader);
			}
			_header = header;
		}

		void UpdateHeaderBehavior()
		{
			if (HeaderBehavior == FlyoutHeaderBehavior.Scroll || HeaderBehavior == FlyoutHeaderBehavior.CollapseOnScroll)
			{
				_menu.ListMode = GenListMode.Expand;
			}
			else
			{
				_menu.ListMode = GenListMode.Scroll;
			}
			OnLayout();
		}

		bool IsMenuItemChanged(List<List<Element>> flyoutGroups)
		{
			if (_cachedGroups == null)
				return true;

			if (_cachedGroups.Count != flyoutGroups.Count)
				return true;

			for (int i = 0; i < flyoutGroups.Count; i++)
			{
				if (_cachedGroups[i].Count != flyoutGroups[i].Count)
					return true;

				for (int j = 0; j < flyoutGroups[i].Count; j++)
				{
					if (_cachedGroups[i][j] != flyoutGroups[i][j])
						return true;
				}
			}
			return false;
		}

		void OnHeaderSizeChanged(object sender, EventArgs e)
		{
			OnLayout();
		}

		void OnLayout()
		{
			if (Geometry.Width == 0 || Geometry.Height == 0)
				return;
			var bound = Geometry;
			int headerHeight = 0;
			if (Header != null)
			{
				var requestSize = Header.Measure(Forms.ConvertToScaledDP(bound.Width), Forms.ConvertToScaledDP(bound.Height));
				headerHeight = Forms.ConvertToScaledPixel(requestSize.Request.Height);
				var headerBound = Geometry;
				headerBound.Height = headerHeight;
				_nativeHeader.Geometry = headerBound;
			}

			bound.Y += headerHeight;

			if (HeaderBehavior == FlyoutHeaderBehavior.Scroll || HeaderBehavior == FlyoutHeaderBehavior.CollapseOnScroll)
			{
				bound.Height = _menu.MinimumHeight;
			}
			else
			{
				bound.Height -= headerHeight;
			}
			_menu.Geometry = bound;

			_mainLayout.MinimumWidth = _scroller.Geometry.Width;
			_mainLayout.MinimumHeight = headerHeight + bound.Height;
		}
	}
}
