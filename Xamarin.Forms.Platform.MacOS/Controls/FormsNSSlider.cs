using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.Platform.macOS.Controls
{
	internal class FormsNSSlider : NSSlider
	{
		readonly CGSize _fitSize;

		internal FormsNSSlider() : base(CGRect.Empty)
		{
			Continuous = true;
			SizeToFit();
			var size = Bounds.Size;
			_fitSize = size.Width > 0 && size.Height > 0 ? size : new CGSize(96, 21);
		}

		public override CGSize SizeThatFits(CGSize size) => _fitSize;
	}
}
