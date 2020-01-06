using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.ControlGallery.iOS.Tests
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
					new Picker { BackgroundColor = Color.AliceBlue },
					new ProgressBar { BackgroundColor = Color.AliceBlue },
					new SearchBar { Text = "foo", BackgroundColor = Color.AliceBlue },
					new Stepper { BackgroundColor = Color.AliceBlue },
					new Switch { BackgroundColor = Color.AliceBlue },
					new TimePicker { BackgroundColor = Color.AliceBlue },
				};
			}
		}

		[Test, TestCaseSource(nameof(VisualElements))]
		[Description("VisualElement background color should match renderer background color")]
		public void BackgroundColorConsistent(VisualElement element) 
		{
			using (var uiView = GetNativeControl(element))
			{
				var expectedColor = element.BackgroundColor.ToUIColor();
				Assert.That(uiView.BackgroundColor, Is.EqualTo(expectedColor));
			}
		}

		[Test]
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