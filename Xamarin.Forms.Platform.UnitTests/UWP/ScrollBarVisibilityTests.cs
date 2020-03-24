using NUnit.Framework;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.Platform.UnitTests.UWP
{
	[TestFixture]
	public class ScrollBarVisibilityTests
	{
		[Test, Category("Scrollbar")]
		public void ConvertScrollbarVisibility()
		{
			var always = ScrollBarVisibility.Always.ToUwpScrollBarVisibility();
			var defaultVisibility = ScrollBarVisibility.Default.ToUwpScrollBarVisibility();
			var never = ScrollBarVisibility.Never.ToUwpScrollBarVisibility();

			Assert.That(always, Is.EqualTo(Windows.UI.Xaml.Controls.ScrollBarVisibility.Visible));
			Assert.That(defaultVisibility, Is.EqualTo(Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto));
			Assert.That(never, Is.EqualTo(Windows.UI.Xaml.Controls.ScrollBarVisibility.Hidden));
		}
	}
}
