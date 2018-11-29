﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	[ContentProperty("Items")]
	public class ShellSection : ShellGroupItem, IShellSectionController, IPropertyPropagationController
	{
		#region PropertyKeys

		static readonly BindablePropertyKey ItemsPropertyKey =
			BindableProperty.CreateReadOnly(nameof(Items), typeof(ShellContentCollection), typeof(ShellSection), null,
				defaultValueCreator: bo => new ShellContentCollection());

		#endregion PropertyKeys

		#region IShellSectionController

		readonly List<(object Observer, Action<Page> Callback)> _displayedPageObservers =
			new List<(object Observer, Action<Page> Callback)>();
		readonly List<IShellContentInsetObserver> _observers = new List<IShellContentInsetObserver>();
		Thickness _lastInset;
		double _lastTabThickness;

		event EventHandler<NavigationRequestedEventArgs> IShellSectionController.NavigationRequested
		{
			add { _navigationRequested += value; }
			remove { _navigationRequested -= value; }
		}

		event EventHandler<NavigationRequestedEventArgs> _navigationRequested;

		Page IShellSectionController.PresentedPage
		{
			get
			{
				if (_navStack.Count > 1)
					return _navStack[_navStack.Count - 1];
				return ((IShellContentController)CurrentItem).Page;
			}
		}

		void IShellSectionController.AddContentInsetObserver(IShellContentInsetObserver observer)
		{
			if (!_observers.Contains(observer))
				_observers.Add(observer);

			observer.OnInsetChanged(_lastInset, _lastTabThickness);
		}

		void IShellSectionController.AddDisplayedPageObserver(object observer, Action<Page> callback)
		{
			_displayedPageObservers.Add((observer, callback));
			callback(DisplayedPage);
		}

		Task IShellSectionController.GoToPart(List<string> parts, Dictionary<string, string> queryData)
		{
			var shellContentRoute = parts[0];

			var items = Items;
			for (int i = 0; i < items.Count; i++)
			{
				ShellContent shellContent = items[i];
				if (Routing.CompareRoutes(shellContent.Route, shellContentRoute, out var isImplicit))
				{
					Shell.ApplyQueryAttributes(shellContent, queryData, parts.Count == 1);

					if (CurrentItem != shellContent)
						SetValueFromRenderer(CurrentItemProperty, shellContent);

					if (!isImplicit)
						parts.RemoveAt(0);
					if (parts.Count > 0)
					{
						return GoToAsync(parts, queryData, false);
					}
					break;
				}
			}

			return Task.FromResult(true);
		}

		bool IShellSectionController.RemoveContentInsetObserver(IShellContentInsetObserver observer)
		{
			return _observers.Remove(observer);
		}

		bool IShellSectionController.RemoveDisplayedPageObserver(object observer)
		{
			foreach (var item in _displayedPageObservers)
			{
				if (item.Observer == observer)
				{
					return _displayedPageObservers.Remove(item);
				}
			}
			return false;
		}

		void IShellSectionController.SendInsetChanged(Thickness inset, double tabThickness)
		{
			foreach (var observer in _observers)
			{
				observer.OnInsetChanged(inset, tabThickness);
			}
			_lastInset = inset;
			_lastTabThickness = tabThickness;
		}

		void IShellSectionController.SendPopped()
		{
			if (_navStack.Count <= 1)
				throw new Exception("Nav Stack consistency error");

			var last = _navStack[_navStack.Count - 1];
			_navStack.Remove(last);

			RemovePage(last);

			SendUpdateCurrentState(ShellNavigationSource.Pop);
		}

		#endregion IShellSectionController

		#region IPropertyPropagationController
		void IPropertyPropagationController.PropagatePropertyChanged(string propertyName)
		{
			PropertyPropagationExtensions.PropagatePropertyChanged(propertyName, this, Items);
		}
		#endregion

		public static readonly BindableProperty CurrentItemProperty =
			BindableProperty.Create(nameof(CurrentItem), typeof(ShellContent), typeof(ShellSection), null, BindingMode.TwoWay,
				propertyChanged: OnCurrentItemChanged);

		public static readonly BindableProperty ItemsProperty = ItemsPropertyKey.BindableProperty;

		Page _displayedPage;
		IList<Element> _logicalChildren = new List<Element>();

		ReadOnlyCollection<Element> _logicalChildrenReadOnly;

		List<Page> _navStack = new List<Page> { null };

		public ShellSection()
		{
			((INotifyCollectionChanged)Items).CollectionChanged += ItemsCollectionChanged;
			Navigation = new NavigationImpl(this);
		}

		public ShellContent CurrentItem
		{
			get { return (ShellContent)GetValue(CurrentItemProperty); }
			set { SetValue(CurrentItemProperty, value); }
		}

		public ShellContentCollection Items => (ShellContentCollection)GetValue(ItemsProperty);

		public IReadOnlyList<Page> Stack => _navStack;

		internal override ReadOnlyCollection<Element> LogicalChildrenInternal => _logicalChildrenReadOnly ?? (_logicalChildrenReadOnly = new ReadOnlyCollection<Element>(_logicalChildren));

		Page DisplayedPage
		{
			get { return _displayedPage; }
			set
			{
				if (_displayedPage == value)
					return;
				_displayedPage = value;

				foreach (var item in _displayedPageObservers)
					item.Callback(_displayedPage);
			}
		}

		Shell Shell => Parent?.Parent as Shell;

		ShellItem ShellItem => Parent as ShellItem;

#if DEBUG
		[Obsolete("Please dont use this in core code... its SUPER hard to debug when this happens", true)]
#endif
		public static implicit operator ShellSection(ShellContent shellContent)
		{
			var shellSection = new ShellSection();

			var contentRoute = shellContent.Route;

			shellSection.Route = Routing.GenerateImplicitRoute(contentRoute);

			shellSection.Items.Add(shellContent);
			shellSection.SetBinding(TitleProperty, new Binding("Title", BindingMode.OneWay, source: shellContent));
			shellSection.SetBinding(IconProperty, new Binding("Icon", BindingMode.OneWay, source: shellContent));
			return shellSection;
		}

#if DEBUG
		[Obsolete("Please dont use this in core code... its SUPER hard to debug when this happens", true)]
#endif
		public static implicit operator ShellSection(TemplatedPage page)
		{
			return (ShellSection)(ShellContent)page;
		}

		public virtual async Task GoToAsync(List<string> routes, IDictionary<string, string> queryData, bool animate)
		{
			if (routes == null || routes.Count == 0)
			{
				await Navigation.PopToRootAsync(animate);
				return;
			}

			for (int i = 0; i < routes.Count; i++)
			{
				bool isLast = i == routes.Count - 1;
				var route = routes[i];
				var navPage = _navStack.Count > i + 1 ? _navStack[i + 1] : null;

				if (navPage != null)
				{
					if (Routing.GetRoute(navPage) == route)
					{
						Shell.ApplyQueryAttributes(navPage, queryData, isLast);
						continue;
					}

					while (_navStack.Count > i + 1)
					{
						await OnPopAsync(false);
					}
				}

				var content = Routing.GetOrCreateContent(route) as Page;
				if (content == null)
					break;

				Shell.ApplyQueryAttributes(content, queryData, isLast);
				await OnPushAsync(content, i == routes.Count - 1 && animate);
			}

			SendAppearanceChanged();
		}

		internal void SendStructureChanged()
		{
			if (Parent?.Parent is Shell shell)
			{
				shell.SendStructureChanged();
			}
		}

		protected virtual IReadOnlyList<Page> GetNavigationStack() => _navStack;

		internal void UpdateDisplayedPage()
		{
			var stack = Stack;
			if (stack.Count > 1)
			{
				DisplayedPage = stack[stack.Count - 1];
			}
			else
			{
				IShellContentController currentItem = CurrentItem;
				if (currentItem.Page != null)
					DisplayedPage = currentItem.Page;
			}

		}

		protected override void OnChildAdded(Element child)
		{
			base.OnChildAdded(child);
			if (CurrentItem == null && Items.Contains(child))
				SetValueFromRenderer(CurrentItemProperty, child);

			UpdateDisplayedPage();
		}

		protected override void OnChildRemoved(Element child)
		{
			base.OnChildRemoved(child);
			if (CurrentItem == child)
			{
				if (Items.Count == 0)
					ClearValue(CurrentItemProperty);
				else
				{
					// We want to delay invoke this because the renderer may handle this instead
					Device.BeginInvokeOnMainThread(() =>
					{
						if (CurrentItem == null)
							SetValueFromRenderer(CurrentItemProperty, Items[0]);
					});
				}
			}

			UpdateDisplayedPage();
		}

		protected virtual void OnInsertPageBefore(Page page, Page before)
		{
			var index = _navStack.IndexOf(before);
			if (index == -1)
				throw new ArgumentException("Page not found in nav stack");

			var stack = _navStack.ToList();
			stack.Insert(index, page);

			var allow = ((IShellController)Shell).ProposeNavigation(
				ShellNavigationSource.Insert,
				Parent as ShellItem,
				this,
				CurrentItem,
				stack,
				true
			);

			if (!allow)
				return;

			_navStack.Insert(index, page);
			AddPage(page);
			SendAppearanceChanged();

			var args = new NavigationRequestedEventArgs(page, before, false)
			{
				RequestType = NavigationRequestType.Insert
			};

			_navigationRequested?.Invoke(this, args);

			SendUpdateCurrentState(ShellNavigationSource.Insert);
		}

		protected async virtual Task<Page> OnPopAsync(bool animated)
		{
			if (_navStack.Count <= 1)
				throw new InvalidOperationException("Can't pop last page off stack");

			List<Page> stack = _navStack.ToList();
			stack.Remove(stack.Last());
			var allow = ((IShellController)Shell).ProposeNavigation(
				ShellNavigationSource.Pop,
				Parent as ShellItem,
				this,
				CurrentItem,
				stack,
				true
			);

			if (!allow)
				return null;

			var page = _navStack[_navStack.Count - 1];
			var args = new NavigationRequestedEventArgs(page, animated)
			{
				RequestType = NavigationRequestType.Pop
			};

			_navStack.Remove(page);
			SendAppearanceChanged();
			_navigationRequested?.Invoke(this, args);
			if (args.Task != null)
				await args.Task;
			RemovePage(page);

			SendUpdateCurrentState(ShellNavigationSource.Pop);

			return page;
		}

		protected virtual async Task OnPopToRootAsync(bool animated)
		{
			if (_navStack.Count <= 1)
				return;

			var allow = ((IShellController)Shell).ProposeNavigation(
				ShellNavigationSource.PopToRoot,
				Parent as ShellItem,
				this,
				CurrentItem,
				null,
				true
			);

			if (!allow)
				return;

			var page = _navStack[_navStack.Count - 1];
			var args = new NavigationRequestedEventArgs(page, animated)
			{
				RequestType = NavigationRequestType.PopToRoot
			};

			_navigationRequested?.Invoke(this, args);
			var oldStack = _navStack;
			_navStack = new List<Page> { null };
			SendAppearanceChanged();

			if (args.Task != null)
				await args.Task;

			for (int i = 1; i < oldStack.Count; i++)
			{
				RemovePage(oldStack[i]);
			}

			SendUpdateCurrentState(ShellNavigationSource.PopToRoot);
		}

		protected virtual Task OnPushAsync(Page page, bool animated)
		{
			List<Page> stack = _navStack.ToList();
			stack.Add(page);
			var allow = ((IShellController)Shell).ProposeNavigation(
				ShellNavigationSource.Push,
				ShellItem,
				this,
				CurrentItem,
				stack,
				true
			);

			if (!allow)
				return Task.FromResult(true);

			var args = new NavigationRequestedEventArgs(page, animated)
			{
				RequestType = NavigationRequestType.Push
			};

			_navStack.Add(page);
			AddPage(page);
			SendAppearanceChanged();
			_navigationRequested?.Invoke(this, args);

			SendUpdateCurrentState(ShellNavigationSource.Push);

			if (args.Task == null)
				return Task.FromResult(true);
			return args.Task;
		}

		protected virtual void OnRemovePage(Page page)
		{
			if (!_navStack.Contains(page))
				return;

			var stack = _navStack.ToList();
			stack.Remove(page);
			var allow = ((IShellController)Shell).ProposeNavigation(
				ShellNavigationSource.Remove,
				ShellItem,
				this,
				CurrentItem,
				stack,
				true
			);

			if (!allow)
				return;

			_navStack.Remove(page);

			SendAppearanceChanged();
			RemovePage(page);
			var args = new NavigationRequestedEventArgs(page, false)
			{
				RequestType = NavigationRequestType.Remove
			};
			_navigationRequested?.Invoke(this, args);

			SendUpdateCurrentState(ShellNavigationSource.Remove);
		}

		static void OnCurrentItemChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var shellSection = (ShellSection)bindable;

			if (shellSection.Parent?.Parent is IShellController shell)
			{
				shell.UpdateCurrentState(ShellNavigationSource.ShellSectionChanged);
			}

			shellSection.SendStructureChanged();
			((IShellController)shellSection?.Parent?.Parent)?.AppearanceChanged(shellSection, false);

			shellSection.UpdateDisplayedPage();
		}

		void AddPage(Page page)
		{
			_logicalChildren.Add(page);
			OnChildAdded(page);
		}

		void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (Element element in e.NewItems)
					OnChildAdded(element);
			}

			if (e.OldItems != null)
			{
				foreach (Element element in e.OldItems)
					OnChildRemoved(element);
			}

			SendStructureChanged();
		}

		void RemovePage(Page page)
		{
			if (_logicalChildren.Remove(page))
				OnChildRemoved(page);
		}

		void SendAppearanceChanged() => ((IShellController)Parent?.Parent)?.AppearanceChanged(this, false);

		void SendUpdateCurrentState(ShellNavigationSource source)
		{
			if (Parent?.Parent is IShellController shell)
			{
				shell?.UpdateCurrentState(source);
			}
		}

		public class NavigationImpl : NavigationProxy
		{
			readonly ShellSection _owner;

			public NavigationImpl(ShellSection owner) => _owner = owner;

			protected override IReadOnlyList<Page> GetNavigationStack() => _owner.GetNavigationStack();

			protected override void OnInsertPageBefore(Page page, Page before) => _owner.OnInsertPageBefore(page, before);

			protected override Task<Page> OnPopAsync(bool animated) => _owner.OnPopAsync(animated);

			protected override Task OnPopToRootAsync(bool animated) => _owner.OnPopToRootAsync(animated);

			protected override Task OnPushAsync(Page page, bool animated) => _owner.OnPushAsync(page, animated);

			protected override void OnRemovePage(Page page) => _owner.OnRemovePage(page);
		}
	}
}