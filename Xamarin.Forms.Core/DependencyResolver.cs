using System;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.Internals
{
	public static class DependencyResolver
	{
		public delegate object ResolveDelegate(Type type, params object[] args);

		static ResolveDelegate Resolver { get; set; }

		public static void ResolveUsing(ResolveDelegate resolveDelegate)
		{
			Resolver = resolveDelegate;
		}

		public static void ResolveUsing(Func<Type, object> resolveFunc)
		{
			object ResolveDelegate(Type type, object[] args) => resolveFunc.Invoke(type);
			Resolver = ResolveDelegate;
		}

		internal static object Resolve(Type type, params object[] args)
		{
			return Resolver?.Invoke(type, args);
		}

		internal static object ForceResolve(Type type, params object[] args)
		{
			var result = Resolve(type, args);

			if (result != null) return result;

			if (args.Length > 0)
			{
				// This is by no means a general solution to matching with the correct constructor, but it'll
				// do for finding Android renderers which need Context (vs older custom renderers which may still use
				// parameterless constructors)
				if (type.GetTypeInfo().DeclaredConstructors.Any(info => info.GetParameters().Length == args.Length))
				{
					return Activator.CreateInstance(type, args);
				}
			}
			
			return Activator.CreateInstance(type);
		}
	}
}