using System;
using PointF = CoreGraphics.CGPoint;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class FormsScrollViewScrollChangedEventArgs : EventArgs
	{
		public PointF CurrentScrollPoint
		{
			get;
			set;
		}
	}
}
