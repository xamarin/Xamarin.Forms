using System;
using CoreGraphics;
using Foundation;

namespace Xamarin.Forms.Platform.iOS
{
	internal sealed class TemplatedHorizontalListCell : TemplatedCell, IConstrainedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.TemplatedHorizontalListCell");

		[Export("initWithFrame:")]
		public TemplatedHorizontalListCell(CGRect frame) : base(frame)
		{
		}

		public void SetConstrainedDimension(nfloat constant)
		{
			ConstrainedDimension = constant;
			Layout();
		}

		public override CGSize Layout()
		{
			var nativeView = VisualElementRenderer.NativeView;

			var measure = VisualElementRenderer.Element.Measure(ContentView.Frame.Width, 
				ConstrainedDimension, MeasureFlags.IncludeMargins);

			var width = VisualElementRenderer.Element.Width > 0 
				? VisualElementRenderer.Element.Width : measure.Request.Width;

			nativeView.Frame = new CGRect(0, 0, width, ConstrainedDimension);

			VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());

			return VisualElementRenderer.NativeView.Frame.Size;
		}

		public override CGSize GetEstimate()
		{
			return Layout();
		}
	}
}