using System;
using System.ComponentModel;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CheckBoxRenderer : ViewRenderer<CheckBox, XFCheckBox>
	{
		internal const float DefaultSize = 30.0f;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				Control.CheckedChanged -= OnElementCheckedChanged;
			base.Dispose(disposing);
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			var result = base.SizeThatFits(size);

			var height = result.Height;
			var width = result.Width;

			if (height < DefaultSize)
			{
				height = DefaultSize;
			}

			if (width < DefaultSize)
			{
				width = DefaultSize;
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
					width = DefaultSize;
					set = true;
				}
			}

			if (sizeConstraint.Request.Height == 0)
			{
				if (heightConstraint <= 0 || double.IsInfinity(heightConstraint))
				{
					height = DefaultSize;
					set = true;
				}
			}

			

			if(set)
			{
				sizeConstraint = new SizeRequest(new Size(width, height), new Size(DefaultSize, DefaultSize));
			}

			return sizeConstraint;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			if (e.OldElement != null)
				e.OldElement.CheckedChanged -= OnElementCheckedChanged;

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new XFCheckBox());
				}

				Control.IsChecked = Element.IsChecked;
				Control.IsEnabled = Element.IsEnabled;
				Control.DisabledColor = Color.Default;
				Control.CheckColor = Color.Default;

				e.NewElement.CheckedChanged += OnElementChecked;
				UpdateCheckedColor();
				UpdateUncheckedColor();
			}

			base.OnElementChanged(e);
		}

		void UpdateCheckedColor()
		{
			if (Element == null)
				return;

			Control.CheckedColor = Element.CheckedColor;
		}

		void UpdateUncheckedColor()
		{
			if (Element == null)
				return;

			Control.UncheckedColor = Element.UncheckedColor;
		}

		void OnElementCheckedChanged(object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(CheckBox.IsCheckedProperty, Control.IsChecked);
		}

		void OnElementChecked(object sender, EventArgs e)
		{
			Control.IsChecked = Element.IsChecked;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == CheckBox.CheckedColorProperty.PropertyName)
				UpdateCheckedColor();
			else if (e.PropertyName == CheckBox.UncheckedColorProperty.PropertyName)
				UpdateUncheckedColor();
			else if (e.PropertyName == CheckBox.IsEnabledProperty.PropertyName)
				Control.IsEnabled = Element.IsEnabled;
		}
	}


	public class XFCheckBox : UIControl
	{
		public XFCheckBox()
		{
			BackgroundColor = UIColor.Clear;
		}
		public EventHandler CheckedChanged;

		bool _isChecked;
		public bool IsChecked
		{
			get => _isChecked;
			set
			{
				if (value == _isChecked)
					return;

				_isChecked = value;
				SetNeedsDisplay();
			}
		}

		bool _isEnabled;
		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (value == _isEnabled)
					return;

				_isEnabled = value;
				
				UserInteractionEnabled = IsEnabled;

				SetNeedsDisplay();
			}
		}

		Color _checkColor, _checkedColor, _uncheckedColor, _disabledColor;
		public Color DisabledColor
		{
			get => _disabledColor;
			set
			{
				if (_disabledColor == value)
					return;

				_disabledColor = value;
				SetNeedsDisplay();
			}
		}

		public Color CheckColor
		{
			get => _checkColor;
			set
			{
				if (_checkColor == value)
					return;

				_checkColor = value;
				SetNeedsDisplay();
			}
		}
		public Color CheckedColor
		{
			get => _checkedColor;
			set
			{
				if (_checkedColor == value)
					return;

				_checkedColor = value;
				SetNeedsDisplay();
			}
		}
		public Color UncheckedColor
		{
			get => _uncheckedColor;
			set
			{
				if (_uncheckedColor == value)
					return;

				_uncheckedColor = value;
				SetNeedsDisplay();
			}
		}		

		public override void Draw(CGRect rect)
		{
			//base.Draw(rect);

			if (IsEnabled)
			{
				var checkedColor = (CheckedColor.IsDefault ? TintColor : CheckedColor.ToUIColor());
				checkedColor.SetFill();
				if (IsChecked)
				{
					checkedColor.SetStroke();
				}
				else
				{
					(UncheckedColor.IsDefault ? TintColor : UncheckedColor.ToUIColor()).SetStroke();
				}
			}
			else
			{
				(DisabledColor.IsDefault ? UIColor.Black.ColorWithAlpha(.5f) : DisabledColor.ToUIColor()).SetColor();
			}

			var width = Bounds.Size.Width;
			var height = Bounds.Size.Height;

			var outerDiameter = Math.Min (width, height);
			var lineWidth = 2.0 / CheckBoxRenderer.DefaultSize * outerDiameter;
			var diameter = outerDiameter - 3 * lineWidth;
			var radius = diameter / 2;

			var xOffset = diameter + lineWidth * 2 <= width ? lineWidth * 2 : (width - diameter) / 2;
			var hPadding = xOffset;
			var vPadding = (nfloat)((height - diameter) / 2);

			var backgroundRect = new CGRect(xOffset, vPadding, diameter, diameter);
			var boxPath = UIBezierPath.FromOval(backgroundRect);
			boxPath.LineWidth = (nfloat)lineWidth;
			boxPath.Stroke();
			if (IsChecked)
			{
				boxPath.Fill();
				var checkPath = new UIBezierPath
				{
					LineWidth = (nfloat)0.077,
					LineCapStyle = CGLineCap.Round,
					LineJoinStyle = CGLineJoin.Round
				};
				var context = UIGraphics.GetCurrentContext ();
				context.SaveState ();
				context.TranslateCTM ((nfloat)hPadding + (nfloat)(0.05 * diameter), vPadding + (nfloat)(0.1 * diameter));
				context.ScaleCTM ((nfloat)diameter, (nfloat)diameter);
				checkPath.MoveTo(new CGPoint(0.72f, 0.22f));
				checkPath.AddLineTo(new CGPoint(0.33f, 0.6f));
				checkPath.AddLineTo(new CGPoint(0.15f, 0.42f));
				(CheckColor.IsDefault ? UIColor.White : CheckColor.ToUIColor()).SetStroke();
				checkPath.Stroke();
				context.RestoreState ();
			}

		}

		public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
		{
			IsChecked = !IsChecked;
			CheckedChanged?.Invoke(this, null);
			return base.BeginTracking(uitouch, uievent);
		}
	}
}