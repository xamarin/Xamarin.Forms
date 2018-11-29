﻿using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	public abstract class ShellItemRendererBase : Fragment, IShellItemRenderer
	{
		#region IShellItemRenderer

		Fragment IShellItemRenderer.Fragment => this;

		ShellItem IShellItemRenderer.ShellItem
		{
			get { return ShellItem; }
			set { ShellItem = value; }
		}

		public event EventHandler Destroyed;

		#endregion IShellItemRenderer

		readonly Dictionary<Element, IShellObservableFragment> _fragmentMap = new Dictionary<Element, IShellObservableFragment>();
		IShellObservableFragment _currentFragment;
		ShellSection _shellSection;
		Page _displayedPage;

		protected ShellItemRendererBase(IShellContext shellContext)
		{
			ShellContext = shellContext;
		}

		protected ShellSection ShellSection
		{
			get => _shellSection;
			set
			{
				if (_shellSection == value)
					return;

				if (_shellSection != null)
				{
					((IShellSectionController)_shellSection).RemoveDisplayedPageObserver(this);
				}

				_shellSection = value;
				if (value != null)
				{
					OnShellSectionChanged();
					((IShellSectionController)ShellSection).AddDisplayedPageObserver(this, UpdateDisplayedPage);
				}
			}
		}

		protected Page DisplayedPage
		{
			get => _displayedPage;
			set
			{
				if (_displayedPage == value)
					return;

				Page oldPage = _displayedPage;
				_displayedPage = value;
				OnDisplayedPageChanged(_displayedPage, oldPage);
			}
		}

		protected IShellContext ShellContext { get; }

		protected ShellItem ShellItem { get; private set; }

		protected virtual IShellObservableFragment CreateFragmentForPage(Page page)
		{
			return ShellContext.CreateFragmentForPage(page);
		}

		public override void OnDestroy()
		{
			base.OnDestroy();

			foreach (var item in _fragmentMap)
				item.Value.Fragment.Dispose();
			_fragmentMap.Clear();

			ShellSection = null;
			DisplayedPage = null;

			Destroyed?.Invoke(this, EventArgs.Empty);
		}

		protected abstract ViewGroup GetNavigationTarget();

		protected virtual IShellObservableFragment GetOrCreateFragmentForTab(ShellSection shellSection)
		{
			var renderer = ShellContext.CreateShellSectionRenderer(shellSection);
			renderer.ShellSection = shellSection;
			return renderer;
		}

		protected virtual Task<bool> HandleFragmentUpdate(ShellNavigationSource navSource, ShellSection shellSection, Page page, bool animated)
		{
			TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

			var stack = ShellSection.Stack;
			bool isForCurrentTab = shellSection == ShellSection;

			if (!_fragmentMap.ContainsKey(ShellSection))
			{
				_fragmentMap[ShellSection] = GetOrCreateFragmentForTab(ShellSection);
			}

			switch (navSource)
			{
				case ShellNavigationSource.Push:
				case ShellNavigationSource.Insert:
					_fragmentMap[page] = CreateFragmentForPage(page);
					if (!isForCurrentTab)
					{
						return Task.FromResult(true);
					}
					break;

				case ShellNavigationSource.Pop:
					if (_fragmentMap.TryGetValue(page, out var frag))
					{
						if (frag.Fragment.IsAdded && !isForCurrentTab)
							RemoveFragment(frag.Fragment);
						_fragmentMap.Remove(page);
					}
					if (!isForCurrentTab)
						return Task.FromResult(true);
					break;

				case ShellNavigationSource.Remove:
					if (_fragmentMap.TryGetValue(page, out var removeFragment))
					{
						if (removeFragment.Fragment.IsAdded)
							RemoveFragment(removeFragment.Fragment);
						_fragmentMap.Remove(page);
					}
					return Task.FromResult(true);

				case ShellNavigationSource.PopToRoot:
					RemoveAllPushedPages(shellSection, isForCurrentTab);
					if (!isForCurrentTab)
						return Task.FromResult(true);
					break;

				case ShellNavigationSource.ShellSectionChanged:
					// We need to handle this after we know what the target is
					// because we might accidentally remove an already added target.
					// Then there would be two transactions in a row, one removing and one adding
					// the same fragement and things get really screwy when you do that.
					break;

				default:
					throw new InvalidOperationException("Unexpected navigation type");
			}

			Element targetElement = null;
			IShellObservableFragment target = null;
			if (stack.Count == 1 || navSource == ShellNavigationSource.PopToRoot)
			{
				target = _fragmentMap[ShellSection];
				targetElement = ShellSection;
			}
			else
			{
				targetElement = stack[stack.Count - 1];
				target = _fragmentMap[targetElement];
			}

			// Down here because of comment above
			if (navSource == ShellNavigationSource.ShellSectionChanged)
				RemoveAllButCurrent(target.Fragment);

			if (target == _currentFragment)
				return Task.FromResult(true);

			var t = ChildFragmentManager.BeginTransaction();

			if (animated)
				SetupAnimation(navSource, t, page);

			IShellObservableFragment trackFragment = null;
			switch (navSource)
			{
				case ShellNavigationSource.Push:
					trackFragment = target;

					if (_currentFragment != null)
						t.Hide(_currentFragment.Fragment);

					if (!target.Fragment.IsAdded)
						t.Add(GetNavigationTarget().Id, target.Fragment);
					t.Show(target.Fragment);
					break;

				case ShellNavigationSource.Pop:
				case ShellNavigationSource.PopToRoot:
				case ShellNavigationSource.ShellSectionChanged:
					trackFragment = _currentFragment;

					if (_currentFragment != null)
						t.Remove(_currentFragment.Fragment);

					if (!target.Fragment.IsAdded)
						t.Add(GetNavigationTarget().Id, target.Fragment);
					t.Show(target.Fragment);
					break;
			}

			if (animated && trackFragment != null)
			{
				GetNavigationTarget().SetBackgroundColor(Color.Black.ToAndroid());
				void callback(object s, EventArgs e)
				{
					trackFragment.AnimationFinished -= callback;
					result.TrySetResult(true);
					GetNavigationTarget().SetBackground(null);
				}
				trackFragment.AnimationFinished += callback;
			}
			else
			{
				result.TrySetResult(true);
			}

			t.CommitAllowingStateLoss();

			_currentFragment = target;


			return result.Task;
		}

		protected virtual void HookEvents(ShellItem shellItem)
		{
			shellItem.PropertyChanged += OnShellItemPropertyChanged;
			((INotifyCollectionChanged)shellItem.Items).CollectionChanged += OnShellItemsChanged;
			ShellSection = shellItem.CurrentItem;

			foreach (var shellContent in shellItem.Items)
			{
				HookChildEvents(shellContent);
			}
		}

		protected virtual void HookChildEvents(ShellSection shellSection)
		{
			((IShellSectionController)shellSection).NavigationRequested += OnNavigationRequested;
			shellSection.PropertyChanged += OnShellSectionPropertyChanged;
		}

		protected virtual void OnShellSectionChanged()
		{
			HandleFragmentUpdate(ShellNavigationSource.ShellSectionChanged, ShellSection, null, false);
		}

		protected virtual void OnDisplayedPageChanged(Page newPage, Page oldPage)
		{

		}

		protected virtual void OnNavigationRequested(object sender, NavigationRequestedEventArgs e)
		{
			e.Task = HandleFragmentUpdate((ShellNavigationSource)e.RequestType, (ShellSection)sender, e.Page, e.Animated);
		}

		protected virtual void OnShellItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ShellItem.CurrentItemProperty.PropertyName)
				ShellSection = ShellItem.CurrentItem;
		}

		protected virtual void OnShellItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (ShellSection shellSection in e.OldItems)
					UnhookChildEvents(shellSection);
			}

			if (e.NewItems != null)
			{
				foreach (ShellSection shellSection in e.NewItems)
					HookChildEvents(shellSection);
			}
		}

		protected virtual void SetupAnimation(ShellNavigationSource navSource, FragmentTransaction t, Page page)
		{
			switch (navSource)
			{
				case ShellNavigationSource.Push:
					t.SetCustomAnimations(Resource.Animation.EnterFromRight, Resource.Animation.ExitToLeft);
					break;

				case ShellNavigationSource.Pop:
				case ShellNavigationSource.PopToRoot:
					t.SetCustomAnimations(Resource.Animation.EnterFromLeft, Resource.Animation.ExitToRight);
					break;

				case ShellNavigationSource.ShellSectionChanged:
					break;
			}
		}

		protected virtual void UnhookEvents(ShellItem shellItem)
		{
			foreach (var shellSection in shellItem.Items)
			{
				UnhookChildEvents(shellSection);
			}

			((INotifyCollectionChanged)shellItem.Items).CollectionChanged -= OnShellItemsChanged;
			ShellItem.PropertyChanged -= OnShellItemPropertyChanged;
			ShellSection = null;
		}

		protected virtual void UnhookChildEvents(ShellSection shellSection)
		{
			((IShellSectionController)shellSection).NavigationRequested -= OnNavigationRequested;
			shellSection.PropertyChanged -= OnShellSectionPropertyChanged;
		}

		protected virtual void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		void UpdateDisplayedPage(Page page)
		{
			DisplayedPage = page;
		}

		void RemoveAllButCurrent(Fragment skip)
		{
			var trans = ChildFragmentManager.BeginTransaction();
			foreach (var kvp in _fragmentMap)
			{
				var f = kvp.Value.Fragment;
				if (kvp.Value == _currentFragment || kvp.Value.Fragment == skip || !f.IsAdded)
					continue;
				trans.Remove(f);
			};
			trans.CommitAllowingStateLoss();
		}

		void RemoveAllPushedPages(ShellSection shellSection, bool keepCurrent)
		{
			if (shellSection.Stack.Count <= 1 || (keepCurrent && shellSection.Stack.Count == 2))
				return;

			var t = ChildFragmentManager.BeginTransaction();

			foreach (var kvp in _fragmentMap.ToList())
			{
				if (kvp.Key.Parent != shellSection)
					continue;

				_fragmentMap.Remove(kvp.Key);

				if (keepCurrent && kvp.Value.Fragment == _currentFragment)
					continue;

				t.Remove(kvp.Value.Fragment);
			}

			t.CommitAllowingStateLoss();
		}

		void RemoveFragment(Fragment fragment)
		{
			var t = ChildFragmentManager.BeginTransaction();
			t.Remove(fragment);
			t.CommitAllowingStateLoss();
		}
	}
}