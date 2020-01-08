using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics.Drawables;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.ControlGallery.Android.Tests
{
	[TestFixture]
	public class BackgroundColorTests : PlatformTestFixture 
	{
		static IEnumerable TestCases
		{
			get
			{
				foreach (var element in BasicElements.Where(e => !(e is Button)))
				{
					element.BackgroundColor = Color.AliceBlue;
					yield return new TestCaseData(element);
				}
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

		[Test, TestCaseSource(nameof(TestCases))]
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