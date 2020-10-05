using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public class VerticalStackLayout : StackLayout
	{
		public override SizeRequest Measure(double widthConstraint, double heightConstraint)
		{
			if (IsMeasureValid)
			{
				return DesiredSize;
			}

			DesiredSize = Measure(widthConstraint, Spacing, Children);

			IsMeasureValid = true;

			return DesiredSize;
		}

		public override void Arrange(Rectangle bounds)
		{
			if (IsArrangeValid)
			{
				return;
			}

			Arrange(bounds.Width, Spacing, Children);
			IsArrangeValid = true;
			Handler?.SetFrame(bounds);
		}

		static SizeRequest Measure(double widthConstraint, int spacing, IReadOnlyList<IView> views)
		{
			double totalRequestedHeight = 0;
			double totalMinimumHeight = 0;
			double requestedWidth = 0;
			double minimumWidth = 0;

			foreach (var child in views)
			{
				// TODO check child.IsVisible

				var measure = child.IsMeasureValid ? child.DesiredSize : child.Measure(widthConstraint, double.PositiveInfinity);
				totalRequestedHeight += measure.Request.Height;
				totalMinimumHeight += measure.Minimum.Height;

				requestedWidth = Math.Max(requestedWidth, measure.Request.Width);
				minimumWidth = Math.Max(minimumWidth, measure.Minimum.Width);
			}

			var accountForSpacing = MeasureSpacing(spacing, views.Count);
			totalRequestedHeight += accountForSpacing;
			totalMinimumHeight += accountForSpacing;

			return new SizeRequest(
				new Size(requestedWidth, totalRequestedHeight),
				new Size(minimumWidth, totalMinimumHeight));
		}

		static void Arrange(double widthConstraint, int spacing, IEnumerable<IView> views)
		{
			double stackHeight = 0;

			foreach (var child in views)
			{
				var destination = new Rectangle(0, stackHeight, widthConstraint, child.DesiredSize.Request.Height);
				child.Arrange(destination);

				stackHeight += destination.Height + spacing;
			}
		}
	}
}
