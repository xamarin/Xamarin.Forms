using System;
using UIKit;
using RectangleF = CoreGraphics.CGRect;

namespace Microsoft.Maui.Handlers
{
	public partial class SwitchHandler : AbstractViewHandler<ISwitch, UISwitch>
	{
		static UIColor? DefaultOnTrackColor;
		static UIColor? DefaultOffTrackColor;
		static UIColor? DefaultThumbColor;

		protected override UISwitch CreateNativeView()
		{
			return new UISwitch(RectangleF.Empty);
		}

		protected override void ConnectHandler(UISwitch nativeView)
		{
			nativeView.ValueChanged += OnControlValueChanged;
		}

		protected override void DisconnectHandler(UISwitch nativeView)
		{
			nativeView.ValueChanged -= OnControlValueChanged;
		}

		protected override void SetupDefaults(UISwitch nativeView)
		{
			DefaultOnTrackColor = UISwitch.Appearance.OnTintColor;
			DefaultOffTrackColor = nativeView.GetOffTrackColor();
			DefaultThumbColor = UISwitch.Appearance.ThumbTintColor;
		}

		public static void MapIsToggled(SwitchHandler handler, ISwitch view)
		{
			handler.TypedNativeView?.UpdateIsToggled(view);
		}

		public static void MapTrackColor(SwitchHandler handler, ISwitch view)
		{
			handler.TypedNativeView?.UpdateTrackColor(view, DefaultOnTrackColor, DefaultOffTrackColor);
		}

		public static void MapThumbColor(SwitchHandler handler, ISwitch view)
		{
			handler.TypedNativeView?.UpdateThumbColor(view, DefaultThumbColor);
		}

		void OnControlValueChanged(object? sender, EventArgs e)
		{
			if (VirtualView == null)
				return;

			if (TypedNativeView != null)
				VirtualView.IsToggled = TypedNativeView.On;
		}
	}
}