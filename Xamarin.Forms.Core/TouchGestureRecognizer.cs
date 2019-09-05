using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xamarin.Forms
{
	public class TouchGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty UseVisualStateManagerProperty =
			BindableProperty.Create(nameof(UseVisualStateManager), typeof(bool), typeof(TouchGestureRecognizer), true);

		public static readonly BindableProperty StateProperty =
			BindableProperty.Create(nameof(State), typeof(TouchState), typeof(TouchGestureRecognizer), TouchState.Default);

		public static readonly BindableProperty TouchCountProperty =
			BindableProperty.Create(nameof(TouchCount), typeof(int), typeof(TouchGestureRecognizer), 0);

		readonly Dictionary<int, Touch> _touches = new Dictionary<int, Touch>();

		public TouchState State
		{
			get => (TouchState)GetValue(StateProperty);
			set => SetValue(StateProperty, value);
		}

		public int TouchCount
		{
			get => (int)GetValue(TouchCountProperty);
			private set => SetValue(TouchCountProperty, value);
		}

		public List<Touch> Touches => _touches.Values.ToList();

		public bool UseVisualStateManager
		{
			get => (bool)GetValue(UseVisualStateManagerProperty);
			set => SetValue(UseVisualStateManagerProperty, value);
		}

		public View View { get; private set; }

		protected TouchState PreviousState { get; set; }

		public virtual void OnTouch(View sender, TouchEventArgs eventArgs)
		{
			//override it and add your custom logic here.
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendTouch(View sender, TouchEventArgs eventArgs)
		{
			if (View != sender)
			{
				View = sender;
			}

			CollectTouch(eventArgs);

			PreviousState = State;
			State = eventArgs.TouchState;
			TouchCount = _touches.Count;

			TouchUpdated?.Invoke(this, eventArgs);
			OnTouch(sender, eventArgs);

			if (UseVisualStateManager)
			{
				VisualStateManager.GoToState(sender, State.ToString());
			}

			if (_touches.Count == 0)
			{
				State = TouchState.Default;
			}
		}

		public event EventHandler<TouchEventArgs> TouchUpdated;

		void CollectTouch(TouchEventArgs ev)
		{
			foreach (TouchPoint touchPoint in ev.TouchPoints)
			{
				if (touchPoint.TouchState.IsTouching())
				{
					if (_touches.TryGetValue(touchPoint.TouchId, out Touch touch))
					{
						touch.TouchPoints.Add(touchPoint);
						touch.Gesture = GestureDetector.DetectGesture(touch.TouchPoints
							.Where(w => w.TouchState.IsTouching()).Select(s => s.Point).ToArray());
					}
					else
					{
						_touches[touchPoint.TouchId] = new Touch(touchPoint.TouchId, touchPoint, View);
					}
				}
				else if (touchPoint.TouchState.IsFinishedTouch())
				{
					_touches.Remove(touchPoint.TouchId);
				}
			}
		}

		static class GestureDetector
		{
			const int GestureThreshold = 6;

			public static GestureType DetectGesture(Point[] points)
			{
				if (points.Length == 0)
				{
					return GestureType.None;
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
						if (Math.Abs(xDiff) / Math.Abs(yDiff) > 7.0)
						{
							up = down = false;
						}
					}
					else
					{
						if (Math.Abs(yDiff) / Math.Abs(xDiff) > 7.0)
						{
							right = left = false;
						}
					}
				}

				var gesture = GestureType.None;

				if (up || down)
				{
					if (left || right)
					{
						gesture = ComplexGesture(points, up, down, right, left);
					}
					else
					{
						gesture = up ? GestureType.Up : GestureType.Down;
					}
				}
				else if (left || right)
				{
					gesture = right ? GestureType.Right : GestureType.Left;
				}

				return gesture;
			}

			static GestureType ComplexGesture(Point[] points, bool up, bool down, bool right, bool left)
			{
				var gestureType = GestureType.None;
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
						gestureType = pointsAboveDiagonal > pointsBelowDiagonal ? GestureType.RightUp : GestureType.UpRight;
					}
					else
					{
						gestureType = pointsAboveDiagonal > pointsBelowDiagonal ? GestureType.LeftUp : GestureType.UpLeft;
					}
				}
				else if (down)
				{
					if (right)
					{
						gestureType = pointsAboveDiagonal > pointsBelowDiagonal ? GestureType.DownRight : GestureType.RightDown;
					}
					else
					{
						gestureType = pointsAboveDiagonal > pointsBelowDiagonal ? GestureType.DownLeft : GestureType.LeftDown;
					}
				}

				return gestureType;
			}
		}
	}
}