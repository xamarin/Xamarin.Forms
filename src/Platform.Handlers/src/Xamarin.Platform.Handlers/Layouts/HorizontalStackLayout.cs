using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public class HorizontalStackLayout : StackLayout
	{
		public override SizeRequest Measure(double widthConstraint, double heightConstraint)
		{
			if (IsMeasureValid)
			{
				return DesiredSize;
			}

			DesiredSize = Measure(heightConstraint, Spacing, Children);

			IsMeasureValid = true;

			return DesiredSize;
		}

		public override void Arrange(Rectangle bounds)
		{
			if (IsArrangeValid)
			{
				return;
			}

			Arrange(bounds.Height, Spacing, Children);
			IsArrangeValid = true;
			Handler?.SetFrame(bounds);
		}

		static SizeRequest Measure(double heightConstraint, int spacing, IList<IView> views)
		{
			double totalRequestedWidth = 0;
			double totalMinimumWidth = 0;
			double requestedHeight = 0;
			double minimumHeight = 0;

			foreach (var child in views)
			{
				// TODO check child.IsVisible

				var measure = child.IsMeasureValid ? child.DesiredSize : child.Measure(double.PositiveInfinity, heightConstraint);
				totalRequestedWidth += measure.Request.Width;
				totalMinimumWidth += measure.Minimum.Width;

				requestedHeight = Math.Max(requestedHeight, measure.Request.Height);
				minimumHeight = Math.Max(minimumHeight, measure.Minimum.Height);
			}

			var accountForSpacing = MeasureSpacing(spacing, views.Count);
			totalRequestedWidth += accountForSpacing;
			totalMinimumWidth += accountForSpacing;

			return new SizeRequest(
				new Size(totalRequestedWidth, requestedHeight),
				new Size(totalMinimumWidth, minimumHeight));
		}

		static void Arrange(double heightConstraint, int spacing, IEnumerable<IView> views)
		{
			double stackWidth = 0;

			foreach (var child in views)
			{
				var destination = new Rectangle(stackWidth, 0, child.DesiredSize.Request.Width, heightConstraint);
				child.Arrange(destination);

				stackWidth += destination.Width + spacing;
			}
		}
	}
}
