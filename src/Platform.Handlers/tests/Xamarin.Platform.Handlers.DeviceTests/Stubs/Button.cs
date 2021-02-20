using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public partial class ButtonStub : StubBase, IButton
	{
		public string Text { get; set; }

		public Color TextColor { get; set; }

		public FontAttributes FontAttributes => throw new System.NotImplementedException();

		public string FontFamily => throw new System.NotImplementedException();

		public double FontSize => throw new System.NotImplementedException();

		public void Clicked() { }
		public void Pressed() { }
		public void Released() { }
	}
}