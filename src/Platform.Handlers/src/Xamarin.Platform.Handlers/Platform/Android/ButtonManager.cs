using Android.Views;
using AView = Android.Views.View;

namespace Xamarin.Platform
{
	public static class ButtonManager
	{
		public static bool OnTouch(IButton? button, AView? v, MotionEvent? e)
		{
			switch (e?.ActionMasked)
			{
				case MotionEventActions.Down:
					button?.Pressed();
					break;
				case MotionEventActions.Up:
					button?.Released();
					break;
			}

			return false;
		}

		public static void OnClick(IButton? button, AView? v)
		{
			button?.Clicked();
		}
	}
}
