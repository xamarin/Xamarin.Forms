﻿using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using WColor = Windows.UI.Color;
using WGrid = Windows.UI.Xaml.Controls.Grid;
using WEllipse = Windows.UI.Xaml.Shapes.Ellipse;
using WRectangle = Windows.UI.Xaml.Shapes.Rectangle;
using WVisualStateManager = Windows.UI.Xaml.VisualStateManager;

namespace Xamarin.Forms.Platform.UWP
{
	public class SwitchRenderer : ViewRenderer<Switch, ToggleSwitch>
	{
		const string ToggleSwitchCommonStates = "CommonStates";
		const string ToggleSwitchPointerOver = "PointerOver";
		const string ToggleSwitchKnobBounds = "SwitchKnobBounds";
		const string ToggleSwitchKnobOn = "SwitchKnobOn";
		const string ToggleSwitchFillMode = "Fill";

		object _originalOnHoverColor;
		Brush _originalOnColorBrush;
		Brush _originalThumbOnBrush;

		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var control = new ToggleSwitch();
					control.Toggled += OnNativeToggled;
					control.Loaded += OnControlLoaded;
					control.ClearValue(ToggleSwitch.OnContentProperty);
					control.ClearValue(ToggleSwitch.OffContentProperty);

					SetNativeControl(control);
				}

				Control.IsOn = Element.IsToggled;

				UpdateFlowDirection();
			}
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
			else if (e.PropertyName == Switch.ThumbColorProperty.PropertyName)
				UpdateThumbColor();
		}

		protected override bool PreventGestureBubbling { get; set; } = true;

		void OnControlLoaded(object sender, RoutedEventArgs e)
		{
			UpdateOnColor();
			UpdateThumbColor();
			Control.Loaded -= OnControlLoaded;
		}

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

			var grid = Control.GetFirstDescendant<WGrid>();
			if (grid == null)
				return;

			var groups = WVisualStateManager.GetVisualStateGroups(grid);
			foreach (var group in groups)
			{
				if (group.Name != ToggleSwitchCommonStates)
					continue;

				foreach (var state in group.States)
				{
					if (state.Name != ToggleSwitchPointerOver)
						continue;

					foreach (var timeline in state.Storyboard.Children.OfType<ObjectAnimationUsingKeyFrames>())
					{
						var property = Storyboard.GetTargetProperty(timeline);
						var target = Storyboard.GetTargetName(timeline);

						if (target == ToggleSwitchKnobBounds && property == ToggleSwitchFillMode)
						{
							var frame = timeline.KeyFrames.FirstOrDefault();

							if (frame != null)
							{
								if (_originalOnHoverColor == null)
								{
									if (frame.Value is WColor color)
										_originalOnHoverColor = color;

									if (frame.Value is SolidColorBrush solidColorBrush)
										_originalOnHoverColor = solidColorBrush;
								}

								if (!Element.OnColor.IsDefault)
								{
									frame.Value = new SolidColorBrush(Element.OnColor.ToWindowsColor())
									{
										Opacity = _originalOnHoverColor is SolidColorBrush originalOnHoverBrush ? originalOnHoverBrush.Opacity : 1
									};
								}
								else
									frame.Value = _originalOnHoverColor;
							}
							break;
						}
					}
				}
			}

			var rect = Control.GetDescendantsByName<WRectangle>(ToggleSwitchKnobBounds).FirstOrDefault();

			if (rect != null)
			{
				if (_originalOnColorBrush == null)
					_originalOnColorBrush = rect.Fill;

				if (!Element.OnColor.IsDefault)
					rect.Fill = new SolidColorBrush(Element.OnColor.ToWindowsColor());
				else
					rect.Fill = _originalOnColorBrush;
			}
		}

		void UpdateThumbColor()
		{
			if (Control == null)
				return;

			var grid = Control.GetFirstDescendant<WGrid>();

			if (grid == null)
				return;

			var groups = WVisualStateManager.GetVisualStateGroups(grid);

			foreach (var group in groups)
			{
				if (group.Name != ToggleSwitchCommonStates)
					continue;

				foreach (var state in group.States)
				{
					if (state.Name != ToggleSwitchPointerOver)
						continue;

					foreach (var timeline in state.Storyboard.Children.OfType<ObjectAnimationUsingKeyFrames>())
					{
						var property = Storyboard.GetTargetProperty(timeline);
						var target = Storyboard.GetTargetName(timeline);

						if ((target == ToggleSwitchKnobOn) && (property == ToggleSwitchFillMode))
						{
							var frame = timeline.KeyFrames.FirstOrDefault();

							if (frame != null)
							{
								if (_originalThumbOnBrush == null)
								{
									if (frame.Value is Windows.UI.Color color)
										_originalOnColorBrush = new SolidColorBrush(color);

									if (frame.Value is Brush brush)
										_originalThumbOnBrush = brush;
								}

								if (!Element.ThumbColor.IsDefault)
								{
									var brush = Element.ThumbColor.ToBrush();
									brush.Opacity = _originalThumbOnBrush.Opacity;
									frame.Value = brush;
								}
								else
									frame.Value = _originalThumbOnBrush;
							}
							break;
						}
					}
				}
			}

			if (grid.FindName(ToggleSwitchKnobOn) is WEllipse thumb)
			{
				if (_originalThumbOnBrush == null)
					_originalThumbOnBrush = thumb.Fill;

				if (!Element.ThumbColor.IsDefault)
					thumb.Fill = Element.ThumbColor.ToBrush();
				else
					thumb.Fill = _originalThumbOnBrush;
			}
		}
	}
}
