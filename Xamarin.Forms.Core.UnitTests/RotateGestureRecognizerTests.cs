using System.Collections.Generic;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class RotateGestureRecognizerTests : BaseTestFixture
	{
		[Test]
		public void RotateRight()
		{
			var eventCount = 0;
			var startedCommandCount = 0;
			var cancelledCommandCount = 0;
			var commandCount = 0;
			var recognizer = new RotateGestureRecognizer();
			recognizer.DegreeRotationThreshold = 10;
			recognizer.Rotated += (s, e) => eventCount++;
			recognizer.StartedCommand = new Command(() => startedCommandCount++);
			recognizer.CancelledCommand = new Command(() => cancelledCommandCount++);
			recognizer.Command = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(256, 228), TouchState.Press, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(55, 101), TouchState.Press, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move,
					new List<TouchPoint>
					{
						new TouchPoint(1, new Point(256, 227), TouchState.Move, true),
						new TouchPoint(1, new Point(256, 226), TouchState.Move, true)
					}));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move,
					new List<TouchPoint>
					{
						new TouchPoint(0, new Point(55, 103), TouchState.Move, true), 
						new TouchPoint(0, new Point(55, 107), TouchState.Move, true)
					}));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move,
					new List<TouchPoint>
					{
						new TouchPoint(1, new Point(256, 230), TouchState.Move, true),
						new TouchPoint(1, new Point(256, 235), TouchState.Move, true)
					}));

			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Move,
					new List<TouchPoint>
					{
						new TouchPoint(0, new Point(55, 110), TouchState.Move, true), 
						new TouchPoint(0, new Point(55, 115), TouchState.Move, true)
					}));

			Assert.AreEqual(true, recognizer.IsRotating);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Cancel, new List<TouchPoint> { new TouchPoint(0, new Point(256, 228), TouchState.Cancel, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(2, TouchState.Cancel, new List<TouchPoint> { new TouchPoint(0, new Point(55, 101), TouchState.Cancel, true) }));

			Assert.AreEqual(false, recognizer.IsRotating);
			Assert.AreEqual(1, startedCommandCount);
			Assert.AreEqual(2, commandCount);
			Assert.AreEqual(1, cancelledCommandCount);
			Assert.AreEqual(5, eventCount);
		}
	}
}