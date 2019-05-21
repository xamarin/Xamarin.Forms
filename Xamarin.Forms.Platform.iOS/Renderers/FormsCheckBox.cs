using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class FormsCheckBox : UIButton
	{
		// all these values were chosen to just match the android drawables that are used
		const float _defaultSize = 18.0f;
		const float _lineWidth = 2.0f;
		Color _checkColor, _tintColor;
		bool _isChecked;
		bool _isEnabled;
		float _minimumViewSize;

		public EventHandler CheckedChanged;

		internal float MinimumViewSize
		{
			get { return _minimumViewSize; }
			set
			{
				_minimumViewSize = value;
				var xOffset = (value - _defaultSize + _lineWidth) / 4;
				ContentEdgeInsets = new UIEdgeInsets(0, xOffset, 0, 0);
			}
		}

		public FormsCheckBox() : base(UIButtonType.System)
		{
			TouchUpInside += OnTouchUpInside;
			ContentMode = UIViewContentMode.Center;
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			VerticalAlignment = UIControlContentVerticalAlignment.Center;
		}

		void OnTouchUpInside(object sender, EventArgs e)
		{
			IsChecked = !IsChecked;
			CheckedChanged?.Invoke(this, null);
		}

		public bool IsChecked
		{
			get => _isChecked;
			set
			{
				if (value == _isChecked)
					return;

				_isChecked = value;
				UpdateDisplay();
			}
		}

		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (value == _isEnabled)
					return;

				_isEnabled = value;
				UserInteractionEnabled = IsEnabled;
				UpdateDisplay();
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
				UpdateDisplay();
			}
		}

		public Color CheckBoxTintColor
		{
			get => _tintColor;
			set
			{
				if (_tintColor == value)
					return;

				_tintColor = value;
				UpdateDisplay();
			}
		}

		internal void UpdateDisplay()
		{
			this.SetImage(CreateCheckBox().ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			SetNeedsDisplay();
		}

		protected virtual UIImage CreateCheckBox()
		{
			UIGraphics.BeginImageContextWithOptions(new CGSize(_defaultSize, _defaultSize), false, 0);
			var context = UIGraphics.GetCurrentContext();
			context.SaveState();

			var checkedColor = (CheckBoxTintColor.IsDefault ? base.TintColor : CheckBoxTintColor.ToUIColor());
			checkedColor.SetFill();
			checkedColor.SetStroke();

			var vPadding = _lineWidth / 2;
			var hPadding = _lineWidth / 2;
			var diameter = _defaultSize - _lineWidth;
			var backgroundRect = new CGRect(hPadding, vPadding, diameter, diameter);
			var boxPath = UIBezierPath.FromOval(backgroundRect);
			boxPath.LineWidth = (nfloat)_lineWidth;
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

				context.TranslateCTM((nfloat)hPadding + (nfloat)(0.05 * diameter), (nfloat)vPadding + (nfloat)(0.1 * diameter));
				context.ScaleCTM((nfloat)diameter, (nfloat)diameter);
				checkPath.MoveTo(new CGPoint(0.72f, 0.22f));
				checkPath.AddLineTo(new CGPoint(0.33f, 0.6f));
				checkPath.AddLineTo(new CGPoint(0.15f, 0.42f));
				(CheckColor.IsDefault ? UIColor.White : CheckColor.ToUIColor()).SetStroke();
				checkPath.Stroke();
			}

			context.RestoreState();
			var img = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return img;
		}

		protected override void Dispose(bool disposing)
		{
			TouchUpInside -= OnTouchUpInside;
			base.Dispose(disposing);
		}
	}
}