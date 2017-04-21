using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	internal class SwipeGestureHandler
	{
		readonly Func<double, double> _pixelTranslation;

		public SwipeGestureHandler(Func<View> getView, Func<double, double> pixelTranslation)
		{
			_pixelTranslation = pixelTranslation;
			GetView = getView;
		}

		Func<View> GetView { get; }

		public bool OnSwipe(float x, float y)
		{
			View view = GetView();

			if (view == null)
				return false;

			var result = false;
			foreach (SwipeGestureRecognizer swipeGesture in
					 view.GestureRecognizers.GetGesturesFor<SwipeGestureRecognizer>())
			{
				((ISwipeGestureController)swipeGesture).SendSwipe(view, _pixelTranslation(x), _pixelTranslation(y));
				result = true;
			}

			return result;
		}

		public bool OnSwipeComplete()
		{
			View view = GetView();

			if (view == null)
				return false;

			var result = false;
			foreach (SwipeGestureRecognizer swipeGesture in view.GestureRecognizers.GetGesturesFor<SwipeGestureRecognizer>())
			{
				var detected = false;
				var direction = swipeGesture.Direction;
				var threshold = swipeGesture.Threshold;

				switch (direction)
				{
					case SwipeDirection.Left:
						detected = ((ISwipeGestureController)swipeGesture).TotalX < -threshold;
						break;
					case SwipeDirection.Right:
						detected = ((ISwipeGestureController)swipeGesture).TotalX > threshold;
						break;
					case SwipeDirection.Down:
						detected = ((ISwipeGestureController)swipeGesture).TotalY > threshold;
						break;
					case SwipeDirection.Up:
						detected = ((ISwipeGestureController)swipeGesture).TotalY < -threshold;
						break;
				}

				if (detected)
				{
					swipeGesture.SendSwiped(view, direction);
					result = true;
				}
			}

			return result;
		}
	}
}