﻿using System;

namespace Xamarin.Forms
{
	public abstract class BaseSwipeEventArgs : EventArgs
	{
		protected BaseSwipeEventArgs(SwipeDirection swipeDirection)
		{
			SwipeDirection = swipeDirection;
		}

		public SwipeDirection SwipeDirection { get; set; }
	}

	public class OpenSwipeEventArgs : EventArgs
	{
		public OpenSwipeEventArgs(OpenSwipeItem openSwipeItem)
		{
			OpenSwipeItem = openSwipeItem;
		}

		public OpenSwipeItem OpenSwipeItem { get; set; }
	}

	public class SwipeStartedEventArgs : BaseSwipeEventArgs
	{
		public SwipeStartedEventArgs(SwipeDirection swipeDirection) : base(swipeDirection)
		{

		}
	}

	public class SwipeChangingEventArgs : BaseSwipeEventArgs
	{
		public SwipeChangingEventArgs(SwipeDirection swipeDirection, double offset) : base(swipeDirection)
		{
			Offset = offset;
		}

		public double Offset { get; set; }
	}

	public class SwipeEndedEventArgs : BaseSwipeEventArgs
	{
		public SwipeEndedEventArgs(SwipeDirection swipeDirection, bool isOpen) : base(swipeDirection)
		{
			IsOpen = isOpen;
		}

		public bool IsOpen { get; set; }
	}
}