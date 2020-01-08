using System;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Views;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.ControlGallery.Android.Tests
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

		[Test, TestCaseSource(nameof(VisualElements))]
		[Description("VisualElement background color should match renderer background color")]
		public void BackgroundColorConsistent(VisualElement element)
		{
			using (var renderer = GetRenderer(element))
			{
				var expectedColor = element.BackgroundColor.ToAndroid();
				var view = renderer.View;
				var drawable = view.Background as ColorDrawable;
				var nativeColor = drawable.Color;
				Assert.That(nativeColor, Is.EqualTo(expectedColor));
			}
		}
	}
}