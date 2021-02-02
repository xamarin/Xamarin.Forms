using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public class ProgressBarStub : StubBase, IProgress
	{
		public double Progress { get; set; }

		public Color ProgressColor { get; set; }
	}
}