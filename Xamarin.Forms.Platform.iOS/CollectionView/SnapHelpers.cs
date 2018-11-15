using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class SnapHelpers
	{
		public static CGPoint AdjustContentOffset(CGPoint proposedContentOffset, CGRect itemFrame,
			CGRect viewport, SnapPointsAlignment alignment, UICollectionViewScrollDirection scrollDirection)
		{
			var offset = GetViewportOffset(itemFrame, viewport, alignment, scrollDirection);
			return new CGPoint(proposedContentOffset.X - offset.X, proposedContentOffset.Y - offset.Y);
		}

		public static CGPoint FindAlignmentTarget(SnapPointsAlignment snapPointsAlignment,
			CGPoint proposedContentOffset,
			UICollectionView collectionView, UICollectionViewScrollDirection scrollDirection)
		{
			var inset = collectionView.ContentInset;
			var bounds = collectionView.Bounds;

			switch (scrollDirection)
			{
				case UICollectionViewScrollDirection.Vertical:
					var y = FindAlignmentTarget(snapPointsAlignment, proposedContentOffset.Y, inset.Top,
						proposedContentOffset.Y + bounds.Height, inset.Bottom);
					return new CGPoint(proposedContentOffset.X, y);
				case UICollectionViewScrollDirection.Horizontal:
					var x = FindAlignmentTarget(snapPointsAlignment, proposedContentOffset.X, inset.Left,
						proposedContentOffset.X + bounds.Width, inset.Right);
					return new CGPoint(x, proposedContentOffset.Y);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static UICollectionViewLayoutAttributes FindBestSnapCandidate(UICollectionViewLayoutAttributes[] items,
			CGRect viewport, CGPoint alignmentTarget)
		{
			UICollectionViewLayoutAttributes bestCandidate = null;

			foreach (UICollectionViewLayoutAttributes item in items)
			{
				if (!IsAtLeastHalfVisible(item, viewport))
				{
					continue;
				}

				bestCandidate = bestCandidate == null ? item : Nearer(bestCandidate, item, alignmentTarget);
			}

			return bestCandidate;
		}

		static nfloat Area(CGRect rect)
		{
			return rect.Height * rect.Width;
		}

		static CGPoint Center(CGRect rect)
		{
			return new CGPoint(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
		}

		static nfloat DistanceSquared(CGRect rect, CGPoint target)
		{
			var rectCenter = Center(rect);

			return (target.X - rectCenter.X) * (target.X - rectCenter.X) +
					(target.Y - rectCenter.Y) * (target.Y - rectCenter.Y);
		}

		static nfloat FindAlignmentTarget(SnapPointsAlignment snapPointsAlignment, nfloat start, nfloat startInset,
			nfloat end, nfloat endInset)
		{
			switch (snapPointsAlignment)
			{
				case SnapPointsAlignment.Center:
					var viewPortStart = start + startInset;
					var viewPortEnd = end - endInset;
					var viewPortSize = viewPortEnd - viewPortStart;

					return viewPortStart + (viewPortSize / 2);

				case SnapPointsAlignment.End:
					return end - endInset;

				case SnapPointsAlignment.Start:
				default:
					return start + startInset;
			}
		}

		static CGPoint GetViewportOffset(CGRect itemFrame, CGRect viewport, SnapPointsAlignment snapPointsAlignment,
			UICollectionViewScrollDirection scrollDirection)
		{
			if (scrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				if (snapPointsAlignment == SnapPointsAlignment.Start)
				{
					return new CGPoint(viewport.Left - itemFrame.Left, 0);
				}

				if (snapPointsAlignment == SnapPointsAlignment.End)
				{
					return new CGPoint(viewport.Right - itemFrame.Right, 0);
				}

				var centerViewport = Center(viewport);
				var centerItem = Center(itemFrame);

				return new CGPoint(centerViewport.X - centerItem.X, 0);
			}

			if (snapPointsAlignment == SnapPointsAlignment.Start)
			{
				return new CGPoint(0, viewport.Top - itemFrame.Top);
			}

			if (snapPointsAlignment == SnapPointsAlignment.End)
			{
				return new CGPoint(0, viewport.Bottom - itemFrame.Bottom);
			}

			var centerViewport1 = Center(viewport);
			var centerItem1 = Center(itemFrame);

			return new CGPoint(0, centerViewport1.Y - centerItem1.Y);
		}

		static bool IsAtLeastHalfVisible(UICollectionViewLayoutAttributes item, CGRect viewport)
		{
			var itemFrame = item.Frame;
			var visibleArea = Area(CGRect.Intersect(itemFrame, viewport));

			return visibleArea >= Area(itemFrame) / 2;
		}

		static UICollectionViewLayoutAttributes Nearer(UICollectionViewLayoutAttributes a,
			UICollectionViewLayoutAttributes b,
			CGPoint target)
		{
			var dA = DistanceSquared(a.Frame, target);
			var dB = DistanceSquared(b.Frame, target);

			if (dA < dB)
			{
				return a;
			}

			return b;
		}
	}
}