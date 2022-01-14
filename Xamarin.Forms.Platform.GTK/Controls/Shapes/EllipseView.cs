using System;
using Cairo;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
	public class EllipseView : ShapeView
	{
		protected override void Draw(Gdk.Rectangle area, Context cr)
		{
			double width = _width;
			double height = _height;

			cr.Translate(width / 2, height / 2);
			cr.Scale(width / 2, height / 2);
			cr.Arc(0, 0, 1, 0, 2 * Math.PI);

			base.Draw(area, cr);
		}
	}
}
