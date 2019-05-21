using System;
using CoreGraphics;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.Material.iOS
{
	public class MaterialFormsCheckBox : FormsCheckBox
	{
		const float _defaultSize = 18.0f;
		const float _lineWidth = 2.0f;

		//UIImage _checkedImage;
		//UIImage _uncheckedImage;

		protected override UIImage CreateCheckBox()
		{
			var checkBoxColor = (CheckBoxTintColor.IsDefault ? base.TintColor : CheckBoxTintColor.ToUIColor());

			/*if (_checkedImage != null && IsChecked)
			{
				var tintedImage = _checkedImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
				ImageView.TintColor = checkBoxColor;
				return tintedImage;
			}*/

			UIGraphics.BeginImageContextWithOptions(new CGSize(_defaultSize, _defaultSize), false, 0);
			var context = UIGraphics.GetCurrentContext();
			context.SaveState();

			
			checkBoxColor.SetFill();
			checkBoxColor.SetStroke();

			var vPadding = _lineWidth / 2;
			var hPadding = _lineWidth / 2;
			var diameter = _defaultSize - _lineWidth;
			var backgroundRect = new CGRect(hPadding, vPadding, diameter, diameter);
			var boxPath = UIBezierPath.FromRoundedRect(backgroundRect, 1);
			boxPath.LineWidth = (nfloat)_lineWidth;
			boxPath.Stroke();
			if (IsChecked)
			{
				boxPath.Fill();
				var checkPath = new UIBezierPath
				{
					LineWidth = (nfloat).12,
					LineCapStyle = CGLineCap.Square,
					LineJoinStyle = CGLineJoin.Miter
				};

				context.TranslateCTM((nfloat)hPadding + (nfloat)(0.05 * diameter), (nfloat)vPadding + (nfloat)(0.1 * diameter));
				context.ScaleCTM((nfloat)diameter, (nfloat)diameter);
				checkPath.MoveTo(new CGPoint(0.80f, 0.14f));
				checkPath.AddLineTo(new CGPoint(0.33f, 0.6f));
				checkPath.AddLineTo(new CGPoint(0.10f, 0.37f));
				(CheckColor.IsDefault ? UIColor.White : CheckColor.ToUIColor()).SetStroke();
				checkPath.Stroke();
			}

			context.RestoreState();
			var img = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return img;
		}
	}
}
