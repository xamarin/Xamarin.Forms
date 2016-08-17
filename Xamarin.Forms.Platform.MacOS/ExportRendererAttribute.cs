using System;

namespace Xamarin.Forms
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportRendererAttribute : HandlerAttribute
	{
		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
			Idiomatic = false;
		}

		internal bool Idiomatic { get; }

		public override bool ShouldRegister()
		{
			return !Idiomatic;
		}
	}
}