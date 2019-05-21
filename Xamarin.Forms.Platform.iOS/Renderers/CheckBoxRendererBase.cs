using System;
using System.ComponentModel;
using System.Drawing;
using CoreGraphics;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class CheckBoxRendererBase<T> : ViewRenderer<CheckBox, T>
		where T : FormsCheckBox
	{
		// set to match Android
		protected virtual float MinimumSize => 32.0f;
		bool _isDisposed = false;

		protected CheckBoxRendererBase()
		{
			AccessibilityTraits = UIKit.UIAccessibilityTrait.Button;
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			var result = base.SizeThatFits(size);

			var height = result.Height;
			var width = result.Width;

			if (height < MinimumSize)
			{
				height = MinimumSize;
			}

			if (width < MinimumSize)
			{
				width = MinimumSize;
			}

			var final = (nfloat)Math.Min(width, height);
			result.Width = final;
			result.Height = final;

			return result;
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var sizeConstraint = base.GetDesiredSize(widthConstraint, heightConstraint);

			var set = false;

			var width = widthConstraint;
			var height = heightConstraint;
			if (sizeConstraint.Request.Width == 0)
			{
				if (widthConstraint <= 0 || double.IsInfinity(widthConstraint))
				{
					width = MinimumSize;
					set = true;
				}
			}

			if (sizeConstraint.Request.Height == 0)
			{
				if (heightConstraint <= 0 || double.IsInfinity(heightConstraint))
				{
					height = MinimumSize;
					set = true;
				}
			}

			if(set)
			{
				sizeConstraint = new SizeRequest(new Size(width, height), new Size(MinimumSize, MinimumSize));
			}

			return sizeConstraint;
		}

		protected abstract override T CreateNativeControl();

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_isDisposed)
			{
				_isDisposed = true;
				Control.CheckedChanged -= OnElementCheckedChanged;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			if (e.OldElement != null)
				e.OldElement.CheckedChanged -= OnElementCheckedChanged;

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(CreateNativeControl());
				}

				Control.MinimumViewSize = MinimumSize;
				Control.IsChecked = Element.IsChecked;
				Control.IsEnabled = Element.IsEnabled;
				Control.CheckColor = Color.Default;

				e.NewElement.CheckedChanged += OnElementCheckedChanged;
				UpdateTintColor();
			}

			base.OnElementChanged(e);
		}

		protected virtual void UpdateTintColor()
		{
			if (Element == null)
				return;

			Control.CheckBoxTintColor = Element.TintColor;
		}

		void OnElementCheckedChanged(object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(CheckBox.IsCheckedProperty, Control.IsChecked);
			Control.IsChecked = Element.IsChecked;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == CheckBox.TintColorProperty.PropertyName)
				UpdateTintColor();
			else if (e.PropertyName == CheckBox.IsEnabledProperty.PropertyName)
				Control.IsEnabled = Element.IsEnabled;
		}
	}
}
