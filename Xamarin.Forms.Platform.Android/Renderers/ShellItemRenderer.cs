﻿using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using AColor = Android.Graphics.Color;
using AView = Android.Views.View;
using ColorStateList = Android.Content.Res.ColorStateList;
using IMenu = Android.Views.IMenu;
using LP = Android.Views.ViewGroup.LayoutParams;
using Orientation = Android.Widget.Orientation;
using Typeface = Android.Graphics.Typeface;
using TypefaceStyle = Android.Graphics.TypefaceStyle;

namespace Xamarin.Forms.Platform.Android
{
	public class ShellItemRenderer : ShellItemRendererBase, BottomNavigationView.IOnNavigationItemSelectedListener, IAppearanceObserver
	{
		#region IOnNavigationItemSelectedListener

		bool BottomNavigationView.IOnNavigationItemSelectedListener.OnNavigationItemSelected(IMenuItem item)
		{
			return OnItemSelected(item);
		}

		#endregion IOnNavigationItemSelectedListener

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			if (appearance != null)
				SetAppearance(appearance);
			else
				ResetAppearance();
		}

		#endregion IAppearanceObserver

		protected const int MoreTabId = 99;
		BottomNavigationView _bottomView;
		FrameLayout _navigationArea;
		AView _outerLayout;
		IShellBottomNavViewAppearanceTracker _appearanceTracker;

		public ShellItemRenderer(IShellContext shellContext) : base(shellContext)
		{
		}

