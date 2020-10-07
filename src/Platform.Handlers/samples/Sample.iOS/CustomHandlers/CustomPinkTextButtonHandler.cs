using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Platform;
using Xamarin.Platform.Handlers;

namespace Sample.iOS.CustomHandlers
{
	class CustomPinkTextButtonHandler : ButtonHandler
	{
		public CustomPinkTextButtonHandler() : base()
		{
			ButtonMapper[nameof(IButton.Text)] = MapTextWithColor;
		}

		public static void MapTextWithColor(ButtonHandler handler, IButton button)
		{
			MapText(handler, button);

			var uiButton = (handler.NativeView as UIButton);
			uiButton.SetTitleColor(Color.Pink.ToNative(), UIControlState.Normal);
			uiButton.SetTitleColor(Color.Pink.ToNative(), UIControlState.Highlighted);
		}
	}
}