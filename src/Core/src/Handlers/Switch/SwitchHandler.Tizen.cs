using System;
using ElmSharp;

namespace Microsoft.Maui.Handlers
{
	public partial class SwitchHandler : AbstractViewHandler<ISwitch, Check>
	{
		protected override Check CreateNativeView() => new Check(NativeParent);

		protected override void ConnectHandler(Check nativeView)
		{
			nativeView.StateChanged += OnStateChanged;
		}

		protected override void DisconnectHandler(Check nativeView)
		{
			nativeView.StateChanged -= OnStateChanged;
		}

		public static void MapIsToggled(SwitchHandler handler, ISwitch view)
		{
			handler.TypedNativeView?.UpdateIsToggled(view);
		}

		public static void MapTrackColor(SwitchHandler handler, ISwitch view)
		{
			handler.TypedNativeView?.UpdateTrackColor(view);
		}

		public static void MapThumbColor(SwitchHandler handler, ISwitch view)
		{
			handler.TypedNativeView?.UpdateThumbColor(view);
		}

		void OnStateChanged(object sender, EventArgs e)
		{
			if (VirtualView == null)
				return;

			if (TypedNativeView != null)
				VirtualView.IsToggled = TypedNativeView.IsChecked;
		}
	}
}