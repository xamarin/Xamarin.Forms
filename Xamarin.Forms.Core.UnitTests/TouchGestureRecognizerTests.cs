using System.Collections.Generic;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class TouchGestureRecognizerTests : BaseTestFixture
	{
		[Test]
		public void OneTouchMoveDown()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Pressed, true) }));

			Assert.AreEqual(TouchState.Pressed, recognizer.State);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 55), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 60), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 70), TouchState.Move, true) }));

			Assert.AreEqual(TouchState.Move, recognizer.State);
			Assert.AreEqual(GestureType.Down, recognizer.Touches[0].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(0, new Point(50, 80), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchMoveLeft()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Pressed, true) }));

			Assert.AreEqual(TouchState.Pressed, recognizer.State);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(48, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(40, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(30, 50), TouchState.Move, true) }));

			Assert.AreEqual(TouchState.Move, recognizer.State);
			Assert.AreEqual(GestureType.Left, recognizer.Touches[0].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(0, new Point(20, 50), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchMoveRight()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Pressed, true) }));

			Assert.AreEqual(TouchState.Pressed, recognizer.State);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(52, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(60, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(70, 50), TouchState.Move, true) }));

			Assert.AreEqual(TouchState.Move, recognizer.State);
			Assert.AreEqual(GestureType.Right, recognizer.Touches[0].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(0, new Point(75, 50), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchMoveUp()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Pressed, true) }));

			Assert.AreEqual(TouchState.Pressed, recognizer.State);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 42), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 35), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 28), TouchState.Move, true) }));

			Assert.AreEqual(TouchState.Move, recognizer.State);
			Assert.AreEqual(GestureType.Up, recognizer.Touches[0].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(0, new Point(50, 20), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchPressed()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(0, new Point(1, 1), TouchState.Pressed, true) }));

			Assert.AreEqual(1, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Pressed, recognizer.State);
			Assert.AreEqual(1, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchPressedAndMoveAndReleased()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(0, new Point(1, 1), TouchState.Pressed, true) }));

			Assert.AreEqual(TouchState.Pressed, recognizer.State);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(1, 2), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(1, 3), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(1, 4), TouchState.Move, true) }));

			Assert.AreEqual(1, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Move, recognizer.State);
			Assert.AreEqual(4, recognizer.Touches[0].TouchPoints.Count);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(0, new Point(1, 5), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchPressedAndReleased()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(0, new Point(1, 1), TouchState.Pressed, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(0, new Point(1, 2), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void TwoTouchMoveDownLeftRight()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(1, new Point(50, 50), TouchState.Pressed, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(2, new Point(50, 50), TouchState.Pressed, true) }));

			Assert.AreEqual(TouchState.Pressed, recognizer.State);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(30, 55), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(70, 60), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(30, 70), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(70, 75), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(30, 75), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(70, 80), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(30, 85), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(70, 85), TouchState.Move, true) }));

			Assert.AreEqual(GestureType.DownLeft, recognizer.Touches[0].Gesture);
			Assert.AreEqual(GestureType.DownRight, recognizer.Touches[1].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(1, new Point(50, 50), TouchState.Released, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Released, new List<TouchPoint> { new TouchPoint(2, new Point(50, 50), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void TwoTouchMoveLeftRightDown()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(1, new Point(50, 50), TouchState.Pressed, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(2, new Point(50, 50), TouchState.Pressed, true) }));

			Assert.AreEqual(TouchState.Pressed, recognizer.State);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(45, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(55, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(40, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(60, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(35, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(65, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(30, 60), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(70, 60), TouchState.Move, true) }));

			Assert.AreEqual(GestureType.LeftDown, recognizer.Touches[0].Gesture);
			Assert.AreEqual(GestureType.RightDown, recognizer.Touches[1].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(1, new Point(50, 50), TouchState.Released, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Released, new List<TouchPoint> { new TouchPoint(2, new Point(50, 50), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void TwoTouchMoveLeftRightUp()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(1, new Point(50, 50), TouchState.Pressed, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(2, new Point(50, 50), TouchState.Pressed, true) }));

			Assert.AreEqual(TouchState.Pressed, recognizer.State);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(45, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(55, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(40, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(60, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(35, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(65, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(30, 40), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(70, 40), TouchState.Move, true) }));

			Assert.AreEqual(GestureType.LeftUp, recognizer.Touches[0].Gesture);
			Assert.AreEqual(GestureType.RightUp, recognizer.Touches[1].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(1, new Point(50, 50), TouchState.Released, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Released, new List<TouchPoint> { new TouchPoint(2, new Point(50, 50), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void TwoTouchMoveUpLeftRight()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(1, new Point(50, 50), TouchState.Pressed, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Pressed, new List<TouchPoint> { new TouchPoint(2, new Point(50, 50), TouchState.Pressed, true) }));

			Assert.AreEqual(TouchState.Pressed, recognizer.State);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(45, 45), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(55, 45), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(1, new Point(35, 35), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<TouchPoint> { new TouchPoint(2, new Point(64, 35), TouchState.Move, true) }));

			Assert.AreEqual(GestureType.UpLeft, recognizer.Touches[0].Gesture);
			Assert.AreEqual(GestureType.UpRight, recognizer.Touches[1].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Released, new List<TouchPoint> { new TouchPoint(1, new Point(50, 50), TouchState.Released, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Released, new List<TouchPoint> { new TouchPoint(2, new Point(50, 50), TouchState.Released, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}
	}
}