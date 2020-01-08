using System.Collections;
using NUnit.Framework;
using UIKit;

namespace Xamarin.Forms.ControlGallery.iOS.Tests
{
	public class IsEnabledTests : PlatformTestFixture 
	{
		static IEnumerable TestCases
		{
			get
			{
				// Generate IsEnabled = true cases
				foreach (var element in BasicElements)
				{
					var typeName = element.GetType().Name;
					yield return new TestCaseData(element)
						.SetName($"{typeName}_IsEnabled_True")
						.SetCategory(typeName);
				}

				// Generate IsEnabled = false cases
				foreach (var element in BasicElements)
				{
					var typeName = element.GetType().Name;
					yield return new TestCaseData(element)
						.SetName($"{typeName}_IsEnabled_False")
						.SetCategory(typeName);
				}
			}
		}

		[Test, Category("IsEnabled"), TestCaseSource(nameof(TestCases))]
		[Description("VisualElement enabled should match renderer enabled")]
		public void EnabledConsistent(VisualElement element)
		{
			using (var renderer = GetRenderer(element))
			{
				var expected = element.IsEnabled;
				var nativeView = renderer.NativeView;

				// Check the container
				Assert.That(renderer.NativeView.UserInteractionEnabled, Is.EqualTo(expected));

				// Check the actual control
				var control = GetNativeControl(element);

				if (control is UIControl uiControl)
				{
					Assert.That(uiControl.Enabled, Is.EqualTo(expected));
				}

				if (control is UILabel uiLabel)
				{
					Assert.That(uiLabel.Enabled, Is.EqualTo(expected));
				}
			}
		}
	}
}