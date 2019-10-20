using System;
using AppKit;
using CoreAnimation;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	public class FormsNSImageView : NSImageView, IImageView
	{
		bool _isOpaque;

		public void SetIsOpaque(bool isOpaque)
		{
			_isOpaque = isOpaque;
		}

		public override bool IsOpaque => _isOpaque;
	}
}