using System;
using System.ComponentModel;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CheckBoxRenderer : ViewRenderer<CheckBox, XFCheckBox>
	{
		internal const float MinimumHeightWidth = 30.0f;

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

			if (height < MinimumHeightWidth)
			{
				height = MinimumHeightWidth;
			}

			if (width < MinimumHeightWidth)
			{
				width = MinimumHeightWidth;
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
					width = MinimumHeightWidth;
					set = true;
				}
			}

			if (sizeConstraint.Request.Height == 0)
			{
				if (heightConstraint <= 0 || double.IsInfinity(heightConstraint))
				{
					height = MinimumHeightWidth;
					set = true;
				}
			}

			

			if(set)
			{
				sizeConstraint = new SizeRequest(new Size(width, height), new Size(MinimumHeightWidth, MinimumHeightWidth));
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
				Control.CheckColor = UIColor.White;

				e.NewElement.CheckedChanged += OnElementChecked;
				UpdateCheckedColor();
				UpdateUnCheckedColor();
			}

			base.OnElementChanged(e);
		}

		void UpdateCheckedColor()
		{
			if (Element == null)
				return;

			if (Element.CheckedColor == Color.Default)
				Control.FillColor = null;
			else
				Control.FillColor = Element.CheckedColor.ToUIColor();
		}

		void UpdateUnCheckedColor()
		{
			if (Element == null)
				return;

			if (Element.UnCheckedColor == Color.Default)
				Control.BorderColor = null;
			else
				Control.BorderColor = Element.UnCheckedColor.ToUIColor();
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
			else if (e.PropertyName == CheckBox.UnCheckedColorProperty.PropertyName)
				UpdateUnCheckedColor();
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

		UIColor _checkColor, _fillColor, _borderColor;
		public UIColor CheckColor
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
		public UIColor FillColor
		{
			get => _fillColor;
			set
			{
				if (_fillColor == value)
					return;

				_fillColor = value;
				SetNeedsDisplay();
			}
		}
		public UIColor BorderColor
		{
			get => _borderColor;
			set
			{
				if (_borderColor == value)
					return;

				_borderColor = value;
				SetNeedsDisplay();
			}
		}		

		public override void Draw(CGRect rect)
		{
			//base.Draw(rect);

			if (IsEnabled)
			{
				(FillColor ?? TintColor).SetFill();
				(BorderColor ?? TintColor).SetStroke();
			}
			else
			{
				UIColor.DarkTextColor.ColorWithAlpha(.5f).SetColor();
			}

			var width = Bounds.Size.Width;
			var height = Bounds.Size.Height;

			width = height = (nfloat)Math.Min(width, height);

            var padding = (nfloat)(2.0 / CheckBoxRenderer.MinimumHeightWidth * width);
			var r = new CGRect(padding, padding, width - 2*padding, height - 2*padding);
			var boxPath = UIBezierPath.FromRoundedRect(r, width / 8);
			
			if (IsChecked)
			{
				boxPath.Fill();
				var checkPath = new UIBezierPath
				{
					LineWidth = 3,
					LineCapStyle = CGLineCap.Round,
					LineJoinStyle = CGLineJoin.Round
				};
				var context = UIGraphics.GetCurrentContext ();
				context.SaveState ();
				context.TranslateCTM (padding, padding);
				checkPath.MoveTo(new CGPoint(width * 0.72f, height * 0.22f));
				checkPath.AddLineTo(new CGPoint(width * 0.33f, height * 0.6f));
				checkPath.AddLineTo(new CGPoint(width * 0.15f, height * 0.42f));
				CheckColor.SetStroke();
				checkPath.Stroke();
				context.RestoreState ();
			}
			else
			{
				boxPath.LineWidth = 2;
				boxPath.Stroke();
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