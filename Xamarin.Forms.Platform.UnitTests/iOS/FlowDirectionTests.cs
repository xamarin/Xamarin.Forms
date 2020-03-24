using NUnit.Framework;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.Platform.UnitTests.iOS
{
	[TestFixture]
	public class FlowDirectionTests
	{
		[Test]
		public void FlowDirectionConversion() 
		{
			var ltr = UIUserInterfaceLayoutDirection.LeftToRight.ToFlowDirection();
			Assert.That(ltr, Is.EqualTo(FlowDirection.LeftToRight));

			var rtl = UIUserInterfaceLayoutDirection.RightToLeft.ToFlowDirection();
			Assert.That(rtl, Is.EqualTo(FlowDirection.RightToLeft));
		}
	}
}