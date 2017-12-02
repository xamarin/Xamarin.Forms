using System;

namespace Xamarin.Forms
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RuntimeNamePropertyAttribute : Attribute
	{
		public RuntimeNamePropertyAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}