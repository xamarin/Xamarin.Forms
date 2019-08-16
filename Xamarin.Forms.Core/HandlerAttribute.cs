using System;

namespace Xamarin.Forms
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public abstract class HandlerAttribute : Attribute
	{
		protected HandlerAttribute(Type handler, Type target) : this(handler, target, null)
		{
		}

		protected HandlerAttribute(Type handler, Type target, Type[] supportedVisuals)
		{
			SupportedVisuals = supportedVisuals ?? new[] { typeof(VisualMarker.DefaultVisual) };
			TargetType = target;
			HandlerType = handler;
		}

		internal Type[] SupportedVisuals { get; private set; }
		internal Type HandlerType { get; private set; }

		internal Type TargetType { get; private set; }

		public virtual bool ShouldRegister()
		{
			return true;
		}
	}
}
