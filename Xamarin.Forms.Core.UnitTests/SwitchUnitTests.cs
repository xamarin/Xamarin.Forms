using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class SwitchUnitTests : BaseTestFixture
	{
		
		public void TestConstructor ()
		{
			Switch sw = new Switch ();

			Assert.IsFalse (sw.IsToggled);
		}

		
		public void TestOnEvent ()
		{
			Switch sw = new Switch ();

			bool fired = false;
			sw.Toggled += (sender, e) => fired = true;

			sw.IsToggled = true;

			Assert.IsTrue (fired);
		}

		
		public void TestOnEventNotDoubleFired ()
		{
			var sw = new Switch ();

			bool fired = false;
			sw.IsToggled = true;

			sw.Toggled += (sender, args) => fired = true;
			sw.IsToggled = true;

			Assert.IsFalse (fired);
		}
	}
	
}
