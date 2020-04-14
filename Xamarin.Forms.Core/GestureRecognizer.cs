using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xamarin.Forms
{
	public class GestureRecognizer : Element, IGestureRecognizer
	{
		readonly Dictionary<int, Touch> _touchesDictionary = new Dictionary<int, Touch>();

		internal GestureRecognizer()
		{
		}

		public IReadOnlyList<Touch> Touches { get; private set; } = new List<Touch>();

		public event EventHandler<TouchEventArgs> TouchUpdated;

		public virtual void OnTouch(View sender, GestureEventArgs eventArgs)
		{
			// empty
		}


		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendTouch(View sender, TouchEventArgs eventArgs)
		{
			CollectTouch(eventArgs, sender);

			//var ev = new GestureEventArgs(0, TouchState.Cancel,new List<RawTouchPoint>());
			//OnTouch(sender, ev);
			TouchUpdated?.Invoke(this, eventArgs);
		}

		void CollectTouch(TouchEventArgs ev, View view)
		{
			foreach (RawTouchPoint touchPoint in ev.TouchPoints)
			{
				if (touchPoint.TouchState.IsTouching())
				{
					if (_touchesDictionary.TryGetValue(touchPoint.TouchId, out Touch touch))
					{
						touch.TouchPoints.Add(touchPoint);
						var points = new List<Point>();
						foreach (RawTouchPoint point in touch.TouchPoints)
						{
							if (point.TouchState.IsTouching())
							{
								points.Add(point.Point);
							}
						}

						touch.Gesture = GestureDetector.DetectGesture(points);
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

			Touches = new List<Touch>(_touchesDictionary.Values);
		}

		internal static class GestureDetector
		{
			const double GestureThreshold = 6.0;

			internal static GestureDirection DetectGesture(List<Point> points)
			{
				if (points.Count == 0)
				{
					return GestureDirection.None;
				}

				Point first = points[0];
				Point last = points[points.Count - 1];

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

			static GestureDirection GetDirection(List<Point> points, bool up, bool down, bool right, bool left)
			{
				var gestureDirection = GestureDirection.None;
				var pointsAboveDiagonal = 0;
				var pointsBelowDiagonal = 0;
				Point first = points[0];
				Point last = points[points.Count - 1];

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

		public struct RawTouchPoint
		{
			public RawTouchPoint(int touchId, Point point, TouchState state, bool isInOriginalView)
			{
				TouchId = touchId;
				Point = point;
				TouchState = state;
				IsInOriginalView = isInOriginalView;
			}

			public Point Point { get; }

			public bool IsInOriginalView { get; }

			public int TouchId { get; }

			public TouchState TouchState { get; }
		}
	}
}