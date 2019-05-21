using System;
using CoreGraphics;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.Material.iOS
{
	public class MaterialFormsCheckBox : FormsCheckBox
	{
		float DefaultSize => 18.0f;

		public override void Draw(CGRect rect)
		{
			var checkedColor = (CheckBoxTintColor.IsDefault ? base.TintColor : CheckBoxTintColor.ToUIColor());
			checkedColor.SetFill();
			checkedColor.SetStroke();

			// all these values were chosen to just match the android drawables that are used
			var width = DefaultSize;
			var height = DefaultSize;

			var outerDiameter = Math.Min(width, height);
			var lineWidth = 2.0;
			var diameter = outerDiameter - lineWidth;
			var vPadding = (Bounds.Height - height + (float)lineWidth) / 2;
			var xOffset = (MinimumViewSize - height + (float)lineWidth) / 4;
			var hPadding = xOffset;
			var backgroundRect = new CGRect(xOffset, vPadding, diameter, diameter);
			var boxPath = UIBezierPath.FromRoundedRect(backgroundRect, 1);
			boxPath.LineWidth = (nfloat)lineWidth;
			boxPath.Stroke();
			if (IsChecked)
			{
				boxPath.Fill();
				var checkPath = new UIBezierPath
				{
					LineWidth = (nfloat)0.12,
					LineCapStyle = CGLineCap.Square,
					LineJoinStyle = CGLineJoin.Miter
				};
				var context = UIGraphics.GetCurrentContext();
				context.SaveState();
				context.TranslateCTM((nfloat)hPadding + (nfloat)(0.05 * diameter), vPadding + (nfloat)(0.1 * diameter));
				context.ScaleCTM((nfloat)diameter, (nfloat)diameter);
				checkPath.MoveTo(new CGPoint(0.80f, 0.14f));
				checkPath.AddLineTo(new CGPoint(0.33f, 0.6f));
				checkPath.AddLineTo(new CGPoint(0.10f, 0.37f));
				(CheckColor.IsDefault ? UIColor.White : CheckColor.ToUIColor()).SetStroke();
				checkPath.Stroke();
				context.RestoreState();
			}
		}
	}
}
