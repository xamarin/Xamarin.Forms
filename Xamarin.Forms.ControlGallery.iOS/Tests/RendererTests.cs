using NUnit.Framework;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.ControlGallery.iOS.Tests
{
	[TestFixture]
	[Internals.Preserve(AllMembers = true)]
	public class RendererTests : PlatformTestFixture
	{
		[Test(Description = "Basic sanity check that Label text matches renderer text")]
		public void LabelTextMatchesRendererText()
		{
			var label = new Label { Text = "foo" };
			using (var uiLabel = GetNativeControl(label))
			{
				Assert.That(label.Text, Is.EqualTo(uiLabel.Text));
			}
		}
	}

	[TestFixture]
	public class BackgroundColorTests : PlatformTestFixture 
	{
		[Test(Description = "Label background color should match renderer background")]
		public void LabelBackgroundMatchesRendererBackground() 
		{
			var label = new Label { Text = "foo", BackgroundColor = Color.AliceBlue };
			using (var renderer = GetRenderer(label))
			{
				var expectedColor = label.BackgroundColor.ToUIColor();
				Assert.That(renderer.NativeView.BackgroundColor, Is.EqualTo(expectedColor));
			}
		}

		[Test(Description = "Entry background color should match renderer background")]
		public void EntryBackgroundMatchesRendererBackground()
		{
			var entry = new Entry { Text = "foo", BackgroundColor = Color.AliceBlue };
			using (var uiTextField = GetNativeControl(entry))
			{
				var expectedColor = entry.BackgroundColor.ToUIColor();
				Assert.That(uiTextField.BackgroundColor, Is.EqualTo(expectedColor));
			}
		}

		[Test(Description = "Editor background color should match renderer background")]
		public void EditorBackgroundMatchesRendererBackground()
		{
			var editor = new Editor { Text = "foo", BackgroundColor = Color.AliceBlue };
			using (var uiTextField = GetNativeControl(editor))
			{
				var expectedColor = editor.BackgroundColor.ToUIColor();
				Assert.That(uiTextField.BackgroundColor, Is.EqualTo(expectedColor));
			}
		}

		//[Test(Description = "BoxView background color should match renderer background")]
		//public void BoxViewBackgroundMatchesRendererBackground()
		//{
		//	var boxView = new BoxView { BackgroundColor = Color.AliceBlue };
		//	using (var uiview = GetNativeControl(boxView))
		//	{
		//		var expectedColor = boxView.BackgroundColor.ToUIColor();

		//		// TODO BackgroundColor isn't a thing for BoxView, need to get a pixel at a location

		//		Assert.That(uiview.BackgroundColor, Is.EqualTo(expectedColor));
		//	}
		//}
	}
}