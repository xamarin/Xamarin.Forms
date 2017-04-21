using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class SwipeGestureRecognizerTests : BaseTestFixture
	{
		[Test]
		public void Constructor()
		{
			var swipe = new SwipeGestureRecognizer();

			Assert.AreEqual(null, swipe.Command);
			Assert.AreEqual(null, swipe.CommandParameter);
			Assert.AreEqual(100, swipe.Threshold);
		}

		[Test]
		public void CallbackPassesParameter()
		{
			var view = new View();
			var swipe = new SwipeGestureRecognizer();
			swipe.CommandParameter = "Hello";

			object result = null;
			swipe.Command = new Command(o => result = o);

			swipe.SendSwiped(view, SwipeDirection.Left);
			Assert.AreEqual(result, swipe.CommandParameter);
		}

		[Test]
		public void SwipedEventDirectionMatchesTotalXTest()
		{
			var view = new View();
			var swipe = new SwipeGestureRecognizer();

			SwipeDirection direction = SwipeDirection.Up;
			swipe.Swiped += (object sender, SwipedEventArgs e) =>
			{
				direction = e.Direction;
			};

			((ISwipeGestureController)swipe).SendSwipe(view, totalX: -150, totalY: 10);
			Assert.AreEqual(SwipeDirection.Left, direction);

			((ISwipeGestureController)swipe).SendSwipe(view, totalX: 150, totalY: 10);
			Assert.AreEqual(SwipeDirection.Right, direction);
		}

		[Test]
		public void SwipedEventDirectionMatchesTotalYTest()
		{
			var view = new View();
			var swipe = new SwipeGestureRecognizer();

			SwipeDirection direction = SwipeDirection.Left;
			swipe.Swiped += (object sender, SwipedEventArgs e) =>
			{
				direction = e.Direction;
			};

			((ISwipeGestureController)swipe).SendSwipe(view, totalX: 10, totalY: -150);
			Assert.AreEqual(SwipeDirection.Up, direction);

			((ISwipeGestureController)swipe).SendSwipe(view, totalX: 10, totalY: 150);
			Assert.AreEqual(SwipeDirection.Down, direction);
		}
	}
}