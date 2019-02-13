using System;
using AppKit;
using CoreAnimation;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class FormsNSImageView : NSView
	{
		bool _isOpaque;

		public FormsNSImageView()
		{
			Layer = new CALayer();
			WantsLayer = true;
		}

		public void SetIsOpaque(bool isOpaque)
		{
			_isOpaque = isOpaque;
		}

		public override bool IsOpaque => _isOpaque;

		public override CGSize FittingSize
		{
			get
			{
				var contents = Layer?.Contents;
				if(contents == null)
				{
					return base.FittingSize;
				}
				var scale = (float)NSScreen.MainScreen.BackingScaleFactor;
				var width = contents.Width / scale;
				var height = contents.Height / scale;
				return new CGSize(width, height);
			}
		}
	}
}