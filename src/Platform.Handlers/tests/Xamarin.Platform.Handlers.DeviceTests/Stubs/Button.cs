using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public partial class ButtonStub : StubBase, IButton
	{
		public LineBreakMode LineBreakMode { get; set; }

		public ButtonContentLayout ContentLayout { get; set; }

		public string Text { get; set; }

		public Color TextColor { get; set; }

		public Font Font { get; set; }

		public TextTransform TextTransform { get; set; }

		public double CharacterSpacing { get; set; }

		public FontAttributes FontAttributes { get; set; }

		public string FontFamily { get; set; }

		public double FontSize { get; set; }

		public TextAlignment HorizontalTextAlignment { get; set; }

		public TextAlignment VerticalTextAlignment { get; set; }

		public int CornerRadius { get; set; }

		public Color BorderColor { get; set; }

		public double BorderWidth { get; set; }

		public Thickness Padding { get; set; }

		public void Clicked() { }
		public void Pressed() { }
		public void Released() { }
	}
}