using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public partial class LabelStub : StubBase, ILabel
	{
		public TextType TextType { get; set; }

		public double LineHeight { get; set; }

		public int MaxLines { get; set; }

		public TextDecorations TextDecorations { get; set; }

		public LineBreakMode LineBreakMode { get; set; }

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

		public Thickness Padding { get; set; }
	}
}