		public override AView OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);

			_outerLayout = inflater.Inflate(Resource.Layout.BottomTabLayout, null);
			_bottomView = _outerLayout.FindViewById<BottomNavigationView>(Resource.Id.bottomtab_tabbar);
			_navigationArea = _outerLayout.FindViewById<FrameLayout>(Resource.Id.bottomtab_navarea);

			_bottomView.SetBackgroundColor(Color.White.ToAndroid());
			_bottomView.SetOnNavigationItemSelectedListener(this);

			HookEvents(ShellItem);
			SetupMenu();

			_appearanceTracker = ShellContext.CreateBottomNavViewAppearanceTracker(ShellItem);
			((IShellController)ShellContext.Shell).AddAppearanceObserver(this, ShellItem);

			return _outerLayout;
		}

		// Use OnDestory become OnDestroyView may fire before events are completed.
		public override void OnDestroy()
		{
			UnhookEvents(ShellItem);
			if (_bottomView != null)
			{
				_bottomView?.SetOnNavigationItemSelectedListener(null);
				_bottomView?.Background?.Dispose();
				_bottomView?.Dispose();
				_bottomView = null;

				_navigationArea?.Dispose();
				_navigationArea = null;

				_appearanceTracker?.Dispose();
				_appearanceTracker = null;

				_outerLayout?.Dispose();
				_outerLayout = null;
			}

			((IShellController)ShellContext.Shell).RemoveAppearanceObserver(this);

			base.OnDestroy();
		}

		protected virtual void SetAppearance(ShellAppearance appearance) => _appearanceTracker.SetAppearance(_bottomView, appearance);

		protected virtual bool ChangeSection(ShellSection shellSection)
		{
			return ((IShellItemController)ShellItem).ProposeSection(shellSection);
		}

		protected virtual Drawable CreateItemBackgroundDrawable()
		{
			var stateList = ColorStateList.ValueOf(Color.Black.MultiplyAlpha(0.2).ToAndroid());
			return new RippleDrawable(stateList, new ColorDrawable(AColor.White), null);
		}

		protected virtual BottomSheetDialog CreateMoreBottomSheet(Action<ShellSection, BottomSheetDialog> selectCallback)
		{
			var bottomSheetDialog = new BottomSheetDialog(Context);
			var bottomSheetLayout = new LinearLayout(Context);
			using (var bottomShellLP = new LP(LP.MatchParent, LP.WrapContent))
				bottomSheetLayout.LayoutParameters = bottomShellLP;
			bottomSheetLayout.Orientation = Orientation.Vertical;
			// handle the more tab
			for (int i = 4; i < ShellItem.Items.Count; i++)
			{
				var shellContent = ShellItem.Items[i];

				using (var innerLayout = new LinearLayout(Context))
				{
					innerLayout.ClipToOutline = true;
					innerLayout.SetBackground(CreateItemBackgroundDrawable());
					innerLayout.SetPadding(0, (int)Context.ToPixels(6), 0, (int)Context.ToPixels(6));
					innerLayout.Orientation = Orientation.Horizontal;
					using (var param = new LP(LP.MatchParent, LP.WrapContent))
						innerLayout.LayoutParameters = param;

					// technically the unhook isn't needed
					// we dont even unhook the events that dont fire
					void clickCallback(object s, EventArgs e)
					{
						selectCallback(shellContent, bottomSheetDialog);
						if (!innerLayout.IsDisposed())
							innerLayout.Click -= clickCallback;
					}
					innerLayout.Click += clickCallback;

					var image = new ImageView(Context);
					var lp = new LinearLayout.LayoutParams((int)Context.ToPixels(32), (int)Context.ToPixels(32))
					{
						LeftMargin = (int)Context.ToPixels(20),
						RightMargin = (int)Context.ToPixels(20),
						TopMargin = (int)Context.ToPixels(6),
						BottomMargin = (int)Context.ToPixels(6),
						Gravity = GravityFlags.Center
					};
					image.LayoutParameters = lp;
					lp.Dispose();

					image.ImageTintList = ColorStateList.ValueOf(Color.Black.MultiplyAlpha(0.6).ToAndroid());
					SetImage(image, shellContent.Icon);

					innerLayout.AddView(image);

					using (var text = new TextView(Context))
					{
						text.SetTypeface(Typeface.Create("sans-serif-medium", TypefaceStyle.Normal), TypefaceStyle.Normal);
						text.SetTextColor(AColor.Black);
						text.Text = shellContent.Title;
						lp = new LinearLayout.LayoutParams(0, LP.WrapContent)
						{
							Gravity = GravityFlags.Center,
							Weight = 1
						};
						text.LayoutParameters = lp;
						lp.Dispose();

						innerLayout.AddView(text);
					}

					bottomSheetLayout.AddView(innerLayout);
				}
			}

			bottomSheetDialog.SetContentView(bottomSheetLayout);
			bottomSheetLayout.Dispose();

			return bottomSheetDialog;
		}

		protected override ViewGroup GetNavigationTarget() => _navigationArea;

		protected override void OnShellSectionChanged()
		{
			base.OnShellSectionChanged();

			var index = ShellItem.Items.IndexOf(ShellSection);
			using (var menu = _bottomView.Menu)
			{
				index = Math.Min(index, menu.Size() - 1);
				if (index < 0)
					return;
				using (var menuItem = menu.GetItem(index))
					menuItem.SetChecked(true);
			}
		}

		protected override void OnDisplayedPageChanged(Page newPage, Page oldPage)
		{
			base.OnDisplayedPageChanged(newPage, oldPage);

			if (oldPage != null)
				oldPage.PropertyChanged -= OnDisplayedElementPropertyChanged;

			if (newPage != null)
				newPage.PropertyChanged += OnDisplayedElementPropertyChanged;

			UpdateTabBarVisibility();
		}

		protected virtual bool OnItemSelected(IMenuItem item)
		{
			var id = item.ItemId;
			if (id == MoreTabId)
			{
				var bottomSheetDialog = CreateMoreBottomSheet(OnMoreItemSelected);
				bottomSheetDialog.Show();
				bottomSheetDialog.DismissEvent += OnMoreSheetDismissed;
			}
			else
			{
				var shellSection = ShellItem.Items[id];
				if (item.IsChecked)
				{
					OnTabReselected(shellSection);
				}
				else
				{
					return ChangeSection(shellSection);
				}
			}

			return true;
		}

		protected virtual void OnMoreItemSelected(ShellSection shellSection, BottomSheetDialog dialog)
		{
			ChangeSection(shellSection);

			dialog.Dismiss();
			dialog.Dispose();
		}

		protected virtual void OnMoreSheetDismissed(object sender, EventArgs e) => OnShellSectionChanged();

		protected override void OnShellItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			base.OnShellItemsChanged(sender, e);

			SetupMenu();
		}

		protected override void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnShellSectionPropertyChanged(sender, e);

			if (e.PropertyName == BaseShellItem.IsEnabledProperty.PropertyName)
			{
				var content = (ShellSection)sender;
				var index = ShellItem.Items.IndexOf(content);

				var itemCount = ShellItem.Items.Count;
				var maxItems = _bottomView.MaxItemCount;

				if (itemCount > maxItems && index > maxItems - 2)
					return;

				var menuItem = _bottomView.Menu.FindItem(index);
				UpdateShellSectionEnabled(content, menuItem);
			}
			else if (e.PropertyName == BaseShellItem.TitleProperty.PropertyName ||
				e.PropertyName == BaseShellItem.IconProperty.PropertyName)
			{
				SetupMenu();
			}
		}

		protected virtual void OnTabReselected(ShellSection shellSection)
		{
		}

		protected virtual void ResetAppearance() => _appearanceTracker.ResetAppearance(_bottomView);

		protected virtual async void SetupMenu(IMenu menu, int maxBottomItems, ShellItem shellItem)
		{
			menu.Clear();
			bool showMore = ShellItem.Items.Count > maxBottomItems;

			int end = showMore ? maxBottomItems - 1 : ShellItem.Items.Count;

			var currentIndex = shellItem.Items.IndexOf(ShellSection);

			List<IMenuItem> menuItems = new List<IMenuItem>();
			List<Task> loadTasks = new List<Task>();
			for (int i = 0; i < end; i++)
			{
				var item = shellItem.Items[i];
				using (var title = new Java.Lang.String(item.Title))
				{
					var menuItem = menu.Add(0, i, 0, title);
					menuItems.Add(menuItem);
					loadTasks.Add(SetMenuItemIcon(menuItem, item.Icon));
					UpdateShellSectionEnabled(item, menuItem);
					if (item == ShellSection)
					{
						menuItem.SetChecked(true);
					}
				}
			}

			if (showMore)
			{
				var moreString = new Java.Lang.String("More");
				var menuItem = menu.Add(0, MoreTabId, 0, moreString);
				moreString.Dispose();

				menuItem.SetIcon(Resource.Drawable.abc_ic_menu_overflow_material);
				if (currentIndex >= maxBottomItems - 1)
					menuItem.SetChecked(true);
			}

			_bottomView.Visibility = end == 1 ? ViewStates.Gone : ViewStates.Visible;

			_bottomView.SetShiftMode(false, false);

			if (loadTasks.Count > 0)
				await Task.WhenAll(loadTasks);

			foreach (var menuItem in menuItems)
				menuItem.Dispose();
		}

		protected virtual void UpdateShellSectionEnabled(ShellSection shellSection, IMenuItem menuItem)
		{
			bool tabEnabled = shellSection.IsEnabled;
			if (menuItem.IsEnabled != tabEnabled)
				menuItem.SetEnabled(tabEnabled);
		}

		void OnDisplayedElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Shell.TabBarIsVisibleProperty.PropertyName)
				UpdateTabBarVisibility();
		}

		async void SetImage(ImageView image, ImageSource source)
		{
			image.SetImageDrawable(await Context.GetFormsDrawable(source));
		}

		async Task SetMenuItemIcon(IMenuItem menuItem, ImageSource source)
		{
			if (source == null)
				return;
			var drawable = await Context.GetFormsDrawable(source);
			menuItem.SetIcon(drawable);
			drawable?.Dispose();
		}

		void SetupMenu()
		{
			using (var menu = _bottomView.Menu)
				SetupMenu(menu, _bottomView.MaxItemCount, ShellItem);
		}

		void UpdateTabBarVisibility()
		{
			if (DisplayedPage == null)
				return;

			bool visible = Shell.GetTabBarIsVisible(DisplayedPage);
			using (var menu = _bottomView.Menu)
			{
				if (menu.Size() == 1)
					visible = false;
			}

			_bottomView.Visibility = (visible) ? ViewStates.Visible : ViewStates.Gone;
		}
	}
}