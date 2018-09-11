using System;
using CoreGraphics;

namespace Xamarin.Forms.Platform.iOS
{
	public interface IConstrainedCell
	{
		void SetConstrainedDimension(nfloat constant);
	}
}