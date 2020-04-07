﻿using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UWPPoint = Windows.Foundation.Point;
using UWPSize = Windows.Foundation.Size;

namespace Xamarin.Forms.Platform.UWP
{
	internal static class ScrollHelpers
	{
		static UWPPoint Zero = new UWPPoint(0, 0);

		static bool IsVertical(ScrollViewer scrollViewer)
		{
			return scrollViewer.HorizontalScrollMode == Windows.UI.Xaml.Controls.ScrollMode.Disabled;
		}

		static UWPPoint AdjustToMakeVisible(UWPPoint point, UWPSize itemSize, ScrollViewer scrollViewer)
		{
			if (IsVertical(scrollViewer))
			{
				return AdjustToMakeVisibleVertical(point, itemSize, scrollViewer);
			}

			return AdjustToMakeVisibleHorizontal(point, itemSize, scrollViewer);
		}

		static UWPPoint AdjustToMakeVisibleVertical(UWPPoint point, UWPSize itemSize, ScrollViewer scrollViewer)
		{
			if (point.Y > (scrollViewer.VerticalOffset + scrollViewer.ViewportHeight))
			{
				return AdjustToEndVertical(point, itemSize, scrollViewer);
			}

			if (point.Y >= scrollViewer.VerticalOffset 
				&& point.Y < (scrollViewer.VerticalOffset + scrollViewer.ViewportHeight - itemSize.Height))
			{
				// The target is already in the viewport, no reason to scroll at all
				return new UWPPoint(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
			}

			return point;
		}

		static UWPPoint AdjustToMakeVisibleHorizontal(UWPPoint point, UWPSize itemSize, ScrollViewer scrollViewer)
		{
			if (point.X > (scrollViewer.HorizontalOffset + scrollViewer.ViewportWidth))
			{
				return AdjustToEndHorizontal(point, itemSize, scrollViewer);
			}

			if (point.X >= scrollViewer.HorizontalOffset 
				&& point.X < (scrollViewer.HorizontalOffset + scrollViewer.ViewportWidth - itemSize.Width))
			{
				// The target is already in the viewport, no reason to scroll at all
				return new UWPPoint(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
			}

			return point;
		}

		static UWPPoint AdjustToEnd(UWPPoint point, UWPSize itemSize, ScrollViewer scrollViewer)
		{
			if (IsVertical(scrollViewer))
			{
				return AdjustToEndVertical(point, itemSize, scrollViewer);
			}

			return AdjustToEndHorizontal(point, itemSize, scrollViewer);
		}

		static UWPPoint AdjustToEndHorizontal(UWPPoint point, UWPSize itemSize, ScrollViewer scrollViewer)
		{
			var adjustment = scrollViewer.ViewportWidth - itemSize.Width;
			return new UWPPoint(point.X - adjustment, point.Y);
		}

		static UWPPoint AdjustToEndVertical(UWPPoint point, UWPSize itemSize, ScrollViewer scrollViewer)
		{
			var adjustment = scrollViewer.ViewportHeight - itemSize.Height;
			return new UWPPoint(point.X, point.Y - adjustment);
		}

		static async Task AdjustToEndAsync(ListViewBase list, ScrollViewer scrollViewer, object targetItem)
		{
			var point = new UWPPoint(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
			var targetContainer = list.ContainerFromItem(targetItem) as UIElement;
			point = AdjustToEnd(point, targetContainer.DesiredSize, scrollViewer);
			await JumpToOffsetAsync(scrollViewer, point.X, point.Y);
		}

		static UWPPoint AdjustToCenter(UWPPoint point, UWPSize itemSize, ScrollViewer scrollViewer)
		{
			if (IsVertical(scrollViewer))
			{
				return AdjustToCenterVertical(point, itemSize, scrollViewer);
			}

			return AdjustToCenterHorizontal(point, itemSize, scrollViewer);
		}

		static UWPPoint AdjustToCenterHorizontal(UWPPoint point, UWPSize itemSize, ScrollViewer scrollViewer)
		{
			var adjustment = (scrollViewer.ViewportWidth / 2) - (itemSize.Width / 2);
			return new UWPPoint(point.X - adjustment, point.Y);
		}

		static UWPPoint AdjustToCenterVertical(UWPPoint point, UWPSize itemSize, ScrollViewer scrollViewer)
		{
			var adjustment = (scrollViewer.ViewportHeight / 2) - (itemSize.Height / 2);
			return new UWPPoint(point.X, point.Y - adjustment);
		}

		static async Task AdjustToCenterAsync(ListViewBase list, ScrollViewer scrollViewer, object targetItem)
		{
			var point = new UWPPoint(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
			var targetContainer = list.ContainerFromItem(targetItem) as UIElement;
			point = AdjustToCenter(point, targetContainer.DesiredSize, scrollViewer);
			await JumpToOffsetAsync(scrollViewer, point.X, point.Y);
		}

		static async Task JumpToOffsetAsync(ScrollViewer scrollViewer, double targetHorizontalOffset, double targetVerticalOffset)
		{
			var tcs = new TaskCompletionSource<object>();

			void ViewChanged(object s, ScrollViewerViewChangedEventArgs e)
			{
				tcs.TrySetResult(null);
			}

			try
			{
				scrollViewer.ViewChanged += ViewChanged;
				scrollViewer.ChangeView(targetHorizontalOffset, targetVerticalOffset, null, true);
				await tcs.Task;
			}
			finally
			{
				scrollViewer.ViewChanged -= ViewChanged;
			}
		}

		static async Task<UWPPoint> GetApproximateTargetAsync(ListViewBase list, ScrollViewer scrollViewer, object targetItem)
		{
			// Keep track of where we are now
			var horizontalOffset = scrollViewer.HorizontalOffset;
			var verticalOffset = scrollViewer.VerticalOffset;

			// Jump to the target item and record its position. This won't be completely accurate because of 
			// virtualization, but it'll be close enough to give us a direction to scroll toward
			await JumpToItemAsync(list, targetItem, ScrollToPosition.Start);
			var targetContainer = list.ContainerFromItem(targetItem) as UIElement;
			if (targetContainer == null)
				return new UWPPoint(0, 0);

			var transform = targetContainer.TransformToVisual(scrollViewer.Content as UIElement);

			// Return to the original position
			await JumpToOffsetAsync(scrollViewer, horizontalOffset, verticalOffset);

			// Return the transformed point
			return transform.TransformPoint(Zero);
		}

		public static async Task JumpToItemAsync(ListViewBase list, object targetItem, ScrollToPosition scrollToPosition)
		{
			var scrollViewer = list.GetFirstDescendant<ScrollViewer>();

			var tcs = new TaskCompletionSource<object>();
			Func<Task> adjust = null;

			async void ViewChanged(object s, ScrollViewerViewChangedEventArgs e)
			{
				if (e.IsIntermediate)
				{
					return;
				}

				scrollViewer.ViewChanged -= ViewChanged;

				if (adjust != null)
				{
					// Handle adjustments for non-natively supported scroll positions
					await adjust();
				}

				tcs.TrySetResult(null);
			}

			try
			{
				scrollViewer.ViewChanged += ViewChanged;

				switch (scrollToPosition)
				{
					case ScrollToPosition.MakeVisible:
						list.ScrollIntoView(targetItem, ScrollIntoViewAlignment.Default);
						break;
					case ScrollToPosition.Start:
						list.ScrollIntoView(targetItem, ScrollIntoViewAlignment.Leading);
						break;
					case ScrollToPosition.Center:
						list.ScrollIntoView(targetItem, ScrollIntoViewAlignment.Leading);
						adjust = () => AdjustToCenterAsync(list, scrollViewer, targetItem);
						break;
					case ScrollToPosition.End:
						list.ScrollIntoView(targetItem, ScrollIntoViewAlignment.Leading);
						adjust = () => AdjustToEndAsync(list, scrollViewer, targetItem);
						break;
				}

				await tcs.Task;
			}
			finally
			{
				scrollViewer.ViewChanged -= ViewChanged;
			}
		}

		static async Task<bool> ScrollToItemAsync(ListViewBase list, object targetItem, ScrollViewer scrollViewer, ScrollToPosition scrollToPosition)
		{
			var targetContainer = list.ContainerFromItem(targetItem) as UIElement;

			if (targetContainer != null)
			{
				await ScrollToTargetContainerAsync(targetContainer, scrollViewer, scrollToPosition);
				return true;
			}

			return false;
		}

		public static async Task AnimateToItemAsync(ListViewBase list, object targetItem, ScrollToPosition scrollToPosition)
		{
			var scrollViewer = list.GetFirstDescendant<ScrollViewer>();

			// ScrollToItemAsync will only scroll to the item if it actually exists in the list (that is, it has been
			// been realized and isn't just a virtual item)
			if (await ScrollToItemAsync(list, targetItem, scrollViewer, scrollToPosition))
			{
				// Happy path; the item was already realized and we could just scroll to it
				return;
			}

			// This is the unhappy path. Because of virtualization, the item has not actually been created yet.
			// So we make our best guess about the location of the item
			var targetPoint = await GetApproximateTargetAsync(list, scrollViewer, targetItem);

			// And then we scroll toward that position. The interruptCheck parameter will be run as we're scrolling
			// to see if the item exists yet; if it does, AnimateToOffsetAsync will be canceled and we'll finish
			// off with a smooth scroll to the item
			await AnimateToOffsetAsync(scrollViewer, targetPoint.X, targetPoint.Y,
				async () => await ScrollToItemAsync(list, targetItem, scrollViewer, scrollToPosition));
		}

		static async Task AnimateToOffsetAsync(ScrollViewer scrollViewer, double targetHorizontalOffset, double targetVerticalOffset,
			Func<Task<bool>> interruptCheck = null)
		{
			var tcs = new TaskCompletionSource<object>();

			// This method will fire as the scrollview scrolls along
			async void ViewChanged(object s, ScrollViewerViewChangedEventArgs e)
			{
				if (tcs.Task.IsCompleted)
				{
					return;
				}

				if (e.IsIntermediate)
				{
					// This is an intermediate scroll as part of the larger scroll; we're not all the way there yet
					// We take this opportunity to see if we should interrupt the scrolling

					if (interruptCheck == null)
					{
						return;
					}

					if (await interruptCheck())
					{
						// Cancel the current scrolling and just stop where we are
						scrollViewer.ChangeView(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset, 1.0f, true);
						tcs.TrySetResult(null);
					}
				}
				else
				{
					tcs.TrySetResult(null);
				}
			}

			try
			{
				scrollViewer.ViewChanged += ViewChanged;
				scrollViewer.ChangeView(targetHorizontalOffset, targetVerticalOffset, null, false);
				await tcs.Task;
			}
			finally
			{
				scrollViewer.ViewChanged -= ViewChanged;
			}
		}

		static async Task ScrollToTargetContainerAsync(UIElement targetContainer, ScrollViewer scrollViewer, ScrollToPosition scrollToPosition)
		{
			var transform = targetContainer.TransformToVisual(scrollViewer.Content as UIElement);
			var position = transform?.TransformPoint(Zero);

			if (!position.HasValue)
			{
				return;
			}

			UWPPoint offset = position.Value;

			// We'll use the desired size of the item because the actual size may not be actualized yet, and
			// we'll get a very unhelpful cast exception when it tries to cast to IUIElement10(!)
			var itemSize = targetContainer.DesiredSize;

			switch (scrollToPosition)
			{
				case ScrollToPosition.Start:
					// The transform will put the container at the top of the ScrollViewer; we'll need to adjust for
					// other scroll positions
					break;
				case ScrollToPosition.MakeVisible:
					offset = AdjustToMakeVisible(offset, itemSize, scrollViewer);
					break;
				case ScrollToPosition.Center:
					offset = AdjustToCenter(offset, itemSize, scrollViewer);
					break;
				case ScrollToPosition.End:
					offset = AdjustToEnd(offset, itemSize, scrollViewer);
					break;
			}

			await AnimateToOffsetAsync(scrollViewer, offset.X, offset.Y);
		}
	}
}