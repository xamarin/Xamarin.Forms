using System.Threading.Tasks;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UnitTests.Android;

[assembly: Dependency(typeof(TestingPlatformService))]
namespace Xamarin.Forms.Platform.UnitTests.Android
{
	class TestingPlatformService : ITestingPlatformService
	{
		public async Task CreateRenderer(VisualElement visualElement)
		{
			await Device.InvokeOnMainThreadAsync(() => 
				Platform.Android.Platform.CreateRendererWithContext(visualElement,
					DependencyService.Resolve<Context>()));
		}
	}
}