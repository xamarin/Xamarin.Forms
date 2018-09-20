using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class WeakEventManagerTests
	{
		static int s_count;

		static void Handler(object sender, EventArgs eventArgs)
		{
			s_count++;
		}

		internal class TestEventSource
		{
			readonly WeakEventManager _weakEventManager;

			public TestEventSource()
			{
				_weakEventManager = new WeakEventManager();
			}

			public void FireTestEvent()
			{
				OnTestEvent();
			}

			internal event EventHandler TestEvent
			{
				add { _weakEventManager.AddEventHandler(nameof(TestEvent), value); }
				remove { _weakEventManager.RemoveEventHandler(nameof(TestEvent), value); }
			}

			void OnTestEvent()
			{
				_weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(TestEvent));
			}
		}

		internal class TestSubscriber
		{
			public void Subscribe(TestEventSource source)
			{
				source.TestEvent += SourceOnTestEvent;
			}

			void SourceOnTestEvent(object sender, EventArgs eventArgs)
			{
				Assert.Fail();
			}
		}

		
		public void AddHandlerWithEmptyEventNameThrowsException()
		{
			var wem = new WeakEventManager();
			Assert.Throws<ArgumentNullException>(() => wem.AddEventHandler("", (sender, args) => { }));
		}

		
		public void AddHandlerWithNullEventHandlerThrowsException()
		{
			var wem = new WeakEventManager();
			Assert.Throws<ArgumentNullException>(() => wem.AddEventHandler("test", null));
		}

		
		public void AddHandlerWithNullEventNameThrowsException()
		{
			var wem = new WeakEventManager();
			Assert.Throws<ArgumentNullException>(() => wem.AddEventHandler(null, (sender, args) => { }));
		}

		
		public void CanRemoveStaticEventHandler()
		{
			int beforeRun = s_count;

			var source = new TestEventSource();
			source.TestEvent += Handler;
			source.TestEvent -= Handler;

			source.FireTestEvent();

			Assert.IsTrue(s_count == beforeRun);
		}

		
		public void EventHandlerCalled()
		{
			var called = false;

			var source = new TestEventSource();
			source.TestEvent += (sender, args) => { called = true; };

			source.FireTestEvent();

			Assert.IsTrue(called);
		}

		
		public void FiringEventWithoutHandlerShouldNotThrow()
		{
			var source = new TestEventSource();
			source.FireTestEvent();
		}

		
		public void MultipleHandlersCalled()
		{
			var called1 = false;
			var called2 = false;

			var source = new TestEventSource();
			source.TestEvent += (sender, args) => { called1 = true; };
			source.TestEvent += (sender, args) => { called2 = true; };
			source.FireTestEvent();

			Assert.IsTrue(called1 && called2);
		}

		
		public void RemoveHandlerWithEmptyEventNameThrowsException()
		{
			var wem = new WeakEventManager();
			Assert.Throws<ArgumentNullException>(() => wem.RemoveEventHandler("", (sender, args) => { }));
		}

		
		public void RemoveHandlerWithNullEventHandlerThrowsException()
		{
			var wem = new WeakEventManager();
			Assert.Throws<ArgumentNullException>(() => wem.RemoveEventHandler("test", null));
		}

		
		public void RemoveHandlerWithNullEventNameThrowsException()
		{
			var wem = new WeakEventManager();
			Assert.Throws<ArgumentNullException>(() => wem.RemoveEventHandler(null, (sender, args) => { }));
		}

		
		public void RemovingNonExistentHandlersShouldNotThrow()
		{
			var wem = new WeakEventManager();
			wem.RemoveEventHandler("fake", (sender, args) => { });
			wem.RemoveEventHandler("alsofake", Handler);
		}

		
		public void StaticHandlerShouldRun()
		{
			int beforeRun = s_count;

			var source = new TestEventSource();
			source.TestEvent += Handler;

			source.FireTestEvent();

			Assert.IsTrue(s_count > beforeRun);
		}

		
		public void VerifySubscriberCanBeCollected()
		{
			WeakReference wr = null;
			var source = new TestEventSource();
			new Action(() =>
			{
				var ts = new TestSubscriber();
				wr = new WeakReference(ts);
				ts.Subscribe(source);
			})();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.IsNotNull(wr);
			Assert.IsFalse(wr.IsAlive);

			// The handler for this calls Assert.Fail, so if the subscriber has not been collected
			// the handler will be called and the test will fail
			source.FireTestEvent();
		}
	}
}