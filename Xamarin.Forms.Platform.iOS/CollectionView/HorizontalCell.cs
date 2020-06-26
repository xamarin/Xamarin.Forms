using System;
using CoreGraphics;
using Foundation;

namespace Xamarin.Forms.Platform.iOS
{
	internal sealed class HorizontalCell : HeightConstrainedTemplatedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.HorizontalCell");

		[Export("initWithFrame:")]
		[Internals.Preserve(Conditional = true)]
		public HorizontalCell(CGRect frame) : base(frame)
		{
		}

		public override CGSize Measure() => MeasureInternal(VisualElementRenderer.Element, ConstrainedDimension);

		public static CGSize MeasureInternal(VisualElement element, nfloat constrainedDimension)
		{
			var measure = element.Measure(double.PositiveInfinity,
				constrainedDimension, MeasureFlags.IncludeMargins);

			return new CGSize(measure.Request.Width, constrainedDimension);
		}

	}
}