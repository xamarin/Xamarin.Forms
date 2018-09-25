﻿using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Internals;
using Windows.UI.Xaml.Input;

namespace Xamarin.Forms.Platform.UWP
{
	public class StepperRenderer : ViewRenderer<Stepper, StepperControl>
	{
		FocusNavigationDirection focusDirection;

		protected override void Dispose(bool disposing)
		{
			if (disposing && Control != null)
			{
				Control.GotFocus -= OnGotFocus;
				Control.GettingFocus -= OnGettingFocus;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Stepper> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new StepperControl());
					Control.ValueChanged += OnControlValue;
					Control.GettingFocus += OnGettingFocus;
					Control.GotFocus += OnGotFocus;
				}

				UpdateMaximum();
				UpdateMinimum();
				UpdateValue();
				UpdateIncrement(); 
				UpdateFlowDirection();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Stepper.ValueProperty.PropertyName)
				UpdateValue();
			else if (e.PropertyName == Stepper.MaximumProperty.PropertyName)
				UpdateMaximum();
			else if (e.PropertyName == Stepper.MinimumProperty.PropertyName)
				UpdateMinimum();
			else if (e.PropertyName == Stepper.IncrementProperty.PropertyName)
				UpdateIncrement();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundColor();
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection();
		}

		protected override void UpdateTabStop()
		{
			base.UpdateTabStop();
			Control?.GetChildren<Control>().ForEach(c => c.IsTabStop = Element.IsTabStop);
		}

		void OnGettingFocus(UIElement sender, GettingFocusEventArgs args) => focusDirection = args.Direction;

		void OnGotFocus(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource == Control)
				FocusManager.TryMoveFocus(focusDirection);
		}

		protected override void UpdateBackgroundColor()
		{
			if (Control != null)
				Control.ButtonBackgroundColor = Element.BackgroundColor;
		}

		protected override bool PreventGestureBubbling { get; set; } = true;

		void OnControlValue(object sender, EventArgs e)
		{
			Element.SetValueCore(Stepper.ValueProperty, Control.Value);
		}

		void UpdateFlowDirection()
		{
			Control.UpdateFlowDirection(Element);
		}

		void UpdateIncrement()
		{
			Control.Increment = Element.Increment;
		}

		void UpdateMaximum()
		{
			Control.Maximum = Element.Maximum;
		}

		void UpdateMinimum()
		{
			Control.Minimum = Element.Minimum;
		}

		void UpdateValue()
		{
			Control.Value = Element.Value;
		}
	}
}