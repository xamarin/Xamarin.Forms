using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Xamarin.Forms.Platform.UWP
{
	public class SwitchRenderer : ViewRenderer<Switch, ToggleSwitch>
	{
		private object _originalOnColor;
		private Brush _originalOnColorBrush;

		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			base.OnElementChanged(e);
			
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var control = new ToggleSwitch();
					control.Toggled += OnNativeToggled;
					control.Loaded += Control_Loaded;
					control.ClearValue(ToggleSwitch.OnContentProperty);
					control.ClearValue(ToggleSwitch.OffContentProperty);

					SetNativeControl(control);
				}

				Control.IsOn = Element.IsToggled;

				UpdateFlowDirection();
			}
		}

		private void Control_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateOnColor();
			Control.Loaded -= Control_Loaded;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Switch.IsToggledProperty.PropertyName)
			{
				Control.IsOn = Element.IsToggled;
			}
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
			{
				UpdateFlowDirection();
			}
			else if (e.PropertyName == Switch.OnColorProperty.PropertyName)
				UpdateOnColor();
		}

		protected override bool PreventGestureBubbling { get; set; } = true;

		void OnNativeToggled(object sender, RoutedEventArgs routedEventArgs)
		{
			((IElementController)Element).SetValueFromRenderer(Switch.IsToggledProperty, Control.IsOn);
		}

		void UpdateFlowDirection()
		{
			Control.UpdateFlowDirection(Element);
		}

		void UpdateOnColor()
		{
			if (Control == null)
				return;

			var grid = Control.GetChildren<Windows.UI.Xaml.Controls.Grid>().FirstOrDefault();
			var groups = Windows.UI.Xaml.VisualStateManager.GetVisualStateGroups(grid);
			foreach (var group in groups)
			{
				if (group.Name != "CommonStates")
					continue;

				foreach (var state in group.States)
				{
					if (state.Name != "PointerOver")
						continue;

					foreach (var timeline in state.Storyboard.Children.OfType<ObjectAnimationUsingKeyFrames>())
					{
						var property = Storyboard.GetTargetProperty(timeline);
						var target = Storyboard.GetTargetName(timeline);
						if ((target == "SwitchKnobBounds") && (property == "Fill"))
						{
							var frame = timeline.KeyFrames.First();

							if (_originalOnColor == null)
								_originalOnColor = frame.Value;

							if (Element.IsSet(Switch.OnColorProperty) && !Element.OnColor.IsDefault)
								frame.Value = new SolidColorBrush(Element.OnColor.ToWindowsColor()) { Opacity = .7 };
							else
								frame.Value = _originalOnColor;
							break;
						}
					}
				}
			}

			var rect = Control.GetDescendantsByName<Windows.UI.Xaml.Shapes.Rectangle>("SwitchKnobBounds").FirstOrDefault();

			if (_originalOnColorBrush == null)
				_originalOnColorBrush = rect.Fill;

			if (Element.IsSet(Switch.OnColorProperty) && !Element.OnColor.IsDefault)
				rect.Fill = new SolidColorBrush(Element.OnColor.ToWindowsColor());
			else
				rect.Fill = _originalOnColorBrush;
		}
	}
}