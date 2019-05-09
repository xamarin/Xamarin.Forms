﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Xamarin.Forms
{
	public static class Routing
	{
		static int s_routeCount = 0;
		static Dictionary<string, RouteFactory> s_routes = new Dictionary<string, RouteFactory>();

		internal const string ImplicitPrefix = "IMPL_";
		const string _pathSeparator = "/";

		internal static string GenerateImplicitRoute(string source)
		{
			if (IsImplicit(source))
				return source;
			return String.Concat(ImplicitPrefix, source);
		}
		internal static bool IsImplicit(string source)
		{
			return source.StartsWith(ImplicitPrefix, StringComparison.Ordinal);
		}
		internal static bool IsImplicit(Element source)
		{
			return IsImplicit(GetRoute(source));
		}

		internal static bool CompareWithRegisteredRoutes(string compare) => s_routes.ContainsKey(compare);

		internal static void Clear()
		{
			s_routes.Clear();
		}

		public static readonly BindableProperty RouteProperty =
			BindableProperty.CreateAttached("Route", typeof(string), typeof(Routing), null,
				defaultValueCreator: CreateDefaultRoute);

		static object CreateDefaultRoute(BindableObject bindable)
		{
			return bindable.GetType().Name + ++s_routeCount;
		}

		internal static string[] GetRouteKeys()
		{
			string[] keys = new string[s_routes.Count];
			s_routes.Keys.CopyTo(keys, 0);
			return keys;
		}

		public static Element GetOrCreateContent(string route)
		{
			Element result = null;

			if (s_routes.TryGetValue(route, out var content))
				result = content.GetOrCreate();

			if (result == null)
			{
				// okay maybe its a type, we'll try that just to be nice to the user
				var type = Type.GetType(route);
				if (type != null)
					result = Activator.CreateInstance(type) as Element;
			}

			if (result != null)
				SetRoute(result, route);

			return result;
		}

		public static string GetRoute(Element obj)
		{
			return (string)obj.GetValue(RouteProperty);
		}

		internal static string GetRoutePathIfNotImplicit(Element obj)
		{
			var source = GetRoute(obj);
			if (IsImplicit(source))
				return String.Empty;

			return $"{source}/";
		}

		internal static Uri RemoveImplicit(Uri uri)
		{
			uri = ShellUriHandler.FormatUri(uri);

			string[] parts = uri.OriginalString.TrimEnd(_pathSeparator[0]).Split(_pathSeparator[0]);

			List<string> toKeep = new List<string>();
			for (int i = 0; i < parts.Length; i++)
				if (!IsImplicit(parts[i]))
					toKeep.Add(parts[i]);

			return new Uri(string.Join(_pathSeparator, toKeep), UriKind.Relative);
		}

		public static string FormatRoute(List<string> segments)
		{
			var route = FormatRoute(String.Join(_pathSeparator, segments));
			return route;
		}

		public static string FormatRoute(string route)
		{
			return route;
		}

		public static void RegisterRoute(string route, RouteFactory factory)
		{
			if (!String.IsNullOrWhiteSpace(route))
				route = FormatRoute(route);
			ValidateRoute(route);

			s_routes[route] = factory;
		}

		public static void UnRegisterRoute(string route)
		{
			if (s_routes.TryGetValue(route, out _))
				s_routes.Remove(route);
		}

		public static void RegisterRoute(string route, Type type)
		{
			if(!String.IsNullOrWhiteSpace(route))
				route = FormatRoute(route);

			ValidateRoute(route);

			s_routes[route] = new TypeRouteFactory(type);
		}

		public static void SetRoute(Element obj, string value)
		{
			obj.SetValue(RouteProperty, value);
		}

		static void ValidateRoute(string route)
		{
			if (string.IsNullOrWhiteSpace(route))
				throw new ArgumentNullException(nameof(route), "Route cannot be an empty string");

			var uri = new Uri(route, UriKind.RelativeOrAbsolute);

			var parts = uri.OriginalString.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var part in parts)
			{
				if (IsImplicit(part))
					throw new ArgumentException($"Route contains invalid characters in \"{part}\"");
			}
		}

		class TypeRouteFactory : RouteFactory
		{
			readonly Type _type;

			public TypeRouteFactory(Type type)
			{
				_type = type;
			}

			public override Element GetOrCreate()
			{
				return (Element)Activator.CreateInstance(_type);
			}
		}
	}
}