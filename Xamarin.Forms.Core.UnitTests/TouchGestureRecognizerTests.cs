using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class TouchGestureRecognizerTests : BaseTestFixture
	{
		[SetUp]
		public override void Setup()
		{
			Device.PlatformServices = new MockPlatformServices();
			base.Setup();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			Device.PlatformServices = null;
		}

		[Test]
		public void OneTouchPressed()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view, new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint>
			{
				new TouchPoint(0,new Point(1,1),TouchState.Pressed,true )
			}));

			Assert.AreEqual(1, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Pressed, recognizer.State);
			Assert.AreEqual(1, recognizer.Touches.Count);
		}

		[Test]
		public void OneTouchPressedAndReleased()
		{
			var recognizer = new TouchGestureRecognizer();
			var view = new View();

			recognizer.SendTouch(view, new TouchEventArgs(1, TouchState.Pressed, new List<TouchPoint>
			{
				new TouchPoint(0,new Point(1,1),TouchState.Pressed,true )
			}));
			recognizer.SendTouch(view, new TouchEventArgs(1, TouchState.Released, new List<TouchPoint>
			{
				new TouchPoint(0,new Point(1,2),TouchState.Released,true )
			}));

			Assert.AreEqual(0, recognizer.TouchCount);
			Assert.AreEqual(TouchState.Default, recognizer.State);
			Assert.AreEqual(0, recognizer.Touches.Count);
		}
	}
}
