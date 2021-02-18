using System;
using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public partial class SwitchStub : StubBase, ISwitch
	{
		public bool IsToggled { get; set; }

		public Color TrackColor { get; set; }

		public Color ThumbColor { get; set; }

		public void Toggled() => ToggledDelegate?.Invoke();

		public Action ToggledDelegate; 
	}
}