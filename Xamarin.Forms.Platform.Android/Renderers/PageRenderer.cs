using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
#if __ANDROID_29__
using AndroidX.Core.Content;
using AndroidX.AppCompat.Widget;
#else
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
#endif
using Android.Views;
using Android.Views.Accessibility;
using AColor = Android.Graphics.Color;
using AColorRes = Android.Resource.Color;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class PageRenderer : VisualElementRenderer<Page>, IOrderedTraversalController
	{
		public PageRenderer(Context context) : base(context)
		{
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use PageRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PageRenderer()
		{
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			base.OnTouchEvent(e);

			return true;
		}

		IPageController PageController => Element as IPageController;

		IOrderedTraversalController OrderedTraversalController => this;

		double _previousHeight;
		bool _isDisposed = false;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				PageController?.SendDisappearing();
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();
			var pageContainer = Parent as PageContainer;
			if (pageContainer != null && (pageContainer.IsInFragment || pageContainer.Visibility == ViewStates.Gone))
				return;

			PageController.SendAppearing();
		}

		protected override void OnDetachedFromWindow()
		{
			base.OnDetachedFromWindow();
			var pageContainer = Parent as PageContainer;
			if (pageContainer != null && pageContainer.IsInFragment)
				return;

			PageController.SendDisappearing();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			if (Id == NoId)
			{
				Id = Platform.GenerateViewId();
			}

			UpdateBackground(false);
			Clickable = true;
			UpdateStatusBarColor();
			UpdateStatusBarStyle();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == Page.StatusBarColorProperty.PropertyName)
				UpdateStatusBarColor();
			else if (e.PropertyName == Page.StatusBarStyleProperty.PropertyName)
				UpdateStatusBarStyle();
			else if(e.PropertyName == Page.BackgroundImageSourceProperty.PropertyName)
				UpdateBackground(true);
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackground(false);
			else if (e.PropertyName == VisualElement.BackgroundProperty.PropertyName)
				UpdateBackground(false);
			else if (e.PropertyName == VisualElement.HeightProperty.PropertyName)
				UpdateHeight();
		}

		void UpdateHeight()
		{
			// Handle size changes because of the soft keyboard (there's probably a more elegant solution to this)

			// This is only necessary if:
			// - we're navigating back from a page where the soft keyboard was open when the user hit the Navigation Bar 'back' button
			// - the Application's content height has changed because WindowSoftInputModeAdjust was set to Resize
			// - the height has increased (in other words, the last layout was with the keyboard open, and now it's closed)
			var newHeight = Element.Height;

			if (_previousHeight > 0 && newHeight > _previousHeight)
			{
				var nav = Element.Navigation;

				// This update check will fire for all the pages on the stack, but we only need to request a layout for the top one
				if (nav?.NavigationStack != null && nav.NavigationStack.Count > 0 && Element == nav.NavigationStack[nav.NavigationStack.Count - 1])
				{
					// The Forms layout stuff is already correct, we just need to force Android to catch up
					RequestLayout();
				}
			}

			// Cache the height for next time
			_previousHeight = newHeight;
		}

		void UpdateBackground(bool setBkndColorEvenWhenItsDefault)
		{
			Page page = Element;

			_ = this.ApplyDrawableAsync(page, Page.BackgroundImageSourceProperty, Context, drawable =>
			{
				if (drawable != null)
				{
					this.SetBackground(drawable);
				}
				else
				{
					Brush background = Element.Background;

					if (!Brush.IsNullOrEmpty(background))
						this.UpdateBackground(background);
					else
					{
						Color backgroundColor = page.BackgroundColor;
						bool isDefaultBackgroundColor = backgroundColor.IsDefault;

						// A TabbedPage has no background. See Github6384.
						bool isInShell = page.Parent is BaseShellItem ||
						(page.Parent is TabbedPage && page.Parent?.Parent is BaseShellItem);

						if (isInShell && isDefaultBackgroundColor)
						{
							var color = Forms.IsMarshmallowOrNewer ?
								Context.Resources.GetColor(AColorRes.BackgroundLight, Context.Theme) :
								new AColor(ContextCompat.GetColor(Context, AColorRes.BackgroundLight));
							SetBackgroundColor(color);
						}
						else if (!isDefaultBackgroundColor || setBkndColorEvenWhenItsDefault)
						{
							SetBackgroundColor(backgroundColor.ToAndroid());
						}
					}
				}
			});
		}

		void UpdateStatusBarColor()
		{
			if (!Element.HasAppeared)
				return;

			var statusBarColor = GetEffectiveBindableValue<Color>(Page.StatusBarColorProperty);
			if (statusBarColor == Color.Default)
				return;

			(Context.GetActivity() as FormsAppCompatActivity)?.SetStatusBarColor(statusBarColor.ToAndroid());
		}

		void UpdateStatusBarStyle()
		{
			if (!Element.HasAppeared)
				return;

			if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
			{
				var window = (Context.GetActivity() as FormsAppCompatActivity)?.Window;
				if (window == null)
				{
					return;
				}

				var statusBarStyle= GetEffectiveBindableValue<StatusBarStyle>(Page.StatusBarStyleProperty);

				switch (statusBarStyle)
				{
					case StatusBarStyle.Default:
						window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.Visible;
						break;
					case StatusBarStyle.LightContent:
						window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.Visible;
						break;
					case StatusBarStyle.DarkContent:
						window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
						break;
				}
			}
		}

		void IOrderedTraversalController.UpdateTraversalOrder()
		{
			// traversal order wasn't added until API 22
			if ((int)Forms.SdkInt < 22)
				return;

			// since getting and updating the traversal order is expensive, let's only do it when a screen reader is active
			// note that this does NOT get auto updated when you enable TalkBack, so the page will need to be reloaded to enable this path 
			var am = AccessibilityManager.FromContext(Context);
			if (!am.IsEnabled)
				return;

			SortedDictionary<int, List<ITabStopElement>> tabIndexes = null;
			foreach (var child in Element.LogicalChildren)
			{
				if (!(child is VisualElement ve))
					continue;

				tabIndexes = ve.GetSortedTabIndexesOnParentPage();
				break;
			}

			if (tabIndexes == null)
				return;

			// Let the page handle tab order itself
			if (tabIndexes.Count <= 1)
				return;

			AView prevControl = null;
			foreach (var idx in tabIndexes?.Keys)
			{
				var tabGroup = tabIndexes[idx];
				foreach (var child in tabGroup)
				{
					if (!(child is VisualElement ve && ve.GetRenderer()?.View is AView view))
						continue;

					AView thisControl = null;

					if (view is ITabStop tabStop)
						thisControl = tabStop.TabStop;

					if (thisControl == null)
						continue;

					// this element should be the first thing focused after the root
					if (prevControl == null)
					{
						thisControl.AccessibilityTraversalAfter = NoId;
					}
					else
					{
						if (thisControl != prevControl)
							thisControl.AccessibilityTraversalAfter = prevControl.Id;
					}

					prevControl = thisControl;
				}
			}
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);
			OrderedTraversalController.UpdateTraversalOrder();
		}

		T GetEffectiveBindableValue<T>(BindableProperty bindableProperty)
		{
			Element element = Element;
			while (element != null && !element.IsSet(bindableProperty))
				element = element.Parent;

			if (element == null)
				return default(T);

			return (T)Element.GetValue(bindableProperty);
		}
	}
}
