using System;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class SwitchRenderer : ViewRenderer<Switch, NSSwitch>
	{
		bool _disposed;

		IElementController ElementController => Element;

		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			if (e.OldElement != null)
				e.OldElement.Toggled -= OnElementToggled;

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new NSSwitch());
					Control.Activated += OnControlActivated;
				}
				UpdateState();
				e.NewElement.Toggled += OnElementToggled;
			}

			base.OnElementChanged(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;
				if (Control != null)
					Control.Activated -= OnControlActivated;
			}

			base.Dispose(disposing);
		}

		void OnControlActivated(object sender, EventArgs e)
		{
			ElementController?.SetValueFromRenderer(Switch.IsToggledProperty, Control.State == (int)NSCellStateValue.On);
		}

		void OnElementToggled(object sender, EventArgs e)
		{
			UpdateState();
		}

		void UpdateState()
		{
			Control.State = Element.IsToggled ? (int)NSCellStateValue.On : (int)NSCellStateValue.Off;
		}
	}
}