﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Platform;
using Xamarin.Platform.Handlers;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public partial class SliderStub : StubBase, ISlider
	{
		public double Minimum { get; set; }
		public double Maximum { get; set; }
		public double Value { get; set; }

		public Color MinimumTrackColor { get; set; }
		public Color MaximumTrackColor { get; set; }
		public Color ThumbColor { get; set; }

		public void DragStarted() { }
		public void DragCompleted() { }
	}
}
