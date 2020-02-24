using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android.Tests;
using Xamarin.Forms.Controls.Tests;

[assembly: Dependency(typeof(TestingPlatformService))]
namespace Xamarin.Forms.ControlGallery.Android.Tests
{
	class TestingPlatformService : ITestingPlatformService
	{
		public void CreateRenderer(VisualElement visualElement)
		{
			var renderer = Platform.Android.Platform.CreateRendererWithContext(visualElement,
				DependencyService.Resolve<Context>());

			Platform.Android.Platform.SetRenderer(visualElement, renderer);
		}
	}
}