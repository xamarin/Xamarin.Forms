using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal sealed class TemplatedHorizontalListCell : TemplatedCell, IConstrainedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.TemplatedHorizontalListCell");

		[Export("initWithFrame:")]
		public TemplatedHorizontalListCell(CGRect frame) : base(frame)
		{
		}

		public override CGSize Layout()
		{
			var nativeView = VisualElementRenderer.NativeView;

			var measure = VisualElementRenderer.Element.Measure(double.PositiveInfinity, 
				ConstrainedDimension, MeasureFlags.IncludeMargins);

			var width = VisualElementRenderer.Element.Width > 0 
				? VisualElementRenderer.Element.Width : measure.Request.Width;

			nativeView.Frame = new CGRect(0, 0, width, ConstrainedDimension);

			VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());

			return VisualElementRenderer.NativeView.Frame.Size;
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
			ConstrainedDimension = constraint.Height;
			Layout(constraint);
		}
	}
}