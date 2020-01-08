using System.Collections;
using System.Linq;
using NUnit.Framework;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.ControlGallery.iOS.Tests
{
	[TestFixture]
	public class BackgroundColorTests : PlatformTestFixture 
	{
		static IEnumerable TestCases
		{
			get
			{
				foreach (var element in BasicElements.Where(e => !(e is Label)))
				{
					element.BackgroundColor = Color.AliceBlue;
					yield return new TestCaseData(element)
						.SetCategory(element.GetType().Name);
				}
			}
		}

		[Test, Category("BackgroundColor"), TestCaseSource(nameof(TestCases))]
		[Description("VisualElement background color should match renderer background color")]
		public void BackgroundColorConsistent(VisualElement element) 
		{
			using (var uiView = GetNativeControl(element))
			{
				var expectedColor = element.BackgroundColor.ToUIColor();
				Assert.That(uiView.BackgroundColor, Is.EqualTo(expectedColor));
			}
		}

		[Test, Category("BackgroundColor"), Category("Label")]
		[Description("Label background color should match renderer background color")]
		public void LabelBackgroundColorConsistent() 
		{
			var label = new Label { Text = "foo", BackgroundColor = Color.AliceBlue };
			using (var renderer = GetRenderer(label))
			{
				var expectedColor = label.BackgroundColor.ToUIColor();
				Assert.That(renderer.NativeView.BackgroundColor, Is.EqualTo(expectedColor));
			}
		}
	}
}