using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using System;
using AColor = Android.Graphics.Color;
using R = Android.Resource;

namespace Xamarin.Forms.Platform.Android
{
	public class ShellBottomNavViewAppearanceTracker : IShellBottomNavViewAppearanceTracker
	{
		IShellContext _shellContext;
		ShellItem _shellItem;
		ColorStateList _defaultList;
		bool _disposed;
		Color _lastColor = Color.Default;
		ColorStateList _colorStateList;

		public ShellBottomNavViewAppearanceTracker(IShellContext shellContext, ShellItem shellItem)
		{
			_shellItem = shellItem;
			_shellContext = shellContext;
		}

		public virtual void ResetAppearance(BottomNavigationView bottomView)
		{
			if (_defaultList != null)
			{
				bottomView.ItemTextColor = _defaultList;
				bottomView.ItemIconTintList = _defaultList;
			}

			SetBackgroundColor(bottomView, Color.White);
		}

		public virtual void SetAppearance(BottomNavigationView bottomView, ShellAppearance appearance)
		{
			IShellAppearanceElement controller = appearance;
			var backgroundColor = controller.EffectiveTabBarBackgroundColor;
			var foregroundColor = controller.EffectiveTabBarForegroundColor; // currently unused
			var disabledColor = controller.EffectiveTabBarDisabledColor;
			var unselectedColor = controller.EffectiveTabBarUnselectedColor;
			var titleColor = controller.EffectiveTabBarTitleColor;

			if (_defaultList == null)
			{
#if __ANDROID_28__
				_defaultList = bottomView.ItemTextColor ?? MakeColorStateList(titleColor.ToAndroid().ToArgb(), disabledColor.ToAndroid().ToArgb(), unselectedColor.ToAndroid().ToArgb());
#else
				_defaultList = bottomView.ItemTextColor;
#endif
			}

			_colorStateList = MakeColorStateList(titleColor, disabledColor, unselectedColor);
			bottomView.ItemTextColor = _colorStateList;
			bottomView.ItemIconTintList = _colorStateList;

			SetBackgroundColor(bottomView, backgroundColor);
		}

		protected virtual void SetBackgroundColor(BottomNavigationView bottomView, Color color)
		{
			if (_lastColor.IsDefault)
				_lastColor = color;

			var menuView = bottomView.GetChildAt(0) as BottomNavigationMenuView;

			var oldBackground = bottomView.Background;

			if (menuView == null)
			{
				bottomView.SetBackground(new ColorDrawable(color.ToAndroid()));
			}
			else
			{
				var index = _shellItem.Items.IndexOf(_shellItem.CurrentItem);

				var menu = bottomView.Menu;
				index = Math.Min(index, menu.Size() - 1);

				var child = menuView.GetChildAt(index);
				var touchPoint = new Point(child.Left + (child.Right - child.Left) / 2, child.Top + (child.Bottom - child.Top) / 2);

				bottomView.SetBackground(new ColorChangeRevealDrawable(_lastColor.ToAndroid(), color.ToAndroid(), touchPoint));

				_lastColor = color;
			}

			oldBackground?.Dispose();
		}

		ColorStateList MakeColorStateList(Color titleColor, Color disabledColor, Color unselectedColor)
		{
			var states = new int[][] {
				new int[] { -R.Attribute.StateEnabled },
				new int[] {R.Attribute.StateChecked },
				new int[] { }
			};

			var disabledInt = disabledColor.IsDefault ?
				_defaultList.GetColorForState(new[] { -R.Attribute.StateEnabled }, AColor.Gray) :
				disabledColor.ToAndroid().ToArgb();

			var checkedInt = titleColor.IsDefault ?
				_defaultList.GetColorForState(new[] { R.Attribute.StateChecked }, AColor.Black) :
				titleColor.ToAndroid().ToArgb();

			var defaultColor = unselectedColor.IsDefault ?
				_defaultList.GetColorForState(new int[0], AColor.Black) :
				unselectedColor.ToAndroid().ToArgb();

			return MakeColorStateList(checkedInt, disabledInt, defaultColor);
		}

		ColorStateList MakeColorStateList(int titleColorInt, int disabledColorInt, int defaultColor)
		{
			var states = new int[][] {
				new int[] { -R.Attribute.StateEnabled },
				new int[] {R.Attribute.StateChecked },
				new int[] { }
			};

			var colors = new[] { disabledColorInt, titleColorInt, defaultColor };

			return new ColorStateList(states, colors);
		}

		#region IDisposable

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				_defaultList?.Dispose();
				_colorStateList?.Dispose();

				_shellItem = null;
				_shellContext = null;
				_defaultList = null;
				_colorStateList = null;
			}
		}

		#endregion IDisposable
	}
}