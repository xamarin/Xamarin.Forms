using Microsoft.Maui;
using SizeF = CoreGraphics.CGSize;

namespace Microsoft.Maui
{
	public static class SizeExtensions
	{
		public static SizeF ToNative(this Size size)
		{
			return new SizeF((float)size.Width, (float)size.Height);
		}
	}
}