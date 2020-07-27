﻿using Windows.UI.Xaml.Controls;
using WBrush = Windows.UI.Xaml.Media.Brush;
using WRectangle = Windows.UI.Xaml.Shapes.Rectangle;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellSplitView : SplitView
	{
		Brush _flyoutBackdrop;
		WBrush _defaultBrush;
		LightDismissOverlayMode? _defaultLightDismissOverlayMode;
		public ShellSplitView()
		{
		}


		internal void UpdateFlyoutBackdrop()
		{
			var dismissLayer = ((WRectangle)GetTemplateChild("LightDismissLayer"));

			if (dismissLayer == null)
				return;

			if (_defaultBrush == null)
				_defaultBrush = dismissLayer.Fill;

			if (_flyoutBackdrop == Brush.Default)
			{
				dismissLayer.Fill = _defaultBrush;
			}
			else
			{
				dismissLayer.Fill = _flyoutBackdrop.ToBrush();
			}
		}

		internal Brush FlyoutBackdrop
		{
			set
			{
				if (_flyoutBackdrop == value)
					return;

				_flyoutBackdrop = value;

				if (_defaultLightDismissOverlayMode == null)
					_defaultLightDismissOverlayMode = LightDismissOverlayMode;

				if (value == Brush.Default)
				{
					LightDismissOverlayMode = _defaultLightDismissOverlayMode ?? LightDismissOverlayMode.Auto;
				}
				else
				{
					LightDismissOverlayMode = LightDismissOverlayMode.On;
				}
			}
		}
	}
}
