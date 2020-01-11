using System.Collections;
using System.Linq;
using NUnit.Framework;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;
using WColor = Windows.UI.Color;

namespace Xamarin.Forms.ControlGallery.WindowsUniversal.Tests
{
	[TestFixture]
	public class BackgroundColorTests : PlatformTestFixture
	{
		static IEnumerable TestCases
		{
			get
			{
				// SearchBar is currently busted; when 8773 gets merged we can turn this back on
				// new SearchBar { Text = "foo", BackgroundColor = Color.AliceBlue },
				foreach (var element in BasicViews.Where(v => !(v is SearchBar)))
				{
					element.BackgroundColor = Color.AliceBlue;
					yield return CreateTestCase(element);
				}
			}
		}

		WColor GetBackgroundColor(Control control) 
		{
			if (control is FormsButton button)
			{
				return (button.BackgroundColor as SolidColorBrush).Color;
			}

			if (control is StepperControl stepper)
			{
				return stepper.ButtonBackgroundColor.ToUwpColor();
			}

			return (control.Background as SolidColorBrush).Color;
		}

		WColor GetBackgroundColor(Panel panel) 
		{
			return (panel.Background as SolidColorBrush).Color;
		}

		[Test, TestCaseSource(nameof(TestCases))]
		[Description("View background color should match renderer background color")]
		public void BackgroundColorConsistent(View view)
		{
			var control = GetNativeControl(view);

			var nativeColor = control != null
				? GetBackgroundColor(control)
				: GetBackgroundColor(GetContainer(view));

			var formsColor = view.BackgroundColor.ToUwpColor();
			Assert.That(nativeColor, Is.EqualTo(formsColor));
		}
	}
}
