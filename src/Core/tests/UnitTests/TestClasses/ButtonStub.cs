using Microsoft.Maui.Controls;

namespace Microsoft.Maui.Tests
{
	class ButtonStub : View, IButton
	{
		public string Text { get; set; }

		public Color TextColor { get; set; }

		public FontAttributes FontAttributes { get; set; }

		public string FontFamily { get; set; }

		public double FontSize { get; set; }

		public int CornerRadius { get; set; }

		public void Clicked() { }

		public void Pressed() { }

		public void Released() { }
	}
}
