using System;

namespace Xamarin.Forms
{
	[Flags]
	public enum EffectiveFlowDirection
	{
		RightToLeft = 1 << 0,
		Explicit = 1 << 1,
		HasExplicitParent = 1 << 2
	}
}