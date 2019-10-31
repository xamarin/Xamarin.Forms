using System;

namespace Xamarin.Forms.Internals
{
	public struct TypedParameter
	{
		public TypedParameter(Type type, object instance)
		{
			Type = type;
			Instance = instance;
		}

		public Type Type { get; }

		public object Instance { get; }
	}
}