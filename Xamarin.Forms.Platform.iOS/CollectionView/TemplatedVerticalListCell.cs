using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal sealed class TemplatedVerticalListCell : TemplatedCell, IConstrainedCell
	{
		static int _called;

		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.TemplatedVerticalListCell");

		[Export("initWithFrame:")]
		public TemplatedVerticalListCell(CGRect frame) : base(frame)
		{
		}

		public void SetConstrainedDimension(nfloat constant)
		{
			ConstrainedDimension = constant;
		}

		protected override CGSize Layout()
		{
			Debug.WriteLine($">>>>> TemplatedVerticalListCell Layout 30: {_called++}");

			var nativeView = VisualElementRenderer.NativeView;

			//Debug.WriteLine($">>>>> TemplatedCell SetRenderer: ContentView.Frame.Width {ContentView.Frame.Width}");
			//Debug.WriteLine($">>>>> TemplatedCell SetRenderer: ContentView.Frame.Height {ContentView.Frame.Height}");

			var measure = VisualElementRenderer.Element.Measure(ConstrainedDimension, 
				double.PositiveInfinity, MeasureFlags.IncludeMargins);

	//		Debug.WriteLine($">>>>> TemplatedCell SetRenderer: sizeRequest is {measure}");

			var height = VisualElementRenderer.Element.Height > 0 
				? VisualElementRenderer.Element.Height : measure.Request.Height;

			nativeView.Frame = new CGRect(0, 0, ConstrainedDimension, height);

		//	Debug.WriteLine($">>>>> TemplatedCell SetRenderer 48: {nativeView.Frame}");

			VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());

			//Debug.WriteLine($">>>>> TemplatedCell SetRenderer 52: {VisualElementRenderer.Element.Bounds}");
		//	Debug.WriteLine($">>>>> TemplatedCell SetRenderer 53: {ContentView.Frame}");

			return VisualElementRenderer.NativeView.Frame.Size;
		}

		public override CGSize GetEstimate()
		{
			return Layout();
		}

		public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
			UICollectionViewLayoutAttributes layoutAttributes)
		{
			Layout();

			layoutAttributes.Frame = VisualElementRenderer.NativeView.Frame;

			return layoutAttributes;
		}
	}
}