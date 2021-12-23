﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WBrush = Windows.UI.Xaml.Media.Brush;
using WRectangle = Windows.UI.Xaml.Shapes.Rectangle;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellSplitView : SplitView
	{
		Brush _flyoutBackdrop;
		WBrush _flyoutPlatformBrush;
		WBrush _defaultBrush;
		LightDismissOverlayMode? _defaultLightDismissOverlayMode;
		double _height = -1d;
		double _width = -1d;
		double _defaultOpenPaneLength = -1d;

		public ShellSplitView()
		{
		}

		internal void SetFlyoutSizes(double height, double width)
		{
			_height = height;
			_width = width;
		}

		internal void RefreshFlyoutPosition()
		{
			var paneRoot = (Windows.UI.Xaml.FrameworkElement)GetTemplateChild("PaneRoot");
			if (paneRoot == null)
				return;

			var HCPaneBorder = (Windows.UI.Xaml.Shapes.Rectangle)GetTemplateChild("HCPaneBorder");

			if (paneRoot != null)
			{
				if (_height == -1)
				{
					paneRoot.Height = double.NaN;
					paneRoot.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;

					if (HCPaneBorder != null)
						HCPaneBorder.Visibility = Windows.UI.Xaml.Visibility.Visible;
				}
				else
				{
					paneRoot.Height = _height;
					paneRoot.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;

					if (HCPaneBorder != null)
						HCPaneBorder.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				}
			}

			if (_width != -1)
			{
				if(_defaultOpenPaneLength == -1)
					_defaultOpenPaneLength = OpenPaneLength;

				OpenPaneLength = _width;
			}
			else if(_defaultOpenPaneLength != -1)
			{
				OpenPaneLength = _defaultOpenPaneLength;
			}
		}

		internal void RefreshFlyoutBackdrop()
		{
			var dismissLayer = ((WRectangle)GetTemplateChild("LightDismissLayer"));
			
			if (dismissLayer == null)
				return;

			if (_defaultBrush == null)
				_defaultBrush = dismissLayer.Fill;

			if (Brush.IsNullOrEmpty(_flyoutBackdrop))
			{
				dismissLayer.Fill = _defaultBrush;
			}
			else
			{
				dismissLayer.Fill = _flyoutPlatformBrush ?? _defaultBrush;
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

				if (_flyoutBackdrop != null)
					_flyoutPlatformBrush = _flyoutBackdrop.ToBrush();
				else
					_flyoutPlatformBrush = _defaultBrush;
			}
		}
	}
}
