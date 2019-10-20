using System;
using System.ComponentModel;

#if __MOBILE__
using UIKit;
using NativeButton = UIKit.UIButton;
namespace Xamarin.Forms.Platform.iOS
#else
using AppKit;
using CoreAnimation;
using NativeButton = Xamarin.Forms.Platform.MacOS.ButtonRenderer.FormsNSButton;
namespace Xamarin.Forms.Platform.MacOS
#endif

{
	internal static class ButtonElementManager
	{
#if __MOBILE__
		static readonly UIControlState[] s_controlStates = { UIControlState.Normal, UIControlState.Highlighted, UIControlState.Disabled };
#endif

		public static void Init(IVisualNativeElementRenderer renderer)
		{
			renderer.ElementPropertyChanged += OnElementPropertyChanged;
			renderer.ControlChanged += OnControlChanged;
		}

		static void OnControlChanged(object sender, EventArgs e)
		{
			var renderer = (IVisualNativeElementRenderer)sender;
			var control = (NativeButton)renderer.Control;

#if __MOBILE__
			foreach (UIControlState uiControlState in s_controlStates)
			{
				control.SetTitleColor(UIButton.Appearance.TitleColor(uiControlState), uiControlState); // if new values are null, old values are preserved.
				control.SetTitleShadowColor(UIButton.Appearance.TitleShadowColor(uiControlState), uiControlState);
				control.SetBackgroundImage(UIButton.Appearance.BackgroundImageForState(uiControlState), uiControlState);
			}

			control.TouchUpInside -= TouchUpInside;
			control.TouchDown -= TouchDown;
			control.TouchUpInside += TouchUpInside;
			control.TouchDown += TouchDown;
#else
			control.Released -= TouchUpInside;
			control.Pressed -= TouchDown;
			control.Released += TouchUpInside;
			control.Pressed += TouchDown;
#endif
		}
		 

		static void TouchUpInside(object sender, EventArgs eventArgs)
		{
			var button = sender as NativeButton;
			var renderer = button.Superview as IVisualNativeElementRenderer;
			OnButtonTouchUpInside(renderer.Element as IButtonController);
		}

		static void TouchDown(object sender, EventArgs eventArgs)
		{
			var button = sender as NativeButton;
			var renderer = button.Superview as IVisualNativeElementRenderer;
			OnButtonTouchDown(renderer.Element as IButtonController);
		}

		public static void Dispose(IVisualNativeElementRenderer renderer)
		{
			var control = (NativeButton)renderer.Control;
			renderer.ElementPropertyChanged -= OnElementPropertyChanged;
#if __MOBILE__
			control.TouchUpInside -= TouchUpInside;
			control.TouchDown -= TouchDown;
#else
			control.Released -= TouchUpInside;
			control.Pressed -= TouchDown;
#endif
		}

		static void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		internal static void OnButtonTouchDown(IButtonController element)
		{
			element?.SendPressed();
		}

		internal static void OnButtonTouchUpInside(IButtonController element)
		{
			element?.SendReleased();
			element?.SendClicked();
		}
	}
}