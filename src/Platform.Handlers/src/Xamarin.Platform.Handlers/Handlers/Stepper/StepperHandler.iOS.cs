using System;
using System.Drawing;
using UIKit;

namespace Xamarin.Platform.Handlers
{
	public partial class StepperHandler : AbstractViewHandler<IStepper, UIStepper>
	{
		protected override UIStepper CreateNativeView()
		{
			return new UIStepper(RectangleF.Empty);
		}

		protected override void ConnectHandler(UIStepper nativeView)
		{
			nativeView.ValueChanged += OnValueChanged;
		}

		protected override void DisconnectHandler(UIStepper nativeView)
		{
			nativeView.ValueChanged -= OnValueChanged;
		}

		void OnValueChanged(object? sender, EventArgs e)
		{
			if (TypedNativeView == null || VirtualView == null)
				return;

			VirtualView.Value = TypedNativeView.Value;
		}
	}
}