using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class OnIdiomTests : BaseTestFixture
	{
		[Test]
		public void OnIdiomReturnsAppropriateValue()
		{
			OnIdiom<int> onIdiom = new OnIdiom<int>();
			onIdiom.Idioms.Add(new OnIdiomSetting() { Idiom = new[] { "Phone", "Desktop" }, Value = 1 });
			onIdiom.Idioms.Add(new OnIdiomSetting() { Idiom = new[] { "Watch", "TV" }, Value = 2 });
			
			Device.Idiom = TargetIdiom.TV;
			Assert.AreEqual(2, onIdiom);
			Device.Idiom = TargetIdiom.Desktop;
			Assert.AreEqual(1, onIdiom);
			Device.Idiom = TargetIdiom.Watch;
			Assert.AreEqual(2, onIdiom);
			Device.Idiom = TargetIdiom.Unsupported;
			Assert.AreEqual(default(int), onIdiom);
		}

		[Test]
		public void OnIdiomFallsBackToLegacyGracefully()
		{
			OnIdiom<int> onIdiom = new OnIdiom<int>();
			onIdiom.Idioms.Add(new OnIdiomSetting() { Idiom = new[] { "Desktop" }, Value = 1 });
			onIdiom.Idioms.Add(new OnIdiomSetting() { Idiom = new[] { "Watch", "TV" }, Value = 2 });
			onIdiom.Phone = 3;

			Device.Idiom = TargetIdiom.Phone;
			Assert.AreEqual(3, onIdiom);
			Device.Idiom = TargetIdiom.TV;
			Assert.AreEqual(2, onIdiom);
		}

		[Test]
		public void DefaultValueIsSupportedOnLegacySyntax()
		{
			OnIdiom<int> onIdiom = new OnIdiom<int>();
			onIdiom.Phone = 1;
			onIdiom.Default = 3;
			Device.Idiom = TargetIdiom.TV;
			Assert.AreEqual(3, onIdiom);
		}

		[Test]
		public void DefaultValueIsSupported()
		{
			OnIdiom<int> onIdiom = new OnIdiom<int>();
			onIdiom.Idioms.Add(new OnIdiomSetting() { Idiom = new[] { "Desktop" }, Value = 1 });
			onIdiom.Idioms.Add(new OnIdiomSetting() { Idiom = new[] { "Watch", "Phone" }, Value = 2 });
			onIdiom.Default = 6;
			Device.Idiom = TargetIdiom.TV;
			Assert.AreEqual(6, onIdiom);
		}
	}
}
