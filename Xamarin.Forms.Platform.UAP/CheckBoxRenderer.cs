using System.ComponentModel;
using Windows.UI.Xaml;

using WindowsCheckbox = Windows.UI.Xaml.Controls.CheckBox;

namespace Xamarin.Forms.Platform.UWP
{
	public class CheckBoxRenderer : ViewRenderer<CheckBox, WindowsCheckbox>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var control = new WindowsCheckbox();
					control.Checked += OnNativeChecked;
					control.Unchecked += OnNativeChecked;
					//control.ClearValue(WindowsCheckbox.IsCheckedProperty);

					SetNativeControl(control);
				}

				Control.IsChecked = Element.IsChecked;
				
				UpdateFlowDirection();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == CheckBox.IsCheckedProperty.PropertyName)
			{
				Control.IsChecked = Element.IsChecked;
			}
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
			{
				UpdateFlowDirection();
			}
		}

		protected override bool PreventGestureBubbling { get; set; } = true;

		void OnNativeChecked(object sender, RoutedEventArgs routedEventArgs)
		{
			((IElementController)Element).SetValueFromRenderer(CheckBox.IsCheckedProperty, Control.IsChecked);
		}

		void UpdateFlowDirection()
		{
			Control.UpdateFlowDirection(Element);
		}
	}
}