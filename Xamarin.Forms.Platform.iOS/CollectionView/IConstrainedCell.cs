using System;
using CoreGraphics;

namespace Xamarin.Forms.Platform.iOS
{
	public interface IConstrainedCell
	{
		void Constrain(nfloat constant);
		void Constrain(CGSize constraint);
		CGSize Measure();
	}
}