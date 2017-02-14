using System;
using System.ComponentModel;
using ElmSharp;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Specific = Xamarin.Forms.PlatformConfiguration.TizenSpecific.VisualElement;

namespace Xamarin.Forms.Platform.Tizen
{
	public class SwitchRenderer : ViewRenderer<Switch, Check>
	{
		public SwitchRenderer()
		{
			RegisterPropertyHandler(Switch.IsToggledProperty, HandleToggled);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			if (Control == null)
			{
				var _switch = new Check(Forms.Context.MainWindow)
				{
					PropagateEvents = false,
				};
				SetNativeControl(_switch);
			}

			if (e.OldElement != null)
			{
				Control.StateChanged -= CheckChangedHandler;
			}

			if (e.NewElement != null)
			{
				Control.StateChanged += CheckChangedHandler;
			}

			base.OnElementChanged(e);
		}

		protected override void UpdateThemeStyle()
		{
			var style = Specific.GetStyle(Element);
			switch (style)
			{
				case SwitchStyle.Toggle:
				case SwitchStyle.Favorite:
				case SwitchStyle.CheckBox:
					Control.Style = style;
					break;
				default:
					Control.Style = SwitchStyle.Toggle;
					break;
			}
			((IVisualElementController)Element).NativeSizeChanged();
		}

		void CheckChangedHandler(object sender, EventArgs e)
		{
			Element.SetValue(Switch.IsToggledProperty, Control.IsChecked);
		}

		void HandleToggled()
		{
			Control.IsChecked = Element.IsToggled;
		}
	}
}
