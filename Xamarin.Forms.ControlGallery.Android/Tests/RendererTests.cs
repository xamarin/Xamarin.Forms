using Android.Views;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.ControlGallery.Android.Tests
{
	[TestFixture]
	public class RendererTests : PlatformTestFixture
	{
		[Test(Description = "Basic sanity check that Label text matches renderer text")]
		public void LabelTextMatchesRendererText()
		{
			var label = new Label { Text = "foo" };
			using (var textView = GetNativeControl(label))
			{
				Assert.That(label.Text == textView.Text);
			}
		}

		[Test(Description = "Validate renderer vertical alignment for Entry with VerticalTextAlignment Center")]
		public void EntryVerticalAlignmentCenterInRenderer()
		{ 
			var entry1 = new Entry { Text = "foo", VerticalTextAlignment = TextAlignment.Center };
			using (var editText = GetNativeControl(entry1))
			{
				var centeredVertical =
				(editText.Gravity & GravityFlags.VerticalGravityMask) == GravityFlags.CenterVertical;

				Assert.That(centeredVertical, Is.True);
			}
		}
	}

	[TestFixture]
	public class BackgroundColorTests : PlatformTestFixture 
	{
		[Test]
		[Description("Button background color should match renderer background color")]
		public void ButtonBackgroundColorConsistent()
		{
			var button = new Button 
			{ 
				Text = "      ",
				HeightRequest = 100, WidthRequest = 100,
				BackgroundColor = Color.AliceBlue 
			};

			using (var nativeButton = GetNativeControl(button))
			{
				var expectedColor = button.BackgroundColor.ToAndroid();
				Layout(button, nativeButton);
				var nativeColor = GetColorAtCenter(nativeButton);
				Assert.That(nativeColor, Is.EqualTo(expectedColor));
			}
		}
	}
}