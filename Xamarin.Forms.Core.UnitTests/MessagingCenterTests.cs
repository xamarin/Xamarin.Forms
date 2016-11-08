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
			MessagingCenter.Instance.Subscribe<MessagingCenterTests, string> (this, "SimpleTest", (sender, args) => sentMessage = args);

			MessagingCenter.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.EqualTo ("My Message"));

			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests, string> (this, "SimpleTest");
		}

		[Test]
		public void Filter ()
		{
			string sentMessage = null;
			MessagingCenter.Instance.Subscribe<MessagingCenterTests, string> (this, "SimpleTest", (sender, args) => sentMessage = args, this);

			MessagingCenter.Instance.Send (new MessagingCenterTests (), "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.Null);

			MessagingCenter.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.EqualTo ("My Message"));

			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests, string> (this, "SimpleTest");
		}

		[Test]
		public void MultiSubscriber ()
		{
			var sub1 = new object ();
			var sub2 = new object ();
			string sentMessage1 = null;
			string sentMessage2 = null;
			MessagingCenter.Instance.Subscribe<MessagingCenterTests, string> (sub1, "SimpleTest", (sender, args) => sentMessage1 = args);
			MessagingCenter.Instance.Subscribe<MessagingCenterTests, string> (sub2, "SimpleTest", (sender, args) => sentMessage2 = args);

			MessagingCenter.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage1, Is.EqualTo ("My Message"));
			Assert.That (sentMessage2, Is.EqualTo ("My Message"));

			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests, string> (sub1, "SimpleTest");
			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests, string> (sub2, "SimpleTest");
		}

		[Test]
		public void Unsubscribe ()
		{
			string sentMessage = null;
			MessagingCenter.Instance.Subscribe<MessagingCenterTests, string> (this, "SimpleTest", (sender, args) => sentMessage = args);
			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests, string> (this, "SimpleTest");

			MessagingCenter.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.EqualTo (null));
		}

		[Test]
		public void SendWithoutSubscribers ()
		{
			Assert.DoesNotThrow (() => MessagingCenter.Instance.Send (this, "SimpleTest", "My Message"));
		}

		[Test]
		public void NoArgSingleSubscriber ()
		{
			bool sentMessage = false;
			MessagingCenter.Instance.Subscribe<MessagingCenterTests> (this, "SimpleTest", sender => sentMessage = true);

			MessagingCenter.Instance.Send (this, "SimpleTest");

			Assert.That (sentMessage, Is.True);

			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests> (this, "SimpleTest");
		}

		[Test]
		public void NoArgFilter ()
		{
			bool sentMessage = false;
			MessagingCenter.Instance.Subscribe (this, "SimpleTest", (sender) => sentMessage = true, this);

			MessagingCenter.Instance.Send (new MessagingCenterTests (), "SimpleTest");

			Assert.That (sentMessage, Is.False);

			MessagingCenter.Instance.Send (this, "SimpleTest");

			Assert.That (sentMessage, Is.True);

			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests> (this, "SimpleTest");
		}

		[Test]
		public void NoArgMultiSubscriber ()
		{
			var sub1 = new object ();
			var sub2 = new object ();
			bool sentMessage1 = false;
			bool sentMessage2 = false;
			MessagingCenter.Instance.Subscribe<MessagingCenterTests> (sub1, "SimpleTest", (sender) => sentMessage1 = true);
			MessagingCenter.Instance.Subscribe<MessagingCenterTests> (sub2, "SimpleTest", (sender) => sentMessage2 = true);

			MessagingCenter.Instance.Send (this, "SimpleTest");

			Assert.That (sentMessage1, Is.True);
			Assert.That (sentMessage2, Is.True);

			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests> (sub1, "SimpleTest");
			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests> (sub2, "SimpleTest");
		}

		[Test]
		public void NoArgUnsubscribe ()
		{
			bool sentMessage = false;
			MessagingCenter.Instance.Subscribe<MessagingCenterTests> (this, "SimpleTest", (sender) => sentMessage = true);
			MessagingCenter.Instance.Unsubscribe<MessagingCenterTests> (this, "SimpleTest");

			MessagingCenter.Instance.Send (this, "SimpleTest", "My Message");

			Assert.That (sentMessage, Is.False);
		}

		[Test]
		public void NoArgSendWithoutSubscribers ()
		{
			Assert.DoesNotThrow (() => MessagingCenter.Instance.Send (this, "SimpleTest"));
		}

		[Test]
		public void ThrowOnNullArgs ()
		{
			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Subscribe<MessagingCenterTests, string> (null, "Foo", (sender, args) => { }));
			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Subscribe<MessagingCenterTests, string> (this, null, (sender, args) => { }));
			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Subscribe<MessagingCenterTests, string> (this, "Foo", null));

			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Subscribe<MessagingCenterTests> (null, "Foo", (sender) => { }));
			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Subscribe<MessagingCenterTests> (this, null, (sender) => { }));
			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Subscribe<MessagingCenterTests> (this, "Foo", null));

			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Send<MessagingCenterTests, string> (null, "Foo", "Bar"));
			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Send<MessagingCenterTests, string> (this, null, "Bar"));

			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Send<MessagingCenterTests> (null, "Foo"));
			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Send<MessagingCenterTests> (this, null));

			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Unsubscribe<MessagingCenterTests> (null, "Foo"));
			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Unsubscribe<MessagingCenterTests> (this, null));

			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Unsubscribe<MessagingCenterTests, string> (null, "Foo"));
			Assert.Throws<ArgumentNullException> (() => MessagingCenter.Instance.Unsubscribe<MessagingCenterTests, string> (this, null));
		}

		[Test]
		public void UnsubscribeInCallback ()
		{
			int messageCount = 0;

			var subscriber1 = new object ();
			var subscriber2 = new object ();

			MessagingCenter.Instance.Subscribe<MessagingCenterTests> (subscriber1, "SimpleTest", (sender) => {
				messageCount++;
				MessagingCenter.Instance.Unsubscribe<MessagingCenterTests> (subscriber2, "SimpleTest");
			});

			MessagingCenter.Instance.Subscribe<MessagingCenterTests> (subscriber2, "SimpleTest", (sender) => {
				messageCount++;
				MessagingCenter.Instance.Unsubscribe<MessagingCenterTests> (subscriber1, "SimpleTest");
			});

			MessagingCenter.Instance.Send (this, "SimpleTest");

			Assert.AreEqual (1, messageCount);
		}
	}
}
