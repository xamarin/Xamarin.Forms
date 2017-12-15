using System;

namespace Xamarin.Forms.Xaml
{
	[AttributeUsage(AttributeTargets.Method
		, Inherited = true
		, AllowMultiple = true)]
	sealed public class DependedOnAttribute : Attribute
	{

		public DependedOnAttribute(string propertyName)
		{
			PropertyName = propertyName;
		}

		public string PropertyName { get; private set; }
	}
}
