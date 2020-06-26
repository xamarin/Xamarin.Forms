using System;
using CoreGraphics;
using Foundation;

namespace Xamarin.Forms.Platform.iOS
{
	internal sealed class VerticalCell : WidthConstrainedTemplatedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.VerticalCell");

		[Export("initWithFrame:")]
		[Internals.Preserve(Conditional = true)]
		public VerticalCell(CGRect frame) : base(frame)
		{
		}

		public override CGSize Measure() => MeasureInternal(VisualElementRenderer.Element, ConstrainedDimension);

		internal static CGSize MeasureInternal(VisualElement element, nfloat constrainedDimension)
		{
			var measure = element.Measure(constrainedDimension,
				double.PositiveInfinity, MeasureFlags.IncludeMargins);

			return new CGSize(constrainedDimension, measure.Request.Height);
		}
	}
}