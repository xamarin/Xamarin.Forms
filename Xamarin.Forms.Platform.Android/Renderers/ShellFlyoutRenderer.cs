﻿using Android.Content;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using System;
using System.ComponentModel;
using AView = Android.Views.View;
using LP = Android.Views.ViewGroup.LayoutParams;

namespace Xamarin.Forms.Platform.Android
{
	public class ShellFlyoutRenderer : DrawerLayout, IShellFlyoutRenderer, DrawerLayout.IDrawerListener, IFlyoutBehaviorObserver
	{
		#region IShellFlyoutRenderer

		AView IShellFlyoutRenderer.AndroidView => this;

		void IShellFlyoutRenderer.AttachFlyout(IShellContext context, AView content)
		{
			AttachFlyout(context, content);
		}

		#endregion IShellFlyoutRenderer

		#region IDrawerListener

		void IDrawerListener.OnDrawerClosed(AView drawerView)
		{
			Shell.SetValueFromRenderer(Shell.FlyoutIsPresentedProperty, false);
		}

		void IDrawerListener.OnDrawerOpened(AView drawerView)
		{
			Shell.SetValueFromRenderer(Shell.FlyoutIsPresentedProperty, true);
		}

		void IDrawerListener.OnDrawerSlide(AView drawerView, float slideOffset)
		{
		}

		void IDrawerListener.OnDrawerStateChanged(int newState)
		{
		}

		#endregion IDrawerListener

		#region IFlyoutBehaviorObserver

		void IFlyoutBehaviorObserver.OnFlyoutBehaviorChanged(FlyoutBehavior behavior)
		{
			UpdateDrawerLockMode(behavior);
		}

		#endregion IFlyoutBehaviorObserver

		const uint DefaultScrimColor = 0x99000000;
		readonly IShellContext _shellContext;
		AView _content;
		IShellFlyoutContentRenderer _flyoutContent;
		int _flyoutWidth;
		int _currentLockMode;

		public ShellFlyoutRenderer(IShellContext shellContext, Context context) : base(context)
		{
			_shellContext = shellContext;

			Shell.PropertyChanged += OnShellPropertyChanged;
		}

		Shell Shell => _shellContext.Shell;

		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			bool result = base.OnInterceptTouchEvent(ev);

			if (GetDrawerLockMode(_flyoutContent.AndroidView) == LockModeLockedOpen)
				return false;

			return result;
		}

		protected virtual void AttachFlyout(IShellContext context, AView content)
		{
			_content = content;

			_flyoutContent = context.CreateShellFlyoutContentRenderer();

			// Depending on what you read the right edge of the drawer should be Max(56dp, actionBarSize)
			// from the right edge of the screen. Fine. Well except that doesn't account
			// for landscape devices, in which case its still, according to design
			// documents from google 56dp, except google doesn't do that with their own apps.
			// So we are just going to go ahead and do what google does here even though
			// this isn't what DrawerLayout does by default.

			// Oh then there is this rule about how wide it should be at most. It should not
			// at least according to docs be more than 6 * actionBarSize wide. Again non of
			// this is about landscape devices and google does not perfectly follow these
			// rules... so we'll kind of just... do our best.

			var metrics = Context.Resources.DisplayMetrics;
			var width = Math.Min(metrics.WidthPixels, metrics.HeightPixels);

			var tv = new TypedValue();
			var actionBarHeight = (int)Context.ToPixels(56);
			if (Context.Theme.ResolveAttribute(global::Android.Resource.Attribute.ActionBarSize, tv, true))
			{
				actionBarHeight = TypedValue.ComplexToDimensionPixelSize(tv.Data, metrics);
			}
			width -= actionBarHeight;

			var maxWidth = actionBarHeight * 6;
			width = Math.Min(width, maxWidth);

			_flyoutWidth = width;

			_flyoutContent.AndroidView.LayoutParameters =
				new LayoutParams(width, LP.MatchParent) { Gravity = (int)GravityFlags.Start };

			AddView(content);
			AddView(_flyoutContent.AndroidView);

			AddDrawerListener(this);

			((IShellController)context.Shell).AddFlyoutBehaviorObserver(this);
		}

		protected virtual void OnShellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Shell.FlyoutIsPresentedProperty.PropertyName)
			{
				var presented = Shell.FlyoutIsPresented;
				if (presented)
					OpenDrawer(_flyoutContent.AndroidView, true);
				else
					CloseDrawers();
			}
		}

		protected virtual void UpdateDrawerLockMode(FlyoutBehavior behavior)
		{
			switch (behavior)
			{
				case FlyoutBehavior.Disabled:
					CloseDrawers();
					Shell.SetValueFromRenderer(Shell.FlyoutIsPresentedProperty, false);
					_currentLockMode = LockModeLockedClosed;
					SetDrawerLockMode(_currentLockMode);
					_content.SetPadding(0, _content.PaddingTop, _content.PaddingRight, _content.PaddingBottom);
					break;

				case FlyoutBehavior.Flyout:
					_currentLockMode = LockModeUnlocked;
					SetDrawerLockMode(_currentLockMode);
					_content.SetPadding(0, _content.PaddingTop, _content.PaddingRight, _content.PaddingBottom);
					break;

				case FlyoutBehavior.Locked:
					Shell.SetValueFromRenderer(Shell.FlyoutIsPresentedProperty, true);
					_currentLockMode = LockModeLockedOpen;
					SetDrawerLockMode(_currentLockMode);
					_content.SetPadding(_flyoutWidth, _content.PaddingTop, _content.PaddingRight, _content.PaddingBottom);
					break;
			}

			unchecked
			{
				SetScrimColor(behavior == FlyoutBehavior.Locked ? Color.Transparent.ToAndroid() : (int)DefaultScrimColor);
			}
		}
	}
}