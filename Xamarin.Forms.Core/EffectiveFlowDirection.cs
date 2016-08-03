using System;

namespace Xamarin.Forms
{
	[Flags]
	public enum EffectiveFlowDirection
	{
		LeftToRight = 1 << 0,
		RightToLeft = 1 << 1,
		Implicit = 1 << 2,
		Explicit = 1 << 3
	}
}