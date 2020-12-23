﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.StyleSheets;

namespace Xamarin.Forms
{
	[ContentProperty(nameof(Items))]
	public class Shell : Page, IShellController, IPropertyPropagationController, IPageContainer<Page>
	{
		public Page CurrentPage => (CurrentSection as IShellSectionController)?.PresentedPage;

		public static readonly BindableProperty BackButtonBehaviorProperty =
			BindableProperty.CreateAttached("BackButtonBehavior", typeof(BackButtonBehavior), typeof(Shell), null, BindingMode.OneTime,
				propertyChanged: OnBackButonBehaviorPropertyChanged);

		static void OnBackButonBehaviorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (oldValue is BackButtonBehavior oldHandlerBehavior)
				SetInheritedBindingContext(oldHandlerBehavior, null);
			if (newValue is BackButtonBehavior newHandlerBehavior)
				SetInheritedBindingContext(newHandlerBehavior, bindable.BindingContext);
		}

		public static readonly BindableProperty PresentationModeProperty = BindableProperty.CreateAttached("PresentationMode", typeof(PresentationMode), typeof(Shell), PresentationMode.Animated);

		public static readonly BindableProperty FlyoutBehaviorProperty =
			BindableProperty.CreateAttached("FlyoutBehavior", typeof(FlyoutBehavior), typeof(Shell), FlyoutBehavior.Flyout,
				propertyChanged: OnFlyoutBehaviorChanged);

		public static readonly BindableProperty NavBarIsVisibleProperty =
			BindableProperty.CreateAttached("NavBarIsVisible", typeof(bool), typeof(Shell), true);

		public static readonly BindableProperty NavBarHasShadowProperty =
			BindableProperty.CreateAttached("NavBarHasShadow", typeof(bool), typeof(Shell), default(bool),
				defaultValueCreator: (b) => Device.RuntimePlatform == Device.Android);

		public static readonly BindableProperty SearchHandlerProperty =
			BindableProperty.CreateAttached("SearchHandler", typeof(SearchHandler), typeof(Shell), null, BindingMode.OneTime,
				propertyChanged: OnSearchHandlerPropertyChanged);

