using System;
using System.ComponentModel;
using System.Linq;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class EffectiveFlowDirectionExtensions
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static EffectiveFlowDirection ToEffectiveFlowDirection(this FlowDirection self, EffectiveFlowDirection mode)
		{
			switch (self)
			{
				case FlowDirection.MatchParent:
					return EffectiveFlowDirection.LeftToRight | EffectiveFlowDirection.Implicit;
				case FlowDirection.LeftToRight:
					return EffectiveFlowDirection.LeftToRight | mode;
				case FlowDirection.RightToLeft:
					return EffectiveFlowDirection.RightToLeft | mode;
				default:
					throw new InvalidOperationException($"Cannot convert {self} to {nameof(EffectiveFlowDirection)}.");
			}			
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static FlowDirection ToFlowDirection(this EffectiveFlowDirection self)
		{
			if (self.IsLeftToRight() && !self.IsRightToLeft())
				return FlowDirection.LeftToRight;
			else if (self.IsRightToLeft() && !self.IsLeftToRight())
				return FlowDirection.RightToLeft;

			throw new InvalidOperationException($"Cannot convert {self} to {nameof(FlowDirection)}.");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsRightToLeft(this EffectiveFlowDirection self)
		{
			return (self & EffectiveFlowDirection.RightToLeft) == EffectiveFlowDirection.RightToLeft;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsLeftToRight(this EffectiveFlowDirection self)
		{
			return (self & EffectiveFlowDirection.LeftToRight) == EffectiveFlowDirection.LeftToRight;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsImplicit(this EffectiveFlowDirection self)
		{
			return (self & EffectiveFlowDirection.Implicit) == EffectiveFlowDirection.Implicit;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsExplicit(this EffectiveFlowDirection self)
		{
			return (self & EffectiveFlowDirection.Explicit) == EffectiveFlowDirection.Explicit;
		}
	}
}