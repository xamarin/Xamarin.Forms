using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public class VerticalStackLayout : StackLayout
	{
		public override Size Measure(double widthConstraint, double heightConstraint)
		{
			if (IsMeasureValid)
			{
				return DesiredSize;
			}

			var widthMeasureConstraint = ResolveConstraints(widthConstraint, Width);

			var measure = Measure(widthMeasureConstraint, Spacing, Children);

			var finalHeight = ResolveConstraints(heightConstraint, Height, measure.Height);

			DesiredSize = new Size(measure.Width, finalHeight);

			IsMeasureValid = true;

			return DesiredSize;
		}

		public override void Arrange(Rectangle bounds)
		{
			if (IsArrangeValid)
			{
				return;
			}

			base.Arrange(bounds);

			Arrange(bounds.Width, Spacing, Children);
			IsArrangeValid = true;
			Handler?.SetFrame(bounds);
		}

		static Size Measure(double widthConstraint, int spacing, IReadOnlyList<IView> views)
		{
			double totalRequestedHeight = 0;
			double requestedWidth = 0;

			foreach (var child in views)
			{
				// TODO check child.IsVisible

				var measure = child.IsMeasureValid ? child.DesiredSize : child.Measure(widthConstraint, double.PositiveInfinity);
				totalRequestedHeight += measure.Height;

				requestedWidth = Math.Max(requestedWidth, measure.Width);
			}

			var accountForSpacing = MeasureSpacing(spacing, views.Count);
			totalRequestedHeight += accountForSpacing;

			return new Size(requestedWidth, totalRequestedHeight);
		}

		static void Arrange(double widthConstraint, int spacing, IEnumerable<IView> views)
		{
			double stackHeight = 0;

			foreach (var child in views)
			{
				var destination = new Rectangle(0, stackHeight, widthConstraint, child.DesiredSize.Height);
				child.Arrange(destination);

				stackHeight += destination.Height + spacing;
			}
		}
	}
}
