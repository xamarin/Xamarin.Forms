using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public class ActivityIndicatorStub : StubBase, IActivityIndicator
	{
		public bool IsRunning { get; set; }

		public Color Color { get; set; }
	}
}