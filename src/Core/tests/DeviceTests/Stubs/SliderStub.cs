using System;

namespace Microsoft.Maui.DeviceTests.Stubs
{
	public partial class SliderStub : ViewStubBase, ISlider
	{
		double _value;

		public double Minimum { get; set; }
		public double Maximum { get; set; }
		public double Value
		{
			get => Math.Clamp(_value, Minimum, Maximum);
			set => _value = value;
		}

		public Color MinimumTrackColor { get; set; }
		public Color MaximumTrackColor { get; set; }
		public Color ThumbColor { get; set; }

		public void DragStarted() { }
		public void DragCompleted() { }
	}
}
