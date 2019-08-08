using System;

namespace Xamarin.Forms
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public abstract class HandlerAttribute : Attribute
	{
		protected HandlerAttribute(Type handler, Type target) : this(handler, target, null)
		{
		}

		protected HandlerAttribute(Type handler, Type target, Type[] supportedVisuals) : this(handler, target, supportedVisuals, 0)
		{
		}

		protected HandlerAttribute(Type handler, Type target, Type[] supportedVisuals, short priority)
		{
			SupportedVisuals = supportedVisuals ?? new[] { typeof(VisualMarker.DefaultVisual) };
			TargetType = target;
			HandlerType = handler;
			Priority = priority;
		}

		internal short Priority { get; private set; }
		internal Type[] SupportedVisuals { get; private set; }
		internal Type HandlerType { get; private set; }

		internal Type TargetType { get; private set; }

		public virtual bool ShouldRegister()
		{
			return true;
		}
	}
}
