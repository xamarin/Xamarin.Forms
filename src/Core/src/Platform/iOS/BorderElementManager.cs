using System;
using UIKit;

namespace Microsoft.Maui
{
	public static class BorderElementManager
	{
		static nfloat DefaultCornerRadius = 5;

		public static void UpdateBorder(UIButton nativeView, IBorder border)
		{
			if (nativeView == null)
				return;

			// TODO: Set BorderColor and BorderWidth

			nfloat cornerRadius = DefaultCornerRadius;

			if (border.CornerRadius != -1)
				cornerRadius = border.CornerRadius;

			nativeView.Layer.CornerRadius = cornerRadius;
		}
	}
}