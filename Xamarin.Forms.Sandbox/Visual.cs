using Xamarin.Forms;

namespace SkiaSharpVisual
{
	public sealed class SkiaSharp : IVisual
	{
		public static SkiaSharp Instance { get; } = new SkiaSharp();

		internal SkiaSharp()
		{
		}
	}
}
