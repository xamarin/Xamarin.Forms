using System;

namespace Xamarin.Forms
{
	internal static class LayoutAlignmentExtensions
	{
		public static double ToDouble(this LayoutAlignment align)
		{
			switch (align)
			{
				case LayoutAlignment.Start:
					return 0;
				case LayoutAlignment.Center:
					return 0.5;
				case LayoutAlignment.End:
					return 1;
                default:// TODO - Maybe there is a specific value for LayoutAlignment.Fill ?
                    throw new ArgumentOutOfRangeException("align");
            }
		}
	}
}