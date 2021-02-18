﻿using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class SwitchHandlerTests
	{
		UISwitch GetNativeSwitch(SwitchHandler switchHandler) =>
			(UISwitch)switchHandler.View;

		bool GetNativeIsChecked(SwitchHandler switchHandler) =>
		  GetNativeSwitch(switchHandler).On;

		async Task ValidateTrackColor(ISwitch switchStub, Color color, Action action = null)
		{
			var expected = await GetValueAsync(switchStub, handler => GetNativeSwitch(handler).OnTintColor.ToColor());
			Assert.Equal(expected, color);
		}

		async Task ValidateThumbColor(ISwitch switchStub, Color color, Action action = null)
		{
			var expected = await GetValueAsync(switchStub, handler => GetNativeSwitch(handler).ThumbTintColor.ToColor());
			Assert.Equal(expected, color);
		}
	}
}