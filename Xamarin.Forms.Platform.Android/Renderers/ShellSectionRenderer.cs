using Android.OS;
using Android.Runtime;
using AndroidX.Fragment.App;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using AndroidX.AppCompat.Widget;
using Android.Views;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.Android.AppCompat;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class ShellSectionRenderer : Fragment, IShellSectionRenderer, ViewPager.IOnPageChangeListener, AView.IOnClickListener, IShellObservableFragment, IAppearanceObserver
	{
		#region IOnPageChangeListener

		void ViewPager.IOnPageChangeListener.OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{
			if(!_selecting && ShellSection?.CurrentItem != null)
			{
				UpdateCurrentItem(ShellSection.CurrentItem);
			}
		}

		void ViewPager.IOnPageChangeListener.OnPageScrollStateChanged(int state)
		{
		}

		void ViewPager.IOnPageChangeListener.OnPageSelected(int position)
		{
			if (_selecting)
				return;

			// TODO : Find a way to make this cancellable
			var shellSection = ShellSection;
			var shellContent = SectionController.GetItems()[position];

			if (shellContent == shellSection.CurrentItem)
				return;

			var stack = shellSection.Stack.ToList();
			bool result = ((IShellController)_shellContext.Shell).ProposeNavigation(ShellNavigationSource.ShellContentChanged,
				(ShellItem)shellSection.Parent, shellSection, shellContent, stack, true);

			if (result)
			{
				UpdateCurrentItem(shellContent);
			}
			else if(shellSection?.CurrentItem != null)
			{
				var currentPosition = SectionController.GetItems().IndexOf(shellSection.CurrentItem);
				_selecting = true;

				// Android doesn't really appreciate you calling SetCurrentItem inside a OnPageSelected callback.
				// It wont crash but the way its programmed doesn't really anticipate re-entrancy around that method
				// and it ends up going to the wrong location. Thus we must invoke.

				Device.BeginInvokeOnMainThread(() =>
				{
					if (currentPosition < _viewPager.ChildCount && _toolbarTracker != null)
					{
						_viewPager.SetCurrentItem(currentPosition, false);
						UpdateCurrentItem(shellSection.CurrentItem);
					}

					_selecting = false;
				});
			}
		}

		void UpdateCurrentItem(ShellContent content)
		{
			if (_toolbarTracker == null)
				return;

			var page = ((IShellContentController)content).Page;
			if (page == null)
				throw new ArgumentNullException(nameof(page), "Shell Content Page is Null");

			ShellSection.SetValueFromRenderer(ShellSection.CurrentItemProperty, content);
			_toolbarTracker.Page = page;
		}

		#endregion IOnPageChangeListener

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			if (appearance == null)
				ResetAppearance();
			else
				SetAppearance(appearance);
		}

		#endregion IAppearanceObserver

		#region IOnClickListener

		void AView.IOnClickListener.OnClick(AView v)
		{
		}

		#endregion IOnClickListener

		readonly IShellContext _shellContext;
		AView _rootView;
		bool _selecting;
		TabLayout _tablayout;
		IShellTabLayoutAppearanceTracker _tabLayoutAppearanceTracker;
		Toolbar _toolbar;
		IShellToolbarAppearanceTracker _toolbarAppearanceTracker;
		IShellToolbarTracker _toolbarTracker;
		FormsViewPager _viewPager;
		bool _disposed;

		public ShellSectionRenderer(IShellContext shellContext)
		{
			_shellContext = shellContext;
		}

		public event EventHandler AnimationFinished;

		Fragment IShellObservableFragment.Fragment => this;
		public ShellSection ShellSection { get; set; }
		IShellSectionController SectionController => (IShellSectionController)ShellSection;

		public override AView OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var shellSection = ShellSection;
			if (shellSection == null)
				return null;

			if (shellSection.CurrentItem == null)
				throw new InvalidOperationException($"Content not found for active {shellSection}. Title: {shellSection.Title}. Route: {shellSection.Route}.");

			var root = inflater.Inflate(Resource.Layout.RootLayout, null).JavaCast<CoordinatorLayout>();

			_toolbar = root.FindViewById<Toolbar>(Resource.Id.main_toolbar);
			_viewPager = root.FindViewById<FormsViewPager>(Resource.Id.main_viewpager);
			_tablayout = root.FindViewById<TabLayout>(Resource.Id.main_tablayout);

			_viewPager.EnableGesture = false;

			_viewPager.AddOnPageChangeListener(this);
			_viewPager.Id = Platform.GenerateViewId();

			_viewPager.Adapter = new ShellFragmentPagerAdapter(shellSection, ChildFragmentManager);
			_viewPager.OverScrollMode = OverScrollMode.Never;

			_tablayout.SetupWithViewPager(_viewPager);

			ViewGroup tabStrip = ((ViewGroup)_tablayout.GetChildAt(0));
			tabStrip.SetClipToPadding(false);
			tabStrip.SetClipChildren(false);

			Page currentPage = null;
			int currentIndex = -1;
			var currentItem = ShellSection.CurrentItem;

			while (currentIndex < 0 && SectionController.GetItems().Count > 0 && ShellSection.CurrentItem != null)
			{
				currentItem = ShellSection.CurrentItem;
				currentPage = ((IShellContentController)shellSection.CurrentItem).GetOrCreateContent();

				// current item hasn't changed
				if(currentItem == shellSection.CurrentItem)
					currentIndex = SectionController.GetItems().IndexOf(currentItem);
			}

			_toolbarTracker = _shellContext.CreateTrackerForToolbar(_toolbar);
			_toolbarTracker.Page = currentPage;

			_viewPager.CurrentItem = currentIndex;

			if (SectionController.GetItems().Count == 1)
			{
				_tablayout.Visibility = ViewStates.Gone;
			}

			_tablayout.LayoutChange += OnTabLayoutChange;

			_tabLayoutAppearanceTracker = _shellContext.CreateTabLayoutAppearanceTracker(ShellSection);
			_toolbarAppearanceTracker = _shellContext.CreateToolbarAppearanceTracker();

			HookEvents();
			ApplyBadges();

			return _rootView = root;
		}

		void OnTabLayoutChange(object sender, AView.LayoutChangeEventArgs e)
		{
			if (_disposed)
				return;

			var items = SectionController.GetItems();
			for (int i = 0; i < _tablayout.TabCount; i++)
			{
				if (items.Count <= i)
					break;

				var tab = _tablayout.GetTabAt(i);

				if(tab.View != null)
					FastRenderers.AutomationPropertiesProvider.AccessibilitySettingsChanged(tab.View, items[i]);
			}
		}

		void Destroy()
		{
			if (_rootView != null)
			{
				UnhookEvents();

				_viewPager.RemoveOnPageChangeListener(this);
				var adapter = _viewPager.Adapter;
				_viewPager.Adapter = null;
				adapter.Dispose();

				_tablayout.LayoutChange -= OnTabLayoutChange;
				_toolbarAppearanceTracker.Dispose();
				_tabLayoutAppearanceTracker.Dispose();
				_toolbarTracker.Dispose();
				_tablayout.Dispose();
				_toolbar.Dispose();
				_viewPager.Dispose();
				_rootView.Dispose();
			}

			_toolbarAppearanceTracker = null;
			_tabLayoutAppearanceTracker = null;
			_toolbarTracker = null;
			_tablayout = null;
			_toolbar = null;
			_viewPager = null;
			_rootView = null;

		}

		// Use OnDestroy instead of OnDestroyView because OnDestroyView will be
		// called before the animation completes. This causes tons of tiny issues.
		public override void OnDestroy()
		{
			Destroy();
			base.OnDestroy();
		}
		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				Destroy();
			}
		}

		protected virtual void OnAnimationFinished(EventArgs e)
		{
			AnimationFinished?.Invoke(this, e);
		}

		protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			_tablayout.Visibility = (SectionController.GetItems().Count > 1) ? ViewStates.Visible : ViewStates.Gone;

			if (e.OldItems != null)
			{
				foreach (ShellContent shellContent in e.OldItems)
					UnhookChildEvents(shellContent);
			}

			if (e.NewItems != null)
			{
				foreach (ShellContent shellContent in e.NewItems)
				{
					ApplyBadge(shellContent);
					HookChildEvents(shellContent);
				}
			}
		}

		protected virtual void OnShellItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_rootView == null)
				return;

			if (e.PropertyName == ShellSection.CurrentItemProperty.PropertyName)
			{
				var newIndex = SectionController.GetItems().IndexOf(ShellSection.CurrentItem);

				if (SectionController.GetItems().Count != _viewPager.ChildCount)
					_viewPager.Adapter.NotifyDataSetChanged();

				if (newIndex >= 0)
				{
					_viewPager.CurrentItem = newIndex;
				}
			}
		}

		protected virtual void ResetAppearance()
		{
			_toolbarAppearanceTracker.ResetAppearance(_toolbar, _toolbarTracker);
			_tabLayoutAppearanceTracker.ResetAppearance(_tablayout);
		}

		protected virtual void SetAppearance(ShellAppearance appearance)
		{
			_toolbarAppearanceTracker.SetAppearance(_toolbar, _toolbarTracker, appearance);
			_tabLayoutAppearanceTracker.SetAppearance(_tablayout, appearance);
		}

		void HookEvents()
		{
			SectionController.ItemsCollectionChanged += OnItemsCollectionChanged;
			((IShellController)_shellContext.Shell).AddAppearanceObserver(this, ShellSection);
			ShellSection.PropertyChanged += OnShellItemPropertyChanged;

			foreach (var shellContent in ShellSection.Items)
			{
				HookChildEvents(shellContent);
			}
		}

		protected virtual void HookChildEvents(ShellContent shellContent)
		{
			shellContent.PropertyChanged += OnShellContentPropertyChanged;
		}

		void UnhookEvents()
		{
			foreach (var shellContent in ShellSection.Items)
			{
				UnhookChildEvents(shellContent);
			}

			SectionController.ItemsCollectionChanged -= OnItemsCollectionChanged;
			((IShellController)_shellContext?.Shell)?.RemoveAppearanceObserver(this);
			ShellSection.PropertyChanged -= OnShellItemPropertyChanged;
		}

		protected virtual void UnhookChildEvents(ShellContent shellContent)
		{
			shellContent.PropertyChanged -= OnShellContentPropertyChanged;
		}

		void OnShellContentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == BaseShellItem.BadgeTextProperty.PropertyName ||
				e.PropertyName == BaseShellItem.BadgeColorProperty.PropertyName ||
				e.PropertyName == BaseShellItem.BadgeTextColorProperty.PropertyName ||
				e.PropertyName == BaseShellItem.BadgeUnselectedColorProperty.PropertyName ||
				e.PropertyName == BaseShellItem.BadgeUnselectedTextColorProperty.PropertyName ||
				e.PropertyName == BaseShellItem.IsCheckedProperty.PropertyName)
			{
				ApplyBadge((ShellContent)sender);
			}
		}

		void ApplyBadges()
		{
			foreach (ShellContent shellContent in this.ShellSection.Items)
			{
				ApplyBadge(shellContent);
			}
		}

		void ApplyBadge(ShellContent shellContent)
		{
			var indexOf = this.ShellSection.Items.IndexOf(shellContent);
			var tabView = (TabLayout.TabView)((ViewGroup)_tablayout.GetChildAt(0)).GetChildAt(indexOf);

			tabView.ApplyBadge(shellContent.BadgeEffectiveColor, shellContent.BadgeText, shellContent.BadgeEffectiveTextColor);
		}
	}
}
