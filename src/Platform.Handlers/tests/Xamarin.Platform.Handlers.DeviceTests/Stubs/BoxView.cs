using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public class BoxViewStub : StubBase, IBox
	{
		public Color Color { get; set; }

		public CornerRadius CornerRadius { get; set; }
	}
}