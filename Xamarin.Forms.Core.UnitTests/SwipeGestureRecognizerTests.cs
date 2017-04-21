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
	}
}
