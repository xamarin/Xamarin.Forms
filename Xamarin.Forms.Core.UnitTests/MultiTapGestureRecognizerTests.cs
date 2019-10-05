using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class MultiTapGestureRecognizerTests : BaseTestFixture
	{
		[Test]
		public async Task Tap()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new MultiTapGestureRecognizer();
			recognizer.Command = new Command(() => commandCount++);
			recognizer.Tapped += (s, e) => eventCount++;

			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public async Task ThreeTaps()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new MultiTapGestureRecognizer();
			recognizer.NumberOfTapsRequired = 3;
			recognizer.Command = new Command(() => commandCount++);
			recognizer.Tapped += (s, e) => eventCount++;

			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(true, recognizer.IsTapping);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public async Task ThreeTapsFail()
		{
			var eventCount = 0;
			var commandCount = 0;
			var startCommandCount = 0;
			var cancelCommandCount = 0;
			var recognizer = new MultiTapGestureRecognizer();
			recognizer.NumberOfTapsRequired = 3;
			recognizer.Command = new Command(() => commandCount++);
			recognizer.StartedCommand = new Command(() => startCommandCount++);
			recognizer.CancelledCommand = new Command(() => cancelCommandCount++);
			recognizer.Tapped += (s, e) => eventCount++;

			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(true, recognizer.IsTapping);


			await Task.Delay(recognizer.DelayBetweenTaps * 2);


			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			await Task.Delay(recognizer.DelayBetweenTaps * 2);

			Assert.AreEqual(0, eventCount);
			Assert.AreEqual(0, commandCount);
			Assert.AreEqual(2, startCommandCount);
			Assert.AreEqual(2, cancelCommandCount);
		}

		[Test]
		public async Task TapCancel()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new MultiTapGestureRecognizer();
			recognizer.Command = new Command(() => commandCount++);
			recognizer.Tapped += (s, e) => eventCount++;

			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Cancel, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Cancel, true) }));

			Assert.AreEqual(0, eventCount);
			Assert.AreEqual(0, commandCount);
		}

	}
}