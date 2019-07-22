using Android.Views;
using AView = Android.Views.View;
using AResource = Android.Resource;
using Android.Content.Res;

namespace Xamarin.Forms.Platform.Android
{
	public static class ButtonElementManager
	{
		public static bool OnTouch(VisualElement element, IButtonController buttonController, AView v, MotionEvent e)
		{
			switch (e.ActionMasked)
			{
				case MotionEventActions.Down:
					buttonController?.SendPressed();
					break;
				case MotionEventActions.Up:
					buttonController?.SendReleased();
					break;
			}

			return false;
		}

		public static void OnClick(VisualElement element, IButtonController buttonController, AView v)
		{
			buttonController?.SendClicked();
		}

		public static void UpdateDisabledTextColor(Button button, global::Android.Widget.Button nativeButton)
		{
			int[][] states = { new[] { AResource.Attribute.StateEnabled }, new[] { -AResource.Attribute.StateEnabled } };
			int[] colors = { nativeButton.TextColors.GetColorForState(states[0], button.TextColor.ToAndroid()), button.DisabledTextColor.ToAndroid().ToArgb() };

			var colorStateList = new ColorStateList(states, colors);

			nativeButton.SetTextColor(colorStateList);
		}
	}
}
