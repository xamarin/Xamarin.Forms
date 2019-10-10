using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xamarin.Forms
{
	public class GestureRecognizer : Element, IGestureRecognizer
	{
		static readonly BindablePropertyKey TouchStatePropertyKey =
			BindableProperty.CreateReadOnly(nameof(TouchState), typeof(TouchState), typeof(GestureRecognizer), TouchState.Default);

		static readonly BindablePropertyKey TouchCountPropertyKey =
			BindableProperty.CreateReadOnly(nameof(TouchCount), typeof(int), typeof(GestureRecognizer), 0);

		static readonly BindablePropertyKey TouchesPropertyKey =
			BindableProperty.CreateReadOnly(nameof(Touches), typeof(IReadOnlyList<Touch>), typeof(GestureRecognizer), new List<Touch>());

		public static readonly BindableProperty TouchStateProperty = TouchStatePropertyKey.BindableProperty;

		public static readonly BindableProperty TouchCountProperty = TouchCountPropertyKey.BindableProperty;

		public static readonly BindableProperty TouchesProperty = TouchesPropertyKey.BindableProperty;

		readonly Dictionary<int, Touch> _touchesDictionary = new Dictionary<int, Touch>();

		int _touchCount;
		List<Touch> _touches;
		TouchState _touchState;

		internal GestureRecognizer()
		{
		}

		public int TouchCount
		{
			get => _touchCount;
		}

		public IReadOnlyList<Touch> Touches
		{
			get => _touches;
		}

		public TouchState TouchState
		{
			get => _touchState;
		}

		protected TouchState PreviousState { get; set; }

		public virtual void OnTouch(View sender, TouchEventArgs eventArgs)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendTouch(View sender, TouchEventArgs eventArgs)
		{
			CollectTouch(eventArgs, sender);

			PreviousState = TouchState;
			_touchState = eventArgs.TouchState;
			_touchCount = _touches.Count;

			OnTouch(sender, eventArgs);
			TouchUpdated?.Invoke(this, eventArgs);

			OnPropertyChanged(nameof(Touches));
			OnPropertyChanged(nameof(TouchCount));
			OnPropertyChanged(nameof(TouchState));

			if (TouchCount == 0)
			{
				_touchState = TouchState.Default;
			}
		}

		public event EventHandler<TouchEventArgs> TouchUpdated;

		void CollectTouch(TouchEventArgs ev, View view)
		{
			foreach (TouchPoint touchPoint in ev.TouchPoints)
			{
				if (touchPoint.TouchState.IsTouching())
				{
					if (_touchesDictionary.TryGetValue(touchPoint.TouchId, out Touch touch))
					{
						touch.TouchPoints.Add(touchPoint);
						var points = new List<Point>();
						foreach (TouchPoint point in touch.TouchPoints)
						{
							if (point.TouchState.IsTouching())
							{
								points.Add(point.Point);
							}
						}

						touch.Gesture = GestureDetector.DetectGesture(points.ToArray());
					}
					else
					{
						_touchesDictionary[touchPoint.TouchId] = new Touch(touchPoint.TouchId, touchPoint, view);
					}
				}
				else if (touchPoint.TouchState.IsFinishedTouch())
				{
					_touchesDictionary.Remove(touchPoint.TouchId);
				}
			}

			_touches = new List<Touch>(_touchesDictionary.Values);
		}

		internal static class GestureDetector
		{
			const double GestureThreshold = 6.0;

			internal static GestureDirection DetectGesture(Point[] points)
			{
				if (points.Length == 0)
				{
					return GestureDirection.None;
				}

				Point first = points[0];
				Point last = points[points.Length - 1];

				var xDiff = first.X - last.X;
				var yDiff = first.Y - last.Y;

				bool up, down, right, left;
				up = down = right = left = false;

				if (Math.Abs(yDiff) > GestureThreshold)
				{
					up = yDiff > 0;
					down = !up;
				}

				if (Math.Abs(xDiff) > GestureThreshold)
				{
					left = xDiff > 0;
					right = !left;
				}

				if ((up || down) && (left || right))
				{
					if (Math.Abs(xDiff) > Math.Abs(yDiff))
					{
						if (Math.Abs(xDiff) / Math.Abs(yDiff) > GestureThreshold)
						{
							up = down = false;
						}
					}
					else
					{
						if (Math.Abs(yDiff) / Math.Abs(xDiff) > GestureThreshold)
						{
							right = left = false;
						}
					}
				}

				var gesture = GestureDirection.None;

				if (up || down)
				{
					if (left || right)
					{
						gesture = GetDirection(points, up, down, right, left);
					}
					else
					{
						gesture = up ? GestureDirection.Up : GestureDirection.Down;
					}
				}
				else if (left || right)
				{
					gesture = right ? GestureDirection.Right : GestureDirection.Left;
				}

				return gesture;
			}

			static GestureDirection GetDirection(Point[] points, bool up, bool down, bool right, bool left)
			{
				var gestureDirection = GestureDirection.None;
				var pointsAboveDiagonal = 0;
				var pointsBelowDiagonal = 0;
				Point first = points[0];
				Point last = points[points.Length - 1];

				foreach (Point point in points)
				{
					var diagonalOnYAxis = (point.X - first.X) * (point.Y - last.Y) / (first.X - last.X) + first.Y;
					if (point.Y > diagonalOnYAxis)
					{
						pointsAboveDiagonal++;
					}
					else
					{
						pointsBelowDiagonal++;
					}
				}

				if (up)
				{
					if (right)
					{
						gestureDirection = pointsAboveDiagonal > pointsBelowDiagonal ? GestureDirection.RightUp : GestureDirection.UpRight;
					}
					else
					{
						gestureDirection = pointsAboveDiagonal > pointsBelowDiagonal ? GestureDirection.LeftUp : GestureDirection.UpLeft;
					}
				}
				else if (down)
				{
					if (right)
					{
						gestureDirection = pointsAboveDiagonal > pointsBelowDiagonal ? GestureDirection.DownRight : GestureDirection.RightDown;
					}
					else
					{
						gestureDirection = pointsAboveDiagonal > pointsBelowDiagonal ? GestureDirection.DownLeft : GestureDirection.LeftDown;
					}
				}

				return gestureDirection;
			}
		}
	}
}