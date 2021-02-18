using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public partial class LabelStub : StubBase, ILabel
	{
		public string Text { get; set; }

		public Color TextColor { get; set; }		
	}
}