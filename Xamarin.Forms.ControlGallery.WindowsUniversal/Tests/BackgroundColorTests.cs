using System.Collections.Generic;
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
		static IEnumerable<VisualElement> VisualElements
		{
			get
			{
				return new VisualElement[]
				{
					new Button { Text = "foo", BackgroundColor = Color.AliceBlue },
					new CheckBox { BackgroundColor = Color.AliceBlue },
					new DatePicker { BackgroundColor = Color.AliceBlue },
					new Editor { Text = "foo", BackgroundColor = Color.AliceBlue },
					new Entry { Text = "foo", BackgroundColor = Color.AliceBlue },
					new Image { BackgroundColor = Color.AliceBlue },
					new Label { Text = "foo", BackgroundColor = Color.AliceBlue },
					new Picker { BackgroundColor = Color.AliceBlue },
					new ProgressBar { BackgroundColor = Color.AliceBlue },
					
					// SearchBar is currently busted; when 8773 gets merged we can turn this back on
					// new SearchBar { Text = "foo", BackgroundColor = Color.AliceBlue },
					
					new Stepper { BackgroundColor = Color.AliceBlue },
					new Switch { BackgroundColor = Color.AliceBlue },
					new TimePicker { BackgroundColor = Color.AliceBlue },
				};
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

		[Test, TestCaseSource(nameof(VisualElements))]
		[Description("VisualElement background color should match renderer background color")]
		public void BackgroundColorConsistent(VisualElement element)
		{
			var control = GetNativeControl(element);

			var nativeColor = control != null
				? GetBackgroundColor(control)
				: GetBackgroundColor(GetContainer(element));

			var formsColor = element.BackgroundColor.ToUwpColor();
			Assert.That(nativeColor, Is.EqualTo(formsColor));
		}
	}
}
