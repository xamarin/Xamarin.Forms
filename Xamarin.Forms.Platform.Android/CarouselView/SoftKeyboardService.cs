using System;
using Android.App;

// PR Fix for entry focus bug #242
namespace Xamarin.Forms.Platform.Android
{
	internal class SoftKeyboardService : SoftKeyboardServiceBase, IDisposable
	{
		readonly Activity _activity;
		GlobalLayoutListener _globalLayoutListener;

		public SoftKeyboardService(Activity activity)
		{
			_activity = activity;
			if (_activity == null)
				throw new Exception("Activity can't be null!");
		}

		public override event EventHandler<SoftKeyboardEventArgs> VisibilityChanged
		{
			add
			{
				base.VisibilityChanged += value;
				CheckListener();
			}
			remove
			{
				base.VisibilityChanged -= value;
				RemoveListener();
			}
		}

		public void Dispose()
		{
			RemoveListener();
		}

		private void RemoveListener()
		{
			if (_globalLayoutListener != null)
			{
				_activity.Window.DecorView.ViewTreeObserver.RemoveOnGlobalLayoutListener(_globalLayoutListener);
				_globalLayoutListener = null;
			}
		}

		private void CheckListener()
		{
			if (_globalLayoutListener == null)
			{
				_globalLayoutListener = new GlobalLayoutListener(this, _activity);
				_activity.Window.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(_globalLayoutListener);
			}
		}
	}

	internal abstract class SoftKeyboardServiceBase
	{
		public virtual event EventHandler<SoftKeyboardEventArgs> VisibilityChanged;

		public void InvokeVisibilityChanged(SoftKeyboardEventArgs args)
		{
			VisibilityChanged?.Invoke(this, args);
		}
	}
}