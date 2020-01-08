using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.ControlGallery.Android.Tests
{
	[TestFixture]
	public class IsEnabledTests : PlatformTestFixture
	{
		static IEnumerable TestCases
		{
			get
			{
				var elements = new VisualElement[]
				{
					new Button{ Text = "foo" }, new Button{ Text = "foo", IsEnabled = false },
					new CheckBox { }, new CheckBox { IsEnabled = false },
					new DatePicker { }, new DatePicker { IsEnabled = false },
					new Editor { }, new Editor { IsEnabled = false },
					new Entry { }, new Entry { IsEnabled = false },
					new Image { }, new Image { IsEnabled = false },
					new Label { Text = "foo" }, new Label { Text = "foo", IsEnabled = false },
					new Picker { }, new Picker {  IsEnabled = false },
					new ProgressBar { }, new ProgressBar { IsEnabled = false },
					new SearchBar { }, new SearchBar { IsEnabled = false },
					new Stepper {  }, new Stepper { IsEnabled = false },
					new Switch { }, new Switch { IsEnabled = false },
					new TimePicker { }, new TimePicker { IsEnabled = false },
				};

				foreach (var element in elements)
				{
					var typeName = element.GetType().Name;
					yield return new TestCaseData(element).SetName($"{typeName}_IsEnabled_{element.IsEnabled}");
				}
			}
		}

		[Test, TestCaseSource(nameof(TestCases))]
		[Description("VisualElement enabled should match renderer enabled")]
		public void EnabledConsistent(VisualElement element)
		{
			using (var renderer = GetRenderer(element))
			{
				var expected = element.IsEnabled;
				var nativeView = renderer.View;

				ParentView(nativeView);

				// Check the container control
				Assert.That(renderer.View.Enabled, Is.EqualTo(expected));

				// Check the actual control
				var control = GetNativeControl(element);
				Assert.That(control.Enabled, Is.EqualTo(expected));

				UnparentView(nativeView);
			}
		}
	}
}