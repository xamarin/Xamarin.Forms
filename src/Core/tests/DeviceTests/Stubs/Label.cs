namespace Microsoft.Maui.DeviceTests.Stubs
{
	public partial class LabelStub : StubBase, ILabel
	{
		public string Text { get; set; }

		public Color TextColor { get; set; }

		public FontAttributes FontAttributes { get; set; }

		public string FontFamily { get; set; }

		public double FontSize { get; set; }

		public int MaxLines { get; set; } = -1;

		public LineBreakMode LineBreakMode { get; set; } = LineBreakMode.WordWrap;
	}
}