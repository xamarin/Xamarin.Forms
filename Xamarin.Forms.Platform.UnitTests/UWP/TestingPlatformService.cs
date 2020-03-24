using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UnitTests.UWP;

[assembly: Dependency(typeof(TestingPlatformService))]
namespace Xamarin.Forms.Platform.UnitTests.UWP
{
	class TestingPlatformService : ITestingPlatformService
	{
		public async Task CreateRenderer(VisualElement visualElement)
		{
			await Device.InvokeOnMainThreadAsync(() => Platform.UWP.Platform.CreateRenderer(visualElement));
		}
	}
}
