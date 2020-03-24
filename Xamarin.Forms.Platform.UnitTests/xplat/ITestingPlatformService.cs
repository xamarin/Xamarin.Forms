using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.UnitTests
{
	public interface ITestingPlatformService
	{
		Task CreateRenderer(VisualElement visualElement);
	}
}
