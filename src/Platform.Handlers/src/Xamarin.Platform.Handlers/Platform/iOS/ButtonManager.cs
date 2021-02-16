using UIKit;

namespace Xamarin.Platform
{
	internal static class ButtonManager
	{
		static readonly UIControlState[] ControlStates = { UIControlState.Normal, UIControlState.Highlighted, UIControlState.Disabled };

		public static void Init(UIButton nativeView)
		{
			SetControlPropertiesFromProxy(nativeView);
		}

		public static void SetControlPropertiesFromProxy(UIButton nativeView)
		{
			foreach (UIControlState uiControlState in ControlStates)
			{
				nativeView.SetTitleColor(UIButton.Appearance.TitleColor(uiControlState), uiControlState); // If new values are null, old values are preserved.
				nativeView.SetTitleShadowColor(UIButton.Appearance.TitleShadowColor(uiControlState), uiControlState);
				nativeView.SetBackgroundImage(UIButton.Appearance.BackgroundImageForState(uiControlState), uiControlState);
			}
		}

		public static void OnTouchDown(IButton? virtualView)
		{
			virtualView?.Pressed();
		}

		public static void OnTouchUpInside(IButton? virtualView)
		{
			virtualView?.Released();
			virtualView?.Clicked();
		}

		public static void OnTouchUpOutside(IButton? virtualView)
		{
			virtualView?.Released();
		}
	}
}