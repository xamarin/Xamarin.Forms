using System;
using Microsoft.Maui;

namespace Microsoft.Maui.DeviceTests.Stubs
{
	public partial class ButtonStub : StubBase, IButton
	{
		public string Text { get; set; }

		public Color TextColor { get; set; }

		public event EventHandler Pressed;
		public event EventHandler Released;
		public event EventHandler Clicked;

		void IButton.Pressed() => Pressed?.Invoke(this, EventArgs.Empty);
		void IButton.Released() => Released?.Invoke(this, EventArgs.Empty);
		void IButton.Clicked() => Clicked?.Invoke(this, EventArgs.Empty);
	}
}