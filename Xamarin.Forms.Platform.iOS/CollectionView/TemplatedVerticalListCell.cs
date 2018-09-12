using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal sealed class TemplatedVerticalListCell : TemplatedCell, IConstrainedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.TemplatedVerticalListCell");

		[Export("initWithFrame:")]
		public TemplatedVerticalListCell(CGRect frame) : base(frame)
		{
		}

		public void Layout(CGSize constraints)
		{
			var nativeView = VisualElementRenderer.NativeView;

			var width = constraints.Width;
			var height = constraints.Height;

			VisualElementRenderer.Element.Measure(width, height, MeasureFlags.IncludeMargins);

			nativeView.Frame = new CGRect(0, 0, width, height);

			VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());
		}

		// TODO hartez 2018/09/12 10:51:26 This layout shouldn't return a value; rather, the parts which do the measuring should be a separate method which the Controller can use	

		public override CGSize Layout()
		{
			var nativeView = VisualElementRenderer.NativeView;

			var measure = VisualElementRenderer.Element.Measure(ConstrainedDimension, 
				double.PositiveInfinity, MeasureFlags.IncludeMargins);

			var height = VisualElementRenderer.Element.Height > 0 
				? VisualElementRenderer.Element.Height : measure.Request.Height;

			nativeView.Frame = new CGRect(0, 0, ConstrainedDimension, height);

			VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());

			return VisualElementRenderer.NativeView.Frame.Size;
		}

		public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
			UICollectionViewLayoutAttributes layoutAttributes)
		{
			Layout();

			layoutAttributes.Frame = VisualElementRenderer.NativeView.Frame;

			return layoutAttributes;
		}

		public void Constrain(nfloat constant)
		{
			ConstrainedDimension = constant;
		}

		public void Constrain(CGSize constraint)
		{
			ConstrainedDimension = constraint.Width;
			Layout(constraint);
		}
	}
}