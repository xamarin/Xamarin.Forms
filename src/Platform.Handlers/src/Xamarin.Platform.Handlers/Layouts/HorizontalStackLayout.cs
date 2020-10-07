using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public class HorizontalStackLayout : StackLayout
	{
		public override Size Measure(double widthConstraint, double heightConstraint)
		{
			if (IsMeasureValid)
			{
				return DesiredSize;
			}

			var heightMeasureConstraint = ResolveConstraints(heightConstraint, Height);

			var measure = Measure(heightMeasureConstraint, Spacing, Children);

			var finalWidth = ResolveConstraints(widthConstraint, Width, measure.Width);

			DesiredSize = new Size(finalWidth, measure.Height);

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

			Arrange(bounds.Height, Spacing, Children);
			IsArrangeValid = true;
			Handler?.SetFrame(bounds);
		}

		static Size Measure(double heightConstraint, int spacing, IReadOnlyList<IView> views)
		{
			double totalRequestedWidth = 0;
			double requestedHeight = 0;

			foreach (var child in views)
			{
				// TODO check child.IsVisible

				var measure = child.IsMeasureValid ? child.DesiredSize : child.Measure(double.PositiveInfinity, heightConstraint);
				totalRequestedWidth += measure.Width;

				requestedHeight = Math.Max(requestedHeight, measure.Height);
			}

			var accountForSpacing = MeasureSpacing(spacing, views.Count);
			totalRequestedWidth += accountForSpacing;

			return new Size(totalRequestedWidth, requestedHeight);
		}

		static void Arrange(double heightConstraint, int spacing, IEnumerable<IView> views)
		{
			double stackWidth = 0;

			foreach (var child in views)
			{
				var destination = new Rectangle(stackWidth, 0, child.DesiredSize.Width, heightConstraint);
				child.Arrange(destination);

				stackWidth += destination.Width + spacing;
			}
		}
	}
}
