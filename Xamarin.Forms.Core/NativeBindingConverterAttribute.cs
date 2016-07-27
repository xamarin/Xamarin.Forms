using System;
namespace Xamarin.Forms
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class NativeBindingConverterAttribute : HandlerAttribute
	{
		public NativeBindingConverterAttribute(Type handler, Type target) : base(handler, target)
		{
		}
	}
}

