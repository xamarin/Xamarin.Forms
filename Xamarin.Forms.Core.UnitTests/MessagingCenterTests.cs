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
	}
}
