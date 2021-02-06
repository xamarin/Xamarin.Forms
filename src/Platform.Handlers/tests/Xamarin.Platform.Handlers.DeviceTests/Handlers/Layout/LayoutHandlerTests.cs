﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests.Handlers.Layout
{
	public partial class LayoutHandlerTests : HandlerTestBase<LayoutHandler>
	{
		[Fact(DisplayName = "Empty layout")]
		public async Task EmptyLayout()
		{
			var layout = new LayoutStub();
			await ValidatePropertyInitValue(layout, () => layout.Children.Count, GetNativeChildCount, 0);
		}

		[Fact(DisplayName = "Handler view count matches layout view count")]
		public async Task HandlerViewCountMatchesLayoutViewCount()
		{
			var layout = new LayoutStub();

			layout.Add(new SliderStub());
			layout.Add(new SliderStub());

			await ValidatePropertyInitValue(layout, () => layout.Children.Count, GetNativeChildCount, 2);
		}

		[Fact(DisplayName = "Handler removes child from native layout")]
		public async Task HandlerRemovesChildFromNativeLayout()
		{
			var layout = new LayoutStub();
			var slider = new SliderStub();
			layout.Add(slider);

			var handler = await CreateHandlerAsync(layout);

			handler.Remove(slider);
			Assert.Equal(0, GetNativeChildCount(handler));
		}
	}
}
