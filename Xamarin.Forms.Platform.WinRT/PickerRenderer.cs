﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public class PickerRenderer : ViewRenderer<Picker, FormsComboBox>
	{
		bool _isAnimating;
		Brush _defaultBrush;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Control != null)
				{
					_isAnimating = false;
					Control.SelectionChanged -= OnControlSelectionChanged;
					Control.DropDownOpened -= OnDropDownOpenStateChanged;
					Control.DropDownClosed -= OnDropDownOpenStateChanged;
					Control.OpenAnimationCompleted -= ControlOnOpenAnimationCompleted;
					Control.Loaded -= ControlOnLoaded;
				}
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new FormsComboBox());
					Control.SelectionChanged += OnControlSelectionChanged;
					Control.DropDownOpened += OnDropDownOpenStateChanged;
					Control.DropDownClosed += OnDropDownOpenStateChanged;
					Control.OpenAnimationCompleted += ControlOnOpenAnimationCompleted;
					Control.ClosedAnimationStarted += ControlOnClosedAnimationStarted;
					Control.Loaded += ControlOnLoaded;
				}

				Control.ItemsSource = Element.Items;

				UpdateTitle();
				UpdateSelectedIndex();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
				UpdateSelectedIndex();
			if (e.PropertyName == Picker.TitleProperty.PropertyName)
				UpdateTitle();
			if (e.PropertyName == TimePicker.TextColorProperty.PropertyName)
				UpdateTextColor();
		}

		void ControlOnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			// The defaults from the control template won't be available
			// right away; we have to wait until after the template has been applied
			_defaultBrush = Control.Foreground;
			UpdateTextColor();
		}

		void ControlOnClosedAnimationStarted(object sender, EventArgs eventArgs)
		{
			if (!Control.IsFullScreen)
			{
				// Start refreshing while the control's closing animation runs;
				// OnDropDownOpenStateChanged will take care of stopping the refresh
				StartAnimationRefresh();
			}
		}

		void ControlOnOpenAnimationCompleted(object sender, EventArgs eventArgs)
		{
			_isAnimating = false;
			if (!Control.IsFullScreen)
			{
				// Force a final redraw after the closing animation has completed
				Element?.InvalidateMeasure(InvalidationTrigger.MeasureChanged);
			}
		}

		void OnControlSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Element != null)
				Element.SelectedIndex = Control.SelectedIndex;
		}

		void OnDropDownOpenStateChanged(object sender, object o)
		{
			if (Control.IsDropDownOpen)
			{
				if (Control.IsOpeningAnimated && !Control.IsFullScreen)
				{
					// Start running the animation refresh; 
					// ControlOnOpenAnimationCompleted will take care of stopping it
					StartAnimationRefresh();
				}
				else
				{
					Element?.InvalidateMeasure(InvalidationTrigger.MeasureChanged);
				}
			}
			else
			{
				// The ComboBox is now closed; if we were animating the closure, stop
				_isAnimating = false;
				// and force the final redraw
				Element?.InvalidateMeasure(InvalidationTrigger.MeasureChanged);
			}
		}

		/// <summary>
		///     Forces redraw of the control during opening/closing animations to provide
		///     a smoother sliding animation for the surrounding controls
		///     Only applies on the phone and only when there are fewer than 6 items in the picker
		/// </summary>
		void StartAnimationRefresh()
		{
			_isAnimating = true;
			Task.Factory.StartNew(async () =>
			{
				while (_isAnimating)
				{
					await Task.Delay(16);
					await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Element?.InvalidateMeasure(InvalidationTrigger.MeasureChanged));
				}
			});
		}

		void UpdateSelectedIndex()
		{
			Control.SelectedIndex = Element.SelectedIndex;
		}

		void UpdateTextColor()
		{
			Color color = Element.TextColor;
			Control.Foreground = color.IsDefault ? (_defaultBrush ?? color.ToBrush()) : color.ToBrush();
		}

		void UpdateTitle()
		{
			Control.Header = Element.Title;
		}
	}
}