		static void OnSearchHandlerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (oldValue is SearchHandler oldHandler)
				SetInheritedBindingContext(oldHandler, null);
			if (newValue is SearchHandler newHandler)
				SetInheritedBindingContext(newHandler, bindable.BindingContext);
		}

		public static readonly BindableProperty FlyoutItemIsVisibleProperty =
			BindableProperty.CreateAttached(nameof(IsVisible), typeof(bool), typeof(Shell), true, propertyChanged: OnFlyoutItemIsVisibleChanged);
		public static bool GetFlyoutItemIsVisible(BindableObject obj) => (bool)obj.GetValue(FlyoutItemIsVisibleProperty);
		public static void SetFlyoutItemIsVisible(BindableObject obj, bool isVisible) => obj.SetValue(FlyoutItemIsVisibleProperty, isVisible);

		static void OnFlyoutItemIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (bindable is Element element)
				element
					.FindParentOfType<Shell>()
					?.SendStructureChanged();
		}

		public static readonly BindableProperty TabBarIsVisibleProperty =
			BindableProperty.CreateAttached("TabBarIsVisible", typeof(bool), typeof(Shell), true);

		public static readonly BindableProperty TitleViewProperty =
			BindableProperty.CreateAttached("TitleView", typeof(View), typeof(Shell), null, propertyChanged: OnTitleViewChanged);

		public static readonly BindableProperty MenuItemTemplateProperty =
			BindableProperty.CreateAttached(nameof(MenuItemTemplate), typeof(DataTemplate), typeof(Shell), null, BindingMode.OneTime);

		public static DataTemplate GetMenuItemTemplate(BindableObject obj) => (DataTemplate)obj.GetValue(MenuItemTemplateProperty);
		public static void SetMenuItemTemplate(BindableObject obj, DataTemplate menuItemTemplate) => obj.SetValue(MenuItemTemplateProperty, menuItemTemplate);

		public static readonly BindableProperty ItemTemplateProperty =
			BindableProperty.CreateAttached(nameof(ItemTemplate), typeof(DataTemplate), typeof(Shell), null, BindingMode.OneTime);

		public static DataTemplate GetItemTemplate(BindableObject obj) => (DataTemplate)obj.GetValue(ItemTemplateProperty);
		public static void SetItemTemplate(BindableObject obj, DataTemplate itemTemplate) => obj.SetValue(ItemTemplateProperty, itemTemplate);

		public static BackButtonBehavior GetBackButtonBehavior(BindableObject obj) => (BackButtonBehavior)obj.GetValue(BackButtonBehaviorProperty);
		public static void SetBackButtonBehavior(BindableObject obj, BackButtonBehavior behavior) => obj.SetValue(BackButtonBehaviorProperty, behavior);

		public static PresentationMode GetPresentationMode(BindableObject obj) => (PresentationMode)obj.GetValue(PresentationModeProperty);
		public static void SetPresentationMode(BindableObject obj, PresentationMode presentationMode) => obj.SetValue(PresentationModeProperty, presentationMode);

		public static FlyoutBehavior GetFlyoutBehavior(BindableObject obj) => (FlyoutBehavior)obj.GetValue(FlyoutBehaviorProperty);
		public static void SetFlyoutBehavior(BindableObject obj, FlyoutBehavior value) => obj.SetValue(FlyoutBehaviorProperty, value);

		public static double GetFlyoutWidth(BindableObject obj) => (double)obj.GetValue(FlyoutWidthProperty);
		public static void SetFlyoutWidth(BindableObject obj, double value) => obj.SetValue(FlyoutWidthProperty, value);

		public static double GetFlyoutHeight(BindableObject obj) => (double)obj.GetValue(FlyoutHeightProperty);
		public static void SetFlyoutHeight(BindableObject obj, double value) => obj.SetValue(FlyoutHeightProperty, value);


		public static bool GetNavBarIsVisible(BindableObject obj) => (bool)obj.GetValue(NavBarIsVisibleProperty);
		public static void SetNavBarIsVisible(BindableObject obj, bool value) => obj.SetValue(NavBarIsVisibleProperty, value);

		public static bool GetNavBarHasShadow(BindableObject obj) => (bool)obj.GetValue(NavBarHasShadowProperty);
		public static void SetNavBarHasShadow(BindableObject obj, bool value) => obj.SetValue(NavBarHasShadowProperty, value);

		public static SearchHandler GetSearchHandler(BindableObject obj) => (SearchHandler)obj.GetValue(SearchHandlerProperty);
		public static void SetSearchHandler(BindableObject obj, SearchHandler handler) => obj.SetValue(SearchHandlerProperty, handler);

		public static bool GetTabBarIsVisible(BindableObject obj) => (bool)obj.GetValue(TabBarIsVisibleProperty);
		public static void SetTabBarIsVisible(BindableObject obj, bool value) => obj.SetValue(TabBarIsVisibleProperty, value);

		public static View GetTitleView(BindableObject obj) => (View)obj.GetValue(TitleViewProperty);
		public static void SetTitleView(BindableObject obj, View value) => obj.SetValue(TitleViewProperty, value);

		static void OnFlyoutBehaviorChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var element = (Element)bindable;

			while (!Application.IsApplicationOrNull(element))
			{
				if (element is Shell shell)
					shell.NotifyFlyoutBehaviorObservers();
				element = element.Parent;
			}
		}

		public static readonly new BindableProperty BackgroundColorProperty =
			BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty DisabledColorProperty =
			BindableProperty.CreateAttached("DisabledColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty ForegroundColorProperty =
			BindableProperty.CreateAttached("ForegroundColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty TabBarBackgroundColorProperty =
			BindableProperty.CreateAttached("TabBarBackgroundColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty TabBarDisabledColorProperty =
			BindableProperty.CreateAttached("TabBarDisabledColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty TabBarForegroundColorProperty =
			BindableProperty.CreateAttached("TabBarForegroundColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty TabBarTitleColorProperty =
			BindableProperty.CreateAttached("TabBarTitleColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty TabBarUnselectedColorProperty =
			BindableProperty.CreateAttached("TabBarUnselectedColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty TitleColorProperty =
			BindableProperty.CreateAttached("TitleColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty UnselectedColorProperty =
			BindableProperty.CreateAttached("UnselectedColor", typeof(Color), typeof(Shell), Color.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty FlyoutBackdropProperty =
			BindableProperty.CreateAttached("FlyoutBackdrop", typeof(Brush), typeof(Shell), Brush.Default,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty FlyoutWidthProperty =
			BindableProperty.CreateAttached("FlyoutWidth", typeof(double), typeof(Shell), -1d,
				propertyChanged: OnShellAppearanceValueChanged);

		public static readonly BindableProperty FlyoutHeightProperty =
			BindableProperty.CreateAttached("FlyoutHeight", typeof(double), typeof(Shell), -1d,
				propertyChanged: OnShellAppearanceValueChanged);

		public static Color GetBackgroundColor(BindableObject obj) => (Color)obj.GetValue(BackgroundColorProperty);
		public static void SetBackgroundColor(BindableObject obj, Color value) => obj.SetValue(BackgroundColorProperty, value);

		public static Color GetDisabledColor(BindableObject obj) => (Color)obj.GetValue(DisabledColorProperty);
		public static void SetDisabledColor(BindableObject obj, Color value) => obj.SetValue(DisabledColorProperty, value);

		public static Color GetForegroundColor(BindableObject obj) => (Color)obj.GetValue(ForegroundColorProperty);
		public static void SetForegroundColor(BindableObject obj, Color value) => obj.SetValue(ForegroundColorProperty, value);

		public static Color GetTabBarBackgroundColor(BindableObject obj) => (Color)obj.GetValue(TabBarBackgroundColorProperty);
		public static void SetTabBarBackgroundColor(BindableObject obj, Color value) => obj.SetValue(TabBarBackgroundColorProperty, value);

		public static Color GetTabBarDisabledColor(BindableObject obj) => (Color)obj.GetValue(TabBarDisabledColorProperty);
		public static void SetTabBarDisabledColor(BindableObject obj, Color value) => obj.SetValue(TabBarDisabledColorProperty, value);

		public static Color GetTabBarForegroundColor(BindableObject obj) => (Color)obj.GetValue(TabBarForegroundColorProperty);
		public static void SetTabBarForegroundColor(BindableObject obj, Color value) => obj.SetValue(TabBarForegroundColorProperty, value);

		public static Color GetTabBarTitleColor(BindableObject obj) => (Color)obj.GetValue(TabBarTitleColorProperty);
		public static void SetTabBarTitleColor(BindableObject obj, Color value) => obj.SetValue(TabBarTitleColorProperty, value);

		public static Color GetTabBarUnselectedColor(BindableObject obj) => (Color)obj.GetValue(TabBarUnselectedColorProperty);
		public static void SetTabBarUnselectedColor(BindableObject obj, Color value) => obj.SetValue(TabBarUnselectedColorProperty, value);

		public static Color GetTitleColor(BindableObject obj) => (Color)obj.GetValue(TitleColorProperty);
		public static void SetTitleColor(BindableObject obj, Color value) => obj.SetValue(TitleColorProperty, value);

		public static Color GetUnselectedColor(BindableObject obj) => (Color)obj.GetValue(UnselectedColorProperty);
		public static void SetUnselectedColor(BindableObject obj, Color value) => obj.SetValue(UnselectedColorProperty, value);

		public static Brush GetFlyoutBackdrop(BindableObject obj) => (Brush)obj.GetValue(FlyoutBackdropProperty);
		public static void SetFlyoutBackdrop(BindableObject obj, Brush value) => obj.SetValue(FlyoutBackdropProperty, value);

		static void OnShellAppearanceValueChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var item = (Element)bindable;
			var source = item;

			while (!Application.IsApplicationOrNull(item))
			{
				if (item is IShellController shell)
				{
					shell.AppearanceChanged(source, true);
					return;
				}
				item = item.Parent;
			}
		}

		static readonly BindablePropertyKey CurrentStatePropertyKey =
			BindableProperty.CreateReadOnly(nameof(CurrentState), typeof(ShellNavigationState), typeof(Shell), null);

		static readonly BindablePropertyKey ItemsPropertyKey = BindableProperty.CreateReadOnly(nameof(Items), typeof(ShellItemCollection), typeof(Shell), null,
				defaultValueCreator: bo => new ShellItemCollection { Inner = new ElementCollection<ShellItem>(((Shell)bo).InternalChildren) });

		List<(IAppearanceObserver Observer, Element Pivot)> _appearanceObservers = new List<(IAppearanceObserver Observer, Element Pivot)>();
		List<IFlyoutBehaviorObserver> _flyoutBehaviorObservers = new List<IFlyoutBehaviorObserver>();


		internal static BindableObject GetBindableObjectWithFlyoutItemTemplate(BindableObject bo)
		{
			if (bo is IMenuItemController)
			{
				if (bo is MenuItem mi && mi.Parent != null && mi.Parent.IsSet(MenuItemTemplateProperty))
					return mi.Parent;
				else if (bo is MenuShellItem msi && msi.MenuItem != null && msi.MenuItem.IsSet(MenuItemTemplateProperty))
					return msi.MenuItem;
			}

			return bo;
		}

		DataTemplate IShellController.GetFlyoutItemDataTemplate(BindableObject bo)
		{
			BindableProperty bp = null;
			string textBinding;
			string iconBinding;
			var bindableObjectWithTemplate = GetBindableObjectWithFlyoutItemTemplate(bo);

			if (bo is IMenuItemController)
			{
				bp = MenuItemTemplateProperty;
				textBinding = "Text";
				iconBinding = "Icon";
			}
			else
			{
				bp = ItemTemplateProperty;
				textBinding = "Title";
				iconBinding = "FlyoutIcon";
			}

			if (bindableObjectWithTemplate.IsSet(bp))
			{
				return (DataTemplate)bindableObjectWithTemplate.GetValue(bp);
			}

			if (IsSet(bp))
			{
				return (DataTemplate)GetValue(bp);
			}

			return BaseShellItem.CreateDefaultFlyoutItemCell(textBinding, iconBinding);
		}

		event EventHandler IShellController.StructureChanged
		{
			add { _structureChanged += value; }
			remove { _structureChanged -= value; }
		}

		event EventHandler _structureChanged;

		View IShellController.FlyoutHeader => FlyoutHeaderView;
		View IShellController.FlyoutFooter => FlyoutFooterView;
		View IShellController.FlyoutContent => FlyoutContentView;

		IShellController ShellController => this;

		void IShellController.AddAppearanceObserver(IAppearanceObserver observer, Element pivot)
		{
			_appearanceObservers.Add((observer, pivot));
			observer.OnAppearanceChanged(GetAppearanceForPivot(pivot));
		}

		void IShellController.AddFlyoutBehaviorObserver(IFlyoutBehaviorObserver observer)
		{
			_flyoutBehaviorObservers.Add(observer);

			// We need to wait until the visible page has been created before we try to calculate
			// the flyout behavior
			if (GetVisiblePage() != null)
				observer.OnFlyoutBehaviorChanged(GetEffectiveFlyoutBehavior());
		}

		void IShellController.AppearanceChanged(Element source, bool appearanceSet)
		{
			if (!appearanceSet)
			{
				// This bubbles up whenever there is an kind of structure/page change
				// So its also quite useful for checking the FlyoutBehavior conditions
				NotifyFlyoutBehaviorObservers();
			}

			// here we wish to notify every element whose "pivot line" contains the source
			// To do that we first need to find the leaf node in the line, and then walk up
			// to see if we find the source on the way up.

			// If this is not an appearanceSet event but just a structural change, then we only
			// need walk up from the source to look for the pivot as items below the change
			// can't be affected by it

			foreach (var (Observer, Pivot) in _appearanceObservers)
			{
				var observer = Observer;
				var pivot = Pivot;

				Element target;
				Element leaf;
				if (appearanceSet)
				{
					leaf = WalkToPage(pivot);
					target = source;
				}
				else
				{
					leaf = source;
					target = pivot;
				}

				while (!Application.IsApplicationOrNull(leaf))
				{
					if (leaf == target)
					{
						observer.OnAppearanceChanged(GetAppearanceForPivot(pivot));
						break;
					}

					leaf = leaf.Parent;
				}
			}
		}

		ShellNavigationState IShellController.GetNavigationState(ShellItem shellItem, ShellSection shellSection, ShellContent shellContent, bool includeStack)
			=> ShellNavigationManager.GetNavigationState(shellItem, shellSection, shellContent, includeStack ? shellSection.Stack.ToList() : null, includeStack ? shellSection.Navigation.ModalStack.ToList() : null);

		async void IShellController.OnFlyoutItemSelected(Element element)
		{
			await (this as IShellController).OnFlyoutItemSelectedAsync(element);
		}

		async Task IShellController.OnFlyoutItemSelectedAsync(Element element)
		{
			ShellItem shellItem = null;
			ShellSection shellSection = null;
			ShellContent shellContent = null;

			switch (element)
			{
				case MenuShellItem menuShellItem:
					((IMenuItemController)menuShellItem.MenuItem).Activate();
					break;
				case ShellItem i:
					shellItem = i;
					break;
				case ShellSection s:
					shellItem = s.Parent as ShellItem;
					shellSection = s;
					break;
				case ShellContent c:
					shellItem = c.Parent.Parent as ShellItem;
					shellSection = c.Parent as ShellSection;
					shellContent = c;
					break;
				case MenuItem m:
					((IMenuItemController)m).Activate();
					break;
			}

			if (shellItem == null || !shellItem.IsEnabled)
				return;

			shellSection = shellSection ?? shellItem.CurrentItem;
			shellContent = shellContent ?? shellSection?.CurrentItem;

			var state = ShellNavigationManager.GetNavigationState(shellItem, shellSection, shellContent, null, null);

			if (FlyoutIsPresented && GetEffectiveFlyoutBehavior() != FlyoutBehavior.Locked)
				SetValueFromRenderer(FlyoutIsPresentedProperty, false);

			if (shellSection == null)
				shellItem.PropertyChanged += OnShellItemPropertyChanged;
			else if (shellContent == null)
				shellSection.PropertyChanged += OnShellItemPropertyChanged;
			else
				await GoToAsync(state).ConfigureAwait(false);
		}

		void OnShellItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == CurrentItemProperty.PropertyName)
			{
				(sender as BindableObject).PropertyChanged -= OnShellItemPropertyChanged;
				if (sender is ShellItem item)
					((IShellController)this).OnFlyoutItemSelected(item);
				else if (sender is ShellSection section)
					((IShellController)this).OnFlyoutItemSelected(section.Parent);
			}
		}

		bool IShellController.ProposeNavigation(ShellNavigationSource source, ShellItem shellItem, ShellSection shellSection, ShellContent shellContent, IReadOnlyList<Page> stack, bool canCancel)
		{
			return _navigationManager.ProposeNavigationOutsideGotoAsync(source, shellItem, shellSection, shellContent, stack, canCancel);
		}

		bool IShellController.RemoveAppearanceObserver(IAppearanceObserver observer)
		{
			for (int i = 0; i < _appearanceObservers.Count; i++)
			{
				if (_appearanceObservers[i].Observer == observer)
				{
					_appearanceObservers.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		bool IShellController.RemoveFlyoutBehaviorObserver(IFlyoutBehaviorObserver observer)
			=> _flyoutBehaviorObservers.Remove(observer);

		void IShellController.UpdateCurrentState(ShellNavigationSource source)
		{
			var oldState = CurrentState;
			var shellItem = CurrentItem;
			var shellSection = shellItem?.CurrentItem;
			var shellContent = shellSection?.CurrentItem;
			var stack = shellSection?.Stack;
			var modalStack = shellSection?.Navigation?.ModalStack;
			var result = ShellNavigationManager.GetNavigationState(shellItem, shellSection, shellContent, stack, modalStack);

			SetValueFromRenderer(CurrentStatePropertyKey, result);

			_navigationManager.HandleNavigated(new ShellNavigatedEventArgs(oldState, CurrentState, source));
		}
		ReadOnlyCollection<ShellItem> IShellController.GetItems() =>
			new ReadOnlyCollection<ShellItem>(((ShellItemCollection)Items).VisibleItemsReadOnly.ToList());

		event NotifyCollectionChangedEventHandler IShellController.ItemsCollectionChanged
		{
			add { ((ShellItemCollection)Items).VisibleItemsChanged += value; }
			remove { ((ShellItemCollection)Items).VisibleItemsChanged -= value; }
		}

		public static Shell Current => Application.Current?.MainPage as Shell;

		internal ShellNavigationManager NavigationManager => _navigationManager;
		public Task GoToAsync(ShellNavigationState state)
		{
			return _navigationManager.GoToAsync(state, null, false);
		}

		public Task GoToAsync(ShellNavigationState state, bool animate)
		{
			return _navigationManager.GoToAsync(state, animate, false);
		}

		public static readonly BindableProperty CurrentItemProperty =
			BindableProperty.Create(nameof(CurrentItem), typeof(ShellItem), typeof(Shell), null, BindingMode.TwoWay,
				propertyChanging: OnCurrentItemChanging,
				propertyChanged: OnCurrentItemChanged);

		public static readonly BindableProperty CurrentStateProperty = CurrentStatePropertyKey.BindableProperty;

		public static readonly BindableProperty FlyoutBackgroundImageProperty =
			BindableProperty.Create(nameof(FlyoutBackgroundImage), typeof(ImageSource), typeof(Shell), default(ImageSource), BindingMode.OneTime);

		public static readonly BindableProperty FlyoutBackgroundImageAspectProperty =
			BindableProperty.Create(nameof(FlyoutBackgroundImageAspect), typeof(Aspect), typeof(Shell), default(Aspect), BindingMode.OneTime);

		public static readonly BindableProperty FlyoutBackgroundColorProperty =
			BindableProperty.Create(nameof(FlyoutBackgroundColor), typeof(Color), typeof(Shell), Color.Default, BindingMode.OneTime);

		public static readonly BindableProperty FlyoutBackgroundProperty =
			BindableProperty.Create(nameof(FlyoutBackground), typeof(Brush), typeof(Shell), Brush.Default, BindingMode.OneTime);

		public static readonly BindableProperty FlyoutHeaderBehaviorProperty =
			BindableProperty.Create(nameof(FlyoutHeaderBehavior), typeof(FlyoutHeaderBehavior), typeof(Shell), FlyoutHeaderBehavior.Default, BindingMode.OneTime);

		public static readonly BindableProperty FlyoutHeaderProperty =
			BindableProperty.Create(nameof(FlyoutHeader), typeof(object), typeof(Shell), null, BindingMode.OneTime,
				propertyChanging: OnFlyoutHeaderChanging);

		public static readonly BindableProperty FlyoutFooterProperty =
			BindableProperty.Create(nameof(FlyoutFooter), typeof(object), typeof(Shell), null, BindingMode.OneTime,
				propertyChanging: OnFlyoutFooterChanging);

		public static readonly BindableProperty FlyoutHeaderTemplateProperty =
			BindableProperty.Create(nameof(FlyoutHeaderTemplate), typeof(DataTemplate), typeof(Shell), null, BindingMode.OneTime,
				propertyChanging: OnFlyoutHeaderTemplateChanging);

		public static readonly BindableProperty FlyoutFooterTemplateProperty =
			BindableProperty.Create(nameof(FlyoutFooterTemplate), typeof(DataTemplate), typeof(Shell), null, BindingMode.OneTime,
				propertyChanging: OnFlyoutFooterTemplateChanging);

		public static readonly BindableProperty FlyoutIsPresentedProperty =
			BindableProperty.Create(nameof(FlyoutIsPresented), typeof(bool), typeof(Shell), false, BindingMode.TwoWay);

		public static readonly BindableProperty ItemsProperty = ItemsPropertyKey.BindableProperty;

		public static readonly BindableProperty FlyoutIconProperty =
			BindableProperty.Create(nameof(FlyoutIcon), typeof(ImageSource), typeof(Shell), null);

		public static readonly BindableProperty FlyoutVerticalScrollModeProperty =
			BindableProperty.Create(nameof(FlyoutVerticalScrollMode), typeof(ScrollMode), typeof(Shell), ScrollMode.Auto);

		View _flyoutHeaderView;
		View _flyoutFooterView;
		List<List<Element>> _currentFlyoutViews;
		ShellNavigationManager _navigationManager;

		public Shell()
		{
			_navigationManager = new ShellNavigationManager(this);
			_navigationManager.Navigated += (_, args) =>
			{
				OnNavigated(args);
				Navigated?.Invoke(this, args);
			};

			_navigationManager.Navigating += (_, args) =>
			{
				Navigating?.Invoke(this, args);
				OnNavigating(args);
			};

			Navigation = new NavigationImpl(this);
			Route = Routing.GenerateImplicitRoute("shell");
			Initialize();
		}

		void Initialize()
		{
			if (CurrentItem != null)
				SetCurrentItem();

			((ShellElementCollection)Items).VisibleItemsChangedInternal += (s, e) =>
			{
				SetCurrentItem();
				SendStructureChanged();
			};

			async void SetCurrentItem()
			{
				var shellItems = ShellController.GetItems();

				if (CurrentItem != null && shellItems.Contains(CurrentItem))
					return;

				ShellItem shellItem = null;

				// If shell item has been removed try to renavigate to current location
				// Just in case the item was replaced. This is mainly relevant for hot reload
				if (CurrentItem != null)
				{
					try
					{
						var location = CurrentState.Location;
						var navRequest = ShellUriHandler.GetNavigationRequest(this, ((ShellNavigationState)location).FullLocation, false, false);

						if (navRequest != null)
						{
							var item = navRequest.Request.Item;
							var section = navRequest.Request.Section;
							var Content = navRequest.Request.Content;

							if (IsValidRoute(item) && IsValidRoute(section) && IsValidRoute(Content))
							{
								await GoToAsync(location, false);
								return;
							}

							bool IsValidRoute(BaseShellItem baseShellItem)
							{
								if (baseShellItem == null)
									return true;

								if (!baseShellItem.IsVisible)
									return false;

								return baseShellItem.IsPartOfVisibleTree();
							}
						}
					}
					catch (Exception exc)
					{
						Log.Warning(nameof(Shell), $"If you're using hot reload add a route to everything in your shell file:  {exc}");
					}
				}

				if (shellItem == null)
				{
					foreach (var item in shellItems)
					{
						if (item is ShellItem && ValidDefaultShellItem(item))
						{
							shellItem = item;
							break;
						}
					}
				}

				if (shellItem != null)
					ShellController.OnFlyoutItemSelected(shellItem);
			}
		}

		public ScrollMode FlyoutVerticalScrollMode
		{
			get => (ScrollMode)GetValue(FlyoutVerticalScrollModeProperty);
			set => SetValue(FlyoutVerticalScrollModeProperty, value);
		}

		public event EventHandler<ShellNavigatedEventArgs> Navigated;
		public event EventHandler<ShellNavigatingEventArgs> Navigating;


		public ImageSource FlyoutIcon
		{
			get => (ImageSource)GetValue(FlyoutIconProperty);
			set => SetValue(FlyoutIconProperty, value);
		}

		public ShellItem CurrentItem
		{
			get => (ShellItem)GetValue(CurrentItemProperty);
			set => SetValue(CurrentItemProperty, value);
		}

		internal ShellContent CurrentContent => CurrentItem?.CurrentItem?.CurrentItem;
		internal ShellSection CurrentSection => CurrentItem?.CurrentItem;

		public ShellNavigationState CurrentState => (ShellNavigationState)GetValue(CurrentStateProperty);

		[TypeConverter(typeof(ImageSourceConverter))]
		public ImageSource FlyoutBackgroundImage
		{
			get => (ImageSource)GetValue(FlyoutBackgroundImageProperty);
			set => SetValue(FlyoutBackgroundImageProperty, value);
		}

		public Aspect FlyoutBackgroundImageAspect
		{
			get => (Aspect)GetValue(FlyoutBackgroundImageAspectProperty);
			set => SetValue(FlyoutBackgroundImageAspectProperty, value);
		}

		public Color FlyoutBackgroundColor
		{
			get => (Color)GetValue(FlyoutBackgroundColorProperty);
			set => SetValue(FlyoutBackgroundColorProperty, value);
		}

		public Brush FlyoutBackground
		{
			get => (Brush)GetValue(FlyoutBackgroundProperty);
			set => SetValue(FlyoutBackgroundProperty, value);
		}

		public Brush FlyoutBackdrop
		{
			get => (Brush)GetValue(FlyoutBackdropProperty);
			set => SetValue(FlyoutBackdropProperty, value);
		}

		public double FlyoutWidth
		{
			get => (double)GetValue(FlyoutWidthProperty);
			set => SetValue(FlyoutWidthProperty, value);
		}

		public double FlyoutHeight
		{
			get => (double)GetValue(FlyoutHeightProperty);
			set => SetValue(FlyoutHeightProperty, value);
		}

		public FlyoutBehavior FlyoutBehavior
		{
			get => (FlyoutBehavior)GetValue(FlyoutBehaviorProperty);
			set => SetValue(FlyoutBehaviorProperty, value);
		}

		public object FlyoutHeader
		{
			get => GetValue(FlyoutHeaderProperty);
			set => SetValue(FlyoutHeaderProperty, value);
		}

		public object FlyoutFooter
		{
			get => GetValue(FlyoutFooterProperty);
			set => SetValue(FlyoutFooterProperty, value);
		}

		public FlyoutHeaderBehavior FlyoutHeaderBehavior
		{
			get => (FlyoutHeaderBehavior)GetValue(FlyoutHeaderBehaviorProperty);
			set => SetValue(FlyoutHeaderBehaviorProperty, value);
		}

		public DataTemplate FlyoutHeaderTemplate
		{
			get => (DataTemplate)GetValue(FlyoutHeaderTemplateProperty);
			set => SetValue(FlyoutHeaderTemplateProperty, value);
		}

		public DataTemplate FlyoutFooterTemplate
		{
			get => (DataTemplate)GetValue(FlyoutFooterTemplateProperty);
			set => SetValue(FlyoutFooterTemplateProperty, value);
		}

		public bool FlyoutIsPresented
		{
			get => (bool)GetValue(FlyoutIsPresentedProperty);
			set => SetValue(FlyoutIsPresentedProperty, value);
		}

		public IList<ShellItem> Items => (IList<ShellItem>)GetValue(ItemsProperty);

		public DataTemplate ItemTemplate
		{
			get => GetItemTemplate(this);
			set => SetItemTemplate(this, value);
		}

		public DataTemplate MenuItemTemplate
		{
			get => GetMenuItemTemplate(this);
			set => SetMenuItemTemplate(this, value);
		}

		internal string Route
		{
			get => Routing.GetRoute(this);
			set => Routing.SetRoute(this, value);
		}

		internal string RouteHost { get; set; } = "shell";

		internal string RouteScheme { get; set; } = "app";

		View FlyoutHeaderView
		{
			get => _flyoutHeaderView;
			set
			{
				if (_flyoutHeaderView == value)
					return;

				if (_flyoutHeaderView != null)
					OnChildRemoved(_flyoutHeaderView, -1);
				_flyoutHeaderView = value;
				if (_flyoutHeaderView != null)
					OnChildAdded(_flyoutHeaderView);
			}
		}

		View FlyoutFooterView
		{
			get => _flyoutFooterView;
			set
			{
				if (_flyoutFooterView == value)
					return;

				if (_flyoutFooterView != null)
					OnChildRemoved(_flyoutFooterView, -1);
				_flyoutFooterView = value;
				if (_flyoutFooterView != null)
					OnChildAdded(_flyoutFooterView);
			}
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			if (FlyoutHeaderView != null)
				SetInheritedBindingContext(FlyoutHeaderView, BindingContext);

			if (FlyoutFooterView != null)
				SetInheritedBindingContext(FlyoutFooterView, BindingContext);

			if (FlyoutFooterView != null)
				SetInheritedBindingContext(FlyoutContentView, BindingContext);
		}

		List<List<Element>> IShellController.GenerateFlyoutGrouping()
		{
			// The idea here is to create grouping such that the Flyout would
			// render correctly if it renderered each item in the groups in order
			// but put a line between the groups. This is needed because our grouping can
			// actually go 3 layers deep.

			// Ideally this lets us control where lines are drawn in the core code
			// just by changing how we generate these groupings

			var result = new List<List<Element>>();

			var currentGroup = new List<Element>();

			foreach (var shellItem in ShellController.GetItems())
			{
				if (!ShowInFlyoutMenu(shellItem))
					continue;

				if (Routing.IsImplicit(shellItem) || shellItem.FlyoutDisplayOptions == FlyoutDisplayOptions.AsMultipleItems)
				{
					if (shellItem.FlyoutDisplayOptions == FlyoutDisplayOptions.AsMultipleItems)
						IncrementGroup();

					foreach (var shellSection in (shellItem as IShellItemController).GetItems())
					{
						if (!ShowInFlyoutMenu(shellSection))
							continue;

						var shellContents = ((IShellSectionController)shellSection).GetItems();
						if (Routing.IsImplicit(shellSection) || shellSection.FlyoutDisplayOptions == FlyoutDisplayOptions.AsMultipleItems)
						{
							foreach (var shellContent in shellContents)
							{
								if (!ShowInFlyoutMenu(shellContent))
									continue;

								currentGroup.Add(shellContent);
								if (shellContent == shellSection.CurrentItem)
								{
									AddMenuItems(shellContent.MenuItems);
								}
							}

							if (shellSection.FlyoutDisplayOptions == FlyoutDisplayOptions.AsMultipleItems)
								IncrementGroup();
						}
						else
						{
							if (!(shellSection.Parent is TabBar))
							{
								if (Routing.IsImplicit(shellSection) && shellContents.Count == 1)
								{
									if (!ShowInFlyoutMenu(shellContents[0]))
										continue;

									currentGroup.Add(shellContents[0]);
								}
								else
									currentGroup.Add(shellSection);
							}

							// If we have only a single child we will also show the items menu items
							if (shellContents.Count == 1 && shellSection == shellItem.CurrentItem && shellSection.CurrentItem.MenuItems.Count > 0)
							{
								AddMenuItems(shellSection.CurrentItem.MenuItems);
							}
						}
					}

					if (shellItem.FlyoutDisplayOptions == FlyoutDisplayOptions.AsMultipleItems)
						IncrementGroup();
				}
				else
				{
					if (!(shellItem is TabBar))
						currentGroup.Add(shellItem);
				}
			}

			IncrementGroup();

			// If the flyout groupings haven't changed just return
			// the same instance so the caller knows it hasn't changed
			// at a later point this will all get converted to an observable collection
			if (_currentFlyoutViews?.Count == result.Count)
			{
				bool hasChanged = false;
				for (var i = 0; i < result.Count && !hasChanged; i++)
				{
					var topLevelNew = result[i];
					var topLevelPrevious = _currentFlyoutViews[i];

					if (topLevelNew.Count != topLevelPrevious.Count)
					{
						hasChanged = true;
						break;
					}

					for (var j = 0; j > topLevelNew.Count; j++)
					{
						if (topLevelNew[j] != topLevelPrevious[j])
						{
							hasChanged = true;
							break;
						}
					}

				}

				if (!hasChanged)
					return _currentFlyoutViews;
			}

			_currentFlyoutViews = result;
			return result;

			bool ShowInFlyoutMenu(BindableObject bo)
			{
				if (bo is MenuShellItem msi)
					return Shell.GetFlyoutItemIsVisible(msi.MenuItem);

				return Shell.GetFlyoutItemIsVisible(bo);
			}

			void AddMenuItems(MenuItemCollection menuItems)
			{
				foreach (var item in menuItems)
				{
					if (ShowInFlyoutMenu(item))
						currentGroup.Add(item);
				}
			}

			void IncrementGroup()
			{
				if (currentGroup.Count > 0)
				{
					result.Add(currentGroup);
					currentGroup = new List<Element>();
				}
			}
		}

		internal void SendStructureChanged()
		{
			UpdateChecked(this);
			_structureChanged?.Invoke(this, EventArgs.Empty);
		}

		protected override bool OnBackButtonPressed()
		{
			if (GetVisiblePage() is Page page && page.SendBackButtonPressed())
				return true;

			var currentContent = CurrentItem?.CurrentItem;
			if (currentContent != null && currentContent.Stack.Count > 1)
			{
				NavigationPop();
				return true;
			}

			var args = new ShellNavigatingEventArgs(this.CurrentState, "", ShellNavigationSource.Pop, true);
			_navigationManager.HandleNavigating(args);
			return args.Cancelled;

			async void NavigationPop()
			{
				try
				{
					await currentContent.Navigation.PopAsync();
				}
				catch (Exception exc)
				{
					Internals.Log.Warning(nameof(Shell), $"Failed to Navigate Back: {exc}");
				}
			}
		}

		bool ValidDefaultShellItem(Element child) => !(child is MenuShellItem);

		internal override IEnumerable<Element> ChildrenNotDrawnByThisElement
		{
			get
			{
				if (FlyoutHeaderView != null)
					yield return FlyoutHeaderView;

				if (FlyoutFooterView != null)
					yield return FlyoutFooterView;
			}
		}

		protected virtual void OnNavigated(ShellNavigatedEventArgs args)
		{
		}

		protected virtual void OnNavigating(ShellNavigatingEventArgs args)
		{

		}

		static void OnCurrentItemChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (oldValue is ShellItem oldShellItem)
				oldShellItem.SendDisappearing();

			if (newValue == null)
				return;

			if (newValue is ShellItem newShellItem)
				newShellItem.SendAppearing();

			var shell = (Shell)bindable;
			UpdateChecked(shell);

			shell.ShellController.AppearanceChanged(shell, false);
			shell.ShellController.UpdateCurrentState(ShellNavigationSource.ShellItemChanged);
		}

		static void OnCurrentItemChanging(BindableObject bindable, object oldValue, object newValue)
		{
			var shell = (Shell)bindable;
			var shellItem = (ShellItem)newValue;

			if (!shell.Items.Contains(shellItem))
				shell.Items.Add(shellItem);
			
			var shellSection = shellItem.CurrentItem;
			var shellContent = shellSection.CurrentItem;
			var stack = shellSection.Stack;
			shell._navigationManager.ProposeNavigationOutsideGotoAsync(ShellNavigationSource.ShellItemChanged, shellItem, shellSection, shellContent, stack, false);
		}

		static void UpdateChecked(Element root, bool isChecked = true)
		{
			if (root is BaseShellItem baseItem)
			{
				if (!isChecked && !baseItem.IsChecked)
					return;
				baseItem.SetValue(BaseShellItem.IsCheckedPropertyKey, isChecked);
			}

			if (root is Shell shell)
			{
				ShellItem currentItem = shell.CurrentItem;
				var items = shell.ShellController.GetItems();
				var count = items.Count;
				for (int i = 0; i < count; i++)
				{
					ShellItem item = items[i];
					UpdateChecked(item, isChecked && item == currentItem);
				}
			}
			else if (root is ShellItem shellItem)
			{
				var currentItem = shellItem.CurrentItem;
				var items = (shellItem as IShellItemController).GetItems();
				var count = items.Count;
				for (int i = 0; i < count; i++)
				{
					ShellSection item = items[i];
					UpdateChecked(item, isChecked && item == currentItem);
				}
			}
			else if (root is ShellSection shellSection)
			{
				var currentItem = shellSection.CurrentItem;
				var items = (shellSection as IShellSectionController).GetItems();
				var count = items.Count;
				for (int i = 0; i < count; i++)
				{
					ShellContent item = items[i];
					UpdateChecked(item, isChecked && item == currentItem);
				}
			}
		}

		static void OnFlyoutHeaderChanging(BindableObject bindable, object oldValue, object newValue)
		{
			var shell = (Shell)bindable;
			shell.OnFlyoutHeaderChanged(oldValue, newValue);
		}

		static void OnFlyoutFooterChanging(BindableObject bindable, object oldValue, object newValue)
		{
			var shell = (Shell)bindable;
			shell.OnFlyoutFooterChanged(oldValue, newValue);
		}

		static void OnFlyoutHeaderTemplateChanging(BindableObject bindable, object oldValue, object newValue)
		{
			var shell = (Shell)bindable;
			shell.OnFlyoutHeaderTemplateChanged((DataTemplate)oldValue, (DataTemplate)newValue);
		}

		static void OnFlyoutFooterTemplateChanging(BindableObject bindable, object oldValue, object newValue)
		{
			var shell = (Shell)bindable;
			shell.OnFlyoutFooterTemplateChanged((DataTemplate)oldValue, (DataTemplate)newValue);
		}

		static void OnTitleViewChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (!(bindable is Element owner))
				return;

			var oldView = (View)oldValue;
			var newView = (View)newValue;

			if (oldView != null)
				oldView.Parent = null;

			if (newView != null)
				newView.Parent = owner;
		}

		internal FlyoutBehavior GetEffectiveFlyoutBehavior()
		{
			ShellItem rootItem = null;
			return GetEffectiveValue(Shell.FlyoutBehaviorProperty,
				() =>
				{
					if (this.IsSet(FlyoutBehaviorProperty))
						return FlyoutBehavior;

					if (rootItem is FlyoutItem)
						return FlyoutBehavior.Flyout;
					else if (rootItem is TabBar)
						return FlyoutBehavior.Disabled;

					return FlyoutBehavior;
				},
				(o) => rootItem = rootItem ?? o as ShellItem);
		}

		internal T GetEffectiveValue<T>(BindableProperty property, T defaultValue)
		{
			return GetEffectiveValue<T>(property, () => defaultValue, null);
		}

		internal T GetEffectiveValue<T>(BindableProperty property, Func<T> defaultValue, Action<Element> observer, Element element = null)
		{
			element = element ?? GetVisiblePage() ?? CurrentContent;
			while (element != this && element != null)
			{
				observer?.Invoke(element);

				if (element.IsSet(property))
					return (T)element.GetValue(property);

				element = element.Parent;
			}

			if (defaultValue == null)
				return default(T);

			return defaultValue();
		}

		ShellAppearance GetAppearanceForPivot(Element pivot)
		{
			// this algorithm is pretty simple
			// 1) Get the "CurrentPage" by walking down from the pivot
			//		Walking down goes Shell -> ShellItem -> ShellContent -> ShellContent.Stack.Last
			// 2) Walk up from the pivot to the root Shell. Stop walking as soon as you find a ShellAppearance and return
			// 3) If nothing found, return null

			pivot = WalkToPage(pivot);

			bool foundShellContent = false;
			bool anySet = false;
			ShellAppearance result = new ShellAppearance();
			// Now we walk up
			while (!Application.IsApplicationOrNull(pivot))
			{
				if (pivot is ShellContent)
					foundShellContent = true;

				// One minor deviation here. Even though a pushed page is technically the child of
				// a ShellSection and not the ShellContent, we want the root ShellContent to 
				// be taken into account. Yes this could behave oddly if the developer switches
				// tabs while a page is pushed, however that is in the developers wheelhouse
				// and this will be the generally expected behavior.
				if (!foundShellContent && pivot is ShellSection shellSection && shellSection.CurrentItem != null)
				{
					if (result.Ingest(shellSection.CurrentItem))
						anySet = true;
				}

				if (result.Ingest(pivot))
					anySet = true;

				pivot = pivot.Parent;
			}

			if (anySet)
			{
				result.MakeComplete();
				return result;
			}
			return null;
		}

		void NotifyFlyoutBehaviorObservers()
		{
			if (CurrentItem == null || GetVisiblePage() == null)
				return;

			var behavior = GetEffectiveFlyoutBehavior();
			for (int i = 0; i < _flyoutBehaviorObservers.Count; i++)
				_flyoutBehaviorObservers[i].OnFlyoutBehaviorChanged(behavior);
		}

		void OnFlyoutHeaderChanged(object oldVal, object newVal)
		{
			if (FlyoutHeaderTemplate == null)
			{
				if (newVal is View newFlyoutHeader)
					FlyoutHeaderView = newFlyoutHeader;
				else
					FlyoutHeaderView = null;
			}
		}

		void OnFlyoutHeaderTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
		{
			if (newValue == null)
			{
				if (FlyoutHeader is View flyoutHeaderView)
					FlyoutHeaderView = flyoutHeaderView;
				else
					FlyoutHeaderView = null;
			}
			else
			{
				var newHeaderView = (View)newValue.CreateContent(FlyoutHeader, this);
				FlyoutHeaderView = newHeaderView;
			}
		}

		void OnFlyoutFooterChanged(object oldVal, object newVal)
		{
			if (FlyoutFooterTemplate == null)
			{
				if (newVal is View newFlyoutFooter)
					FlyoutFooterView = newFlyoutFooter;
				else
					FlyoutFooterView = null;
			}
		}

		void OnFlyoutFooterTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
		{
			if (newValue == null)
			{
				if (FlyoutFooter is View flyoutFooterView)
					FlyoutFooterView = flyoutFooterView;
				else
					FlyoutFooterView = null;
			}
			else
			{
				var newFooterView = (View)newValue.CreateContent(FlyoutFooter, this);
				FlyoutFooterView = newFooterView;
			}
		}

		internal Element GetVisiblePage()
		{
			if (CurrentItem?.CurrentItem is IShellSectionController scc)
				return scc.PresentedPage;

			return null;
		}

		Element WalkToPage(Element element)
		{
			switch (element)
			{
				case Shell shell:
					element = shell.CurrentItem;
					break;
				case ShellItem shellItem:
					element = shellItem.CurrentItem;
					break;
				case ShellSection shellSection:
					var controller = (IShellSectionController)element;
					// this is the same as .Last but easier and will add in the root if not null
					// it generally wont be null but this is just in case
					element = controller.PresentedPage ?? element;
					break;
			}

			return element;
		}

		void IPropertyPropagationController.PropagatePropertyChanged(string propertyName)
		{
			PropertyPropagationExtensions.PropagatePropertyChanged(propertyName, this, LogicalChildren);
			if (FlyoutHeaderView != null)
				PropertyPropagationExtensions.PropagatePropertyChanged(propertyName, this, new[] { FlyoutHeaderView });
			if (FlyoutFooterView != null)
				PropertyPropagationExtensions.PropagatePropertyChanged(propertyName, this, new[] { FlyoutFooterView });
		}


		#region Shell Flyout Content


		public static readonly BindableProperty FlyoutContentProperty = 
			BindableProperty.Create(nameof(FlyoutContent), typeof(object), typeof(Shell), null, BindingMode.OneTime, propertyChanging: OnFlyoutContentChanging);

		public static readonly BindableProperty FlyoutContentTemplateProperty =
			BindableProperty.Create(nameof(FlyoutContentTemplate), typeof(DataTemplate), typeof(Shell), null, BindingMode.OneTime, propertyChanging: OnFlyoutContentTemplateChanging);

		View _flyoutContentView;

		public View FlyoutContent
		{
			get => (View)GetValue(FlyoutContentProperty);
			set => SetValue(FlyoutContentProperty, value);
		}

		public DataTemplate FlyoutContentTemplate
		{
			get => (DataTemplate)GetValue(FlyoutContentTemplateProperty);
			set => SetValue(FlyoutContentTemplateProperty, value);
		}

		View FlyoutContentView
		{
			get => _flyoutContentView;
			set
			{
				if (_flyoutContentView == value)
					return;

				if (_flyoutContentView != null)
					OnChildRemoved(_flyoutContentView, -1);
				_flyoutContentView = value;
				if (_flyoutContentView != null)
					OnChildAdded(_flyoutContentView);
			}
		}

		void OnFlyoutContentChanged(object oldVal, object newVal)
		{
			if (FlyoutContentTemplate == null)
			{
				if (newVal is View newFlyoutContent)
					FlyoutContentView = newFlyoutContent;
				else
					FlyoutContentView = null;
			}
		}

		void OnFlyoutContentTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
		{
			if (newValue == null)
			{
				if (FlyoutContent is View flyoutContentView)
					FlyoutContentView = flyoutContentView;
				else
					FlyoutContentView = null;
			}
			else
			{
				var newContentView = (View)newValue.CreateContent(FlyoutContent, this);
				FlyoutContentView = newContentView;
			}
		}

		static void OnFlyoutContentChanging(BindableObject bindable, object oldValue, object newValue)
		{
			var shell = (Shell)bindable;
			shell.OnFlyoutContentChanged(oldValue, newValue);
		}

		static void OnFlyoutContentTemplateChanging(BindableObject bindable, object oldValue, object newValue)
		{
			var shell = (Shell)bindable;
			shell.OnFlyoutContentTemplateChanged((DataTemplate)oldValue, (DataTemplate)newValue);
		}
		#endregion


		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void VerifyShellUWPFlagEnabled(
			string constructorHint = null,
			[CallerMemberName] string memberName = "")
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(Shell), ExperimentalFlags.ShellUWPExperimental);
		}

		class NavigationImpl : NavigationProxy
		{
			readonly Shell _shell;

			NavigationProxy SectionProxy => _shell.CurrentItem?.CurrentItem?.NavigationProxy;

			public NavigationImpl(Shell shell) => _shell = shell;

			protected override IReadOnlyList<Page> GetNavigationStack() => SectionProxy?.NavigationStack;

			protected override void OnInsertPageBefore(Page page, Page before) => SectionProxy.InsertPageBefore(page, before);

			protected override Task<Page> OnPopAsync(bool animated) => SectionProxy.PopAsync(animated);

			protected override Task OnPopToRootAsync(bool animated) => SectionProxy.PopToRootAsync(animated);

			protected override Task OnPushAsync(Page page, bool animated) => SectionProxy.PushAsync(page, animated);

			protected override void OnRemovePage(Page page) => SectionProxy.RemovePage(page);

			protected override async Task<Page> OnPopModal(bool animated)
			{
				if (ModalStack.Count > 0)
					ModalStack[ModalStack.Count - 1].SendDisappearing();

				if (!_shell.CurrentItem.CurrentItem.IsPoppingModalStack)
				{
					if (ModalStack.Count > 1)
						ModalStack[ModalStack.Count - 2].SendAppearing();
				}

				var modalPopped = await base.OnPopModal(animated);

				if (ModalStack.Count == 0 && !_shell.CurrentItem.CurrentItem.IsPoppingModalStack)
					_shell.CurrentItem.SendAppearing();

				return modalPopped;
			}

			protected override async Task OnPushModal(Page modal, bool animated)
			{
				if (ModalStack.Count == 0)
					_shell.CurrentItem.SendDisappearing();

				if (!_shell.CurrentItem.CurrentItem.IsPushingModalStack)
					modal.SendAppearing();

				await base.OnPushModal(modal, animated);

				modal.NavigationProxy.Inner = new NavigationImplWrapper(modal.NavigationProxy.Inner, this);
			}

			class NavigationImplWrapper : NavigationProxy
			{
				readonly INavigation _shellProxy;

				public NavigationImplWrapper(INavigation proxy, INavigation shellProxy)
				{
					Inner = proxy;
					_shellProxy = shellProxy;

				}

				protected override Task<Page> OnPopModal(bool animated) => _shellProxy.PopModalAsync(animated);

				protected override Task OnPushModal(Page modal, bool animated) => _shellProxy.PushModalAsync(modal, animated);
			}
		}
	}
}
