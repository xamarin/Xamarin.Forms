using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Xamarin.Forms.Platform.WPF
{
	public class SwitchRenderer : ViewRenderer<Switch, Border>
	{
		readonly CheckBox _toggleSwitch = new CheckBox();

		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			base.OnElementChanged(e);

			var container = new Border { Child = _toggleSwitch };
			_toggleSwitch.IsChecked = Element.IsToggled;
			_toggleSwitch.Checked += (sender, args) => ((IElementController)Element).SetValueFromRenderer(Switch.IsToggledProperty, true);
			_toggleSwitch.Unchecked += (sender, args) => ((IElementController)Element).SetValueFromRenderer(Switch.IsToggledProperty, false);
			_toggleSwitch.HorizontalAlignment = HorizontalAlignment.Right;

			SetNativeControl(container);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Switch.IsToggledProperty.PropertyName)
			{
				if (_toggleSwitch.IsChecked != Element.IsToggled)
					_toggleSwitch.IsChecked = Element.IsToggled;
			}
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateSwitchIsEnabled();
		}

		protected override void UpdateNativeWidget()
		{
			base.UpdateNativeWidget();

			UpdateSwitchIsEnabled();
		}

		void UpdateSwitchIsEnabled()
		{
			_toggleSwitch.IsEnabled = Element.IsEnabled;
		}
	}
}