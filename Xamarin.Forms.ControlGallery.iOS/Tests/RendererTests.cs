using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.ControlGallery.iOS.Tests
{
	[TestFixture]
	[Internals.Preserve(AllMembers = true)]
	public class RendererTests : PlatformTestFixture
	{
		[Test(Description = "Basic sanity check that Label text matches renderer text")]
		public void LabelTextMatchesRendererText()
		{
			System.Diagnostics.Debug.WriteLine($">>>>>> Starting LabelTextMatchesRendererText");


			try
			{
				var label = new Label { Text = "foo" };
				//	using (var textView = await GetNativeControl(label))
				//{

				var textView = GetNativeControl(label);

				System.Diagnostics.Debug.WriteLine($">>>>>> Got native control");

				Assert.That(label.Text == textView.Text);

				//	}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($">>>>>> {ex}");
				Assert.Fail();
			}
		}
	}
}