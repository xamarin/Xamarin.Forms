namespace Microsoft.Maui.DeviceTests.Stubs
{
	public partial class EntryStub : StubBase, IEntry
	{
		private string _text;

		public string Text { get => _text; set => this.SetProperty(ref _text, value); }

		public Color TextColor { get; set; }

		public bool IsPassword { get; set; }

		public FontAttributes FontAttributes { get; set; }

		public string FontFamily { get; set; }

		public double FontSize { get; set; }
	}
}