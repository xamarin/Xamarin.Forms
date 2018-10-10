using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class MediaElementTests : BaseTestFixture
	{
		[Test]
		public void TestSourceRoundTrip()
		{
			var uri = new Uri("https://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4");
			var media = new MediaElement();
			Assert.Null(media.Source);
			media.Source = uri;
			Assert.NotNull(media.Source);
			Assert.AreEqual(uri, media.Source);
		}
	}
}
