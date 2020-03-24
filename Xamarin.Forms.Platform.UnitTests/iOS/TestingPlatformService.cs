using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UnitTests.iOS;

[assembly: Dependency(typeof(TestingPlatformService))]
namespace Xamarin.Forms.Platform.UnitTests.iOS
{
	class TestingPlatformService : ITestingPlatformService
	{
		public async Task CreateRenderer(VisualElement visualElement)
		{
			await Device.InvokeOnMainThreadAsync(() => Platform.iOS.Platform.CreateRenderer(visualElement));
		}
	}
}