using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class LongPressGestureRecognizerTests : BaseTestFixture
	{

		[Test]
		public async Task LongPress()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new LongPressGestureRecognizer();
			recognizer.Command = new Command(()=> commandCount++);
			recognizer.LongPressed += (s, e) => eventCount++;

			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			await Task.Delay(recognizer.PressDuration * 2);

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}


		[Test]
		public async Task StartedAndFinished()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new LongPressGestureRecognizer();
			recognizer.StartedCommand = new Command(() => commandCount++);
			recognizer.FinishedCommand = new Command(() => commandCount++);
			recognizer.Pressed += (s, e) => eventCount++;
			recognizer.Released += (s, e) => eventCount++;
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			await Task.Delay(recognizer.PressDuration * 2);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(2, eventCount);
			Assert.AreEqual(2, commandCount);
		}

		[Test]
		public async Task StartedAndCancel()
		{
			var eventCount = 0;
			var longPressEventCount = 0;
			var commandCount = 0;
			var recognizer = new LongPressGestureRecognizer();
			recognizer.StartedCommand = new Command(() => commandCount++);
			recognizer.CancelledCommand = new Command(() => commandCount++);
			recognizer.Pressed += (s, e) => eventCount++;
			recognizer.Released += (s, e) => eventCount++;
			recognizer.LongPressed += (s, e) => longPressEventCount++;
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			await Task.Delay(recognizer.PressDuration / 2);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(2, eventCount);
			Assert.AreEqual(2, commandCount);
			Assert.AreEqual(0, longPressEventCount);
		}

		[Test]
		public async Task LongPressMoveNotAllowed()
		{
			var eventCount = 0;
			var commandCount = 0;
			var cancelCommandCount = 0;
			var recognizer = new LongPressGestureRecognizer();
			recognizer.Command = new Command(() => commandCount++);
			recognizer.CancelledCommand = new Command(() => cancelCommandCount++);
			recognizer.LongPressed += (s, e) => eventCount++;

			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			await Task.Delay(recognizer.PressDuration / 2);

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(55, 55), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(60, 60), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(65, 65), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(70, 70), TouchState.Move, true) }));

			await Task.Delay(recognizer.PressDuration / 2);

			Assert.AreEqual(0, eventCount);
			Assert.AreEqual(0, commandCount);
			Assert.AreEqual(1, cancelCommandCount);
		}

	}
}