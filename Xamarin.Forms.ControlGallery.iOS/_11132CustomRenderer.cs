﻿using System.Diagnostics;
using CoreAnimation;
using CoreGraphics;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.iOS;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Issue11132Control), typeof(_11132CustomRenderer))]
namespace Xamarin.Forms.ControlGallery.iOS
{
	public class _11132CustomRenderer : VisualElementRenderer<Issue11132Control>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Issue11132Control> e)
        {
            base.OnElementChanged(e);

            var layer = Layer;

            if (layer != null)
            {
                layer.BorderWidth = 10;
                layer.BorderColor = Color.Red.ToCGColor();
                layer.BackgroundColor = Color.Orange.ToCGColor();

                var width = 100;
                var height = 25;

				var clipPath = new CGPath();
                clipPath.MoveToPoint(width, height);
                clipPath.AddLineToPoint(width * 2, height);
                clipPath.AddLineToPoint(width * 2, height * 2);
                clipPath.AddLineToPoint(width, height * 2);
                clipPath.CloseSubpath();

				var clipShapeLayer = new CAShapeLayer
				{
					Path = clipPath
				};
				layer.Mask = clipShapeLayer;
                layer.Mask.Name = null;

                Debug.WriteLine($"_11132CustomRenderer Layer Name { layer.Mask.Name}");
            }
        }
    }
}