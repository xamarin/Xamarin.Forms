using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.ControlGallery.Android.Tests
{
	[TestFixture]
	public class OpacityTests : PlatformTestFixture 
	{
		static readonly double TestOpacity = 0.35;

		static IEnumerable<VisualElement> VisualElements
		{
			get
			{
				return new VisualElement[]
				{
					new CheckBox { Opacity = TestOpacity },
					new DatePicker { Opacity = TestOpacity  },
					new Editor { Text = "foo", Opacity = TestOpacity  },
					new Entry { Text = "foo", Opacity = TestOpacity  },
					new Image { Opacity = TestOpacity  },
					new Label { Opacity = TestOpacity  },
					new Picker { Opacity = TestOpacity  },
					new ProgressBar { Opacity = TestOpacity  },
					new SearchBar { Text = "foo", Opacity = TestOpacity  },
					new Stepper { Opacity = TestOpacity  },
					new Switch { Opacity = TestOpacity  },
					new TimePicker { Opacity = TestOpacity  },
				};
			}
		}

		[Test, TestCaseSource(nameof(VisualElements))]
		[Description("VisualElement opacity should match renderer opacity")]
		public void OpacityConsistent(VisualElement element)
		{
			using (var renderer = GetRenderer(element))
			{
				var expected = element.Opacity;
				var nativeView = renderer.View;

				ParentView(nativeView);

				Assert.That((double)renderer.View.Alpha, Is.EqualTo(expected).Within(0.001d));

				UnparentView(nativeView);
			}
		}
	}
}