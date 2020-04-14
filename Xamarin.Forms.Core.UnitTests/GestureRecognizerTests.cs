using System.Collections.Generic;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class GestureRecognizerTests : BaseTestFixture
	{
		[Test]
		public void OneTouchMoveDown()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			Assert.AreEqual(TouchState.Press, recognizer.TouchState);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 55), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 60), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 70), TouchState.Move, true) }));

			Assert.AreEqual(TouchState.Move, recognizer.TouchState);
			Assert.AreEqual(GestureDirection.Down, recognizer.Touches[0].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 80), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchMoveLeft()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			Assert.AreEqual(TouchState.Press, recognizer.TouchState);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(48, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(40, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(30, 50), TouchState.Move, true) }));

			Assert.AreEqual(TouchState.Move, recognizer.TouchState);
			Assert.AreEqual(GestureDirection.Left, recognizer.Touches[0].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(20, 50), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchMoveRight()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			Assert.AreEqual(TouchState.Press, recognizer.TouchState);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(52, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(60, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(70, 50), TouchState.Move, true) }));

			Assert.AreEqual(TouchState.Move, recognizer.TouchState);
			Assert.AreEqual(GestureDirection.Right, recognizer.Touches[0].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(75, 50), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchMoveUp()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			Assert.AreEqual(TouchState.Press, recognizer.TouchState);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 42), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 35), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 28), TouchState.Move, true) }));

			Assert.AreEqual(TouchState.Move, recognizer.TouchState);
			Assert.AreEqual(GestureDirection.Up, recognizer.Touches[0].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(50, 20), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchPressed()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(1, 1), TouchState.Press, true) }));

			Assert.AreEqual(1, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Press, recognizer.TouchState);
			Assert.AreEqual(1, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchPressedAndMoveAndReleased()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(1, 1), TouchState.Press, true) }));

			Assert.AreEqual(TouchState.Press, recognizer.TouchState);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(1, 2), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(1, 3), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(1, 4), TouchState.Move, true) }));

			Assert.AreEqual(1, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Move, recognizer.TouchState);
			Assert.AreEqual(4, recognizer.Touches[0].GestureRecognizer.RawTouchPoints.Count);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(1, 5), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchPressedAndReleased()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(1, 1), TouchState.Press, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(0, new Point(1, 2), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void TwoTouchMoveDownLeftRight()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(50, 50), TouchState.Press, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(50, 50), TouchState.Press, true) }));

			Assert.AreEqual(TouchState.Press, recognizer.TouchState);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(30, 55), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(70, 60), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(30, 70), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(70, 75), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(30, 75), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(70, 80), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(30, 85), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(70, 85), TouchState.Move, true) }));

			Assert.AreEqual(GestureDirection.DownLeft, recognizer.Touches[0].Gesture);
			Assert.AreEqual(GestureDirection.DownRight, recognizer.Touches[1].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(50, 50), TouchState.Release, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void TwoTouchMoveLeftRightDown()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(50, 50), TouchState.Press, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(50, 50), TouchState.Press, true) }));

			Assert.AreEqual(TouchState.Press, recognizer.TouchState);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(45, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(55, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(40, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(60, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(35, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(65, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(30, 60), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(70, 60), TouchState.Move, true) }));

			Assert.AreEqual(GestureDirection.LeftDown, recognizer.Touches[0].Gesture);
			Assert.AreEqual(GestureDirection.RightDown, recognizer.Touches[1].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(50, 50), TouchState.Release, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void TwoTouchMoveLeftRightUp()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(50, 50), TouchState.Press, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(50, 50), TouchState.Press, true) }));

			Assert.AreEqual(TouchState.Press, recognizer.TouchState);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(45, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(55, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(40, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(60, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(35, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(65, 50), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(30, 40), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(70, 40), TouchState.Move, true) }));

			Assert.AreEqual(GestureDirection.LeftUp, recognizer.Touches[0].Gesture);
			Assert.AreEqual(GestureDirection.RightUp, recognizer.Touches[1].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(50, 50), TouchState.Release, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}

		[Test]
		public void TwoTouchMoveUpLeftRight()
		{
			var recognizer = new GestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(50, 50), TouchState.Press, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Press, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(50, 50), TouchState.Press, true) }));

			Assert.AreEqual(TouchState.Press, recognizer.TouchState);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(45, 45), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(55, 45), TouchState.Move, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(35, 35), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(64, 35), TouchState.Move, true) }));

			Assert.AreEqual(GestureDirection.UpLeft, recognizer.Touches[0].Gesture);
			Assert.AreEqual(GestureDirection.UpRight, recognizer.Touches[1].Gesture);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(1, new Point(50, 50), TouchState.Release, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Release, new List<GestureRecognizer.RawTouchPoint> { new GestureRecognizer.RawTouchPoint(2, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.TouchState);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}
	}
}