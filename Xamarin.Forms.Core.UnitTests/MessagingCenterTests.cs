using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class MessagingCenterTests : BaseTestFixture
	{
		[Test]
		public void SingleSubscriber ()
		{
			string sentMessage = null;
			Messaging.Instance.Subscribe<MessagingCenterTests, string> (this, "SimpleTest", (sender, args) => sentMessage = args);

			Messaging.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.EqualTo ("My Message"));

			Messaging.Instance.Unsubscribe<MessagingCenterTests, string> (this, "SimpleTest");
		}

		[Test]
		public void Filter ()
		{
			string sentMessage = null;
			Messaging.Instance.Subscribe<MessagingCenterTests, string> (this, "SimpleTest", (sender, args) => sentMessage = args, this);

			Messaging.Instance.Send (new MessagingCenterTests (), "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.Null);

			Messaging.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.EqualTo ("My Message"));

			Messaging.Instance.Unsubscribe<MessagingCenterTests, string> (this, "SimpleTest");
		}

		[Test]
		public void MultiSubscriber ()
		{
			var sub1 = new object ();
			var sub2 = new object ();
			string sentMessage1 = null;
			string sentMessage2 = null;
			Messaging.Instance.Subscribe<MessagingCenterTests, string> (sub1, "SimpleTest", (sender, args) => sentMessage1 = args);
			Messaging.Instance.Subscribe<MessagingCenterTests, string> (sub2, "SimpleTest", (sender, args) => sentMessage2 = args);

			Messaging.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage1, Is.EqualTo ("My Message"));
			Assert.That (sentMessage2, Is.EqualTo ("My Message"));

			Messaging.Instance.Unsubscribe<MessagingCenterTests, string> (sub1, "SimpleTest");
			Messaging.Instance.Unsubscribe<MessagingCenterTests, string> (sub2, "SimpleTest");
		}

		[Test]
		public void Unsubscribe ()
		{
			string sentMessage = null;
			Messaging.Instance.Subscribe<MessagingCenterTests, string> (this, "SimpleTest", (sender, args) => sentMessage = args);
			Messaging.Instance.Unsubscribe<MessagingCenterTests, string> (this, "SimpleTest");

			Messaging.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.EqualTo (null));
		}

		[Test]
		public void SendWithoutSubscribers ()
		{
			Assert.DoesNotThrow (() => Messaging.Instance.Send (this, "SimpleTest", "My Message"));
		}

		[Test]
		public void NoArgSingleSubscriber ()
		{
			bool sentMessage = false;
			Messaging.Instance.Subscribe<MessagingCenterTests> (this, "SimpleTest", sender => sentMessage = true);

			Messaging.Instance.Send (this, "SimpleTest");

			Assert.That (sentMessage, Is.True);

			Messaging.Instance.Unsubscribe<MessagingCenterTests> (this, "SimpleTest");
		}

		[Test]
		public void NoArgFilter ()
		{
			bool sentMessage = false;
			Messaging.Instance.Subscribe (this, "SimpleTest", (sender) => sentMessage = true, this);

			Messaging.Instance.Send (new MessagingCenterTests (), "SimpleTest");

			Assert.That (sentMessage, Is.False);

			Messaging.Instance.Send (this, "SimpleTest");

			Assert.That (sentMessage, Is.True);

			Messaging.Instance.Unsubscribe<MessagingCenterTests> (this, "SimpleTest");
		}

		[Test]
		public void NoArgMultiSubscriber ()
		{
			var sub1 = new object ();
			var sub2 = new object ();
			bool sentMessage1 = false;
			bool sentMessage2 = false;
			Messaging.Instance.Subscribe<MessagingCenterTests> (sub1, "SimpleTest", (sender) => sentMessage1 = true);
			Messaging.Instance.Subscribe<MessagingCenterTests> (sub2, "SimpleTest", (sender) => sentMessage2 = true);

			Messaging.Instance.Send (this, "SimpleTest");

			Assert.That (sentMessage1, Is.True);
			Assert.That (sentMessage2, Is.True);

			Messaging.Instance.Unsubscribe<MessagingCenterTests> (sub1, "SimpleTest");
			Messaging.Instance.Unsubscribe<MessagingCenterTests> (sub2, "SimpleTest");
		}

		[Test]
		public void NoArgUnsubscribe ()
		{
			bool sentMessage = false;
			Messaging.Instance.Subscribe<MessagingCenterTests> (this, "SimpleTest", (sender) => sentMessage = true);
			Messaging.Instance.Unsubscribe<MessagingCenterTests> (this, "SimpleTest");

			Messaging.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.False);
		}

		[Test]
		public void NoArgSendWithoutSubscribers ()
		{
			Assert.DoesNotThrow (() => Messaging.Instance.Send (this, "SimpleTest"));
		}

		[Test]
		public void ThrowOnNullArgs ()
		{
			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Subscribe<MessagingCenterTests, string> (null, "Foo", (sender, args) => { }));
			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Subscribe<MessagingCenterTests, string> (this, null, (sender, args) => { }));
			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Subscribe<MessagingCenterTests, string> (this, "Foo", null));

			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Subscribe<MessagingCenterTests> (null, "Foo", (sender) => { }));
			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Subscribe<MessagingCenterTests> (this, null, (sender) => { }));
			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Subscribe<MessagingCenterTests> (this, "Foo", null));

			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Send<MessagingCenterTests, string> (null, "Foo", "Bar"));
			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Send<MessagingCenterTests, string> (this, null, "Bar"));

			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Send<MessagingCenterTests> (null, "Foo"));
			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Send<MessagingCenterTests> (this, null));

			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Unsubscribe<MessagingCenterTests> (null, "Foo"));
			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Unsubscribe<MessagingCenterTests> (this, null));

			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Unsubscribe<MessagingCenterTests, string> (null, "Foo"));
			Assert.Throws<ArgumentNullException> (() => Messaging.Instance.Unsubscribe<MessagingCenterTests, string> (this, null));
		}

		[Test]
		public void UnsubscribeInCallback ()
		{
			int messageCount = 0;

			var subscriber1 = new object ();
			var subscriber2 = new object ();

			Messaging.Instance.Subscribe<MessagingCenterTests> (subscriber1, "SimpleTest", (sender) => {
				messageCount++;
				Messaging.Instance.Unsubscribe<MessagingCenterTests> (subscriber2, "SimpleTest");
			});

			Messaging.Instance.Subscribe<MessagingCenterTests> (subscriber2, "SimpleTest", (sender) => {
				messageCount++;
				Messaging.Instance.Unsubscribe<MessagingCenterTests> (subscriber1, "SimpleTest");
			});

			Messaging.Instance.Send (this, "SimpleTest");

			Assert.AreEqual (1, messageCount);
		}

		[Test]
		public void SubscriberShouldBeCollected()
		{
			new Action(() =>
			{
				var subscriber = new TestSubcriber();
				MessagingCenter.Subscribe<TestPublisher>(subscriber, "test", p => Assert.Fail());
			})();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			var pub = new TestPublisher();
			pub.Test(); // Assert.Fail() shouldn't be called, because the TestSubcriber object should have ben GCed
		}

		[Test]
		public void ShouldBeCollectedIfCallbackTargetIsSubscriber()
		{
			WeakReference wr = null;

			new Action(() =>
			{
				var subscriber = new TestSubcriber();

				wr = new WeakReference(subscriber);

				subscriber.SubscribeToTestMessages();
			})();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			var pub = new TestPublisher();
			pub.Test();

			Assert.IsFalse(wr.IsAlive); // The Action target and subscriber were the same object, so both could be collected
		}

		[Test]
		public void NotCollectedIfSubscriberIsNotTheCallbackTarget()
		{
			WeakReference wr = null;

			new Action(() =>
			{
				var subscriber = new TestSubcriber();

				wr = new WeakReference(subscriber);

				// This creates a closure, so the callback target is not 'subscriber', but an instancce of a compiler generated class 
				// So MC has to keep a strong reference to it, and 'subscriber' won't be collectable
				MessagingCenter.Subscribe<TestPublisher>(subscriber, "test", p => subscriber.SetSuccess());
			})();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			
			Assert.IsTrue(wr.IsAlive); // The closure in Subscribe should be keeping the subscriber alive
			Assert.IsNotNull(wr.Target as TestSubcriber);

			Assert.IsFalse(((TestSubcriber)wr.Target).Successful);

			var pub = new TestPublisher();
			pub.Test();

			Assert.IsTrue(((TestSubcriber)wr.Target).Successful);  // Since it's still alive, the subscriber should still have received the message and updated the property
		}

		[Test]
		public void SubscriberCollectableAfterUnsubscribeEvenIfHeldByClosure()
		{
			WeakReference wr = null;

			new Action(() =>
			{
				var subscriber = new TestSubcriber();

				wr = new WeakReference(subscriber);

				MessagingCenter.Subscribe<TestPublisher>(subscriber, "test", p => subscriber.SetSuccess());
			})();

			Assert.IsNotNull(wr.Target as TestSubcriber); 

			MessagingCenter.Unsubscribe<TestPublisher>(wr.Target, "test");
			
			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.IsFalse(wr.IsAlive); // The Action target and subscriber were the same object, so both could be collected
		}

		[Test]
		public void StaticCallback()
		{
			int i = 4;

			var subscriber = new TestSubcriber();

			MessagingCenter.Subscribe<TestPublisher>(subscriber, "test", p => MessagingCenterTestsCallbackSource.Increment(ref i));

			GC.Collect();
			GC.WaitForPendingFinalizers();

			var pub = new TestPublisher();
			pub.Test();

			Assert.IsTrue(i == 5, "The static method should have incremented 'i'"); 
		}

		[Test]
		public void NothingShouldBeCollected()
		{
			var success = false;

			var subscriber = new TestSubcriber();
			
			var source = new MessagingCenterTestsCallbackSource();
			MessagingCenter.Subscribe<TestPublisher>(subscriber, "test", p => source.SuccessCallback(ref success));
			
			GC.Collect();
			GC.WaitForPendingFinalizers();

			var pub = new TestPublisher();
			pub.Test(); 

			Assert.True(success); // TestCallbackSource.SuccessCallback() should be invoked to make success == true
		}

		class TestSubcriber
		{
			public void SetSuccess()
			{
				Successful = true;
			}

			public bool Successful { get; private set; }

			public void SubscribeToTestMessages()
			{
				MessagingCenter.Subscribe<TestPublisher>(this, "test", p => SetSuccess());
			}
		}

		class TestPublisher
		{
			public void Test()
			{
				MessagingCenter.Send(this, "test");
			}
		}

		public class MessagingCenterTestsCallbackSource
		{
			public void SuccessCallback(ref bool success)
			{
				success = true;
			}

			public static void Increment(ref int i)
			{
				i = i + 1;
			}
		}
	}
}
