using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Xamarin.Forms
{
	internal class ShellUriHandler
	{
		static readonly char[] _pathSeparators = { '/', '\\' };
		const string _pathSeparator = "/";

		static internal string Route = Routing.GenerateImplicitRoute("shell");
		static internal string RouteHost => "shell";
		static internal string RouteScheme => "app";

		internal static Uri FormatUri(Uri path)
		{
			if (path.IsAbsoluteUri)
				return new Uri(FormatUri(path.OriginalString), UriKind.Absolute);

			return new Uri(FormatUri(path.OriginalString), UriKind.Relative);
		}

		internal static string FormatUri(string path)
		{
			return path.Replace(_pathSeparators[1], _pathSeparator[0]);
		}

		internal static Uri CreateUri(string path)
		{
			path = FormatUri(path);

			// on iOS if the uri starts with // it'll instantiate as absolute with
			// file: as the default scheme where as android just crashes
			// so this checks if it starts with / and just forces relative
			if (path.StartsWith(_pathSeparator, StringComparison.Ordinal))
				return new Uri(path, UriKind.Relative);

			if (Uri.TryCreate(path, UriKind.Absolute, out Uri result))
				return result;

			return new Uri(path, UriKind.Relative);
		}

		public static Uri ConvertToStandardFormat(Uri request)
		{
			request = FormatUri(request);
			string pathAndQuery = null;
			if (request.IsAbsoluteUri)
				pathAndQuery = $"{request.Host}/{request.PathAndQuery}";
			else
				pathAndQuery = request.OriginalString;

			var segments = new List<string>(pathAndQuery.Split(_pathSeparators, StringSplitOptions.RemoveEmptyEntries));


			if (segments.Count > 0 && segments[0] != RouteHost)
				segments.Insert(0, RouteHost);

			if (segments.Count > 1 && segments[1] != Route)
				segments.Insert(1, Route);

			var path = String.Join(_pathSeparator, segments.ToArray());
			string uri = $"{RouteScheme}://{path}";

			return new Uri(uri);
		}

		internal static ShellRouteState GetNavigationRequest(Shell shell, Uri uri, bool enableRelativeShellRoutes = false)
		{
			uri = FormatUri(uri);
			Uri request = ConvertToStandardFormat(uri);
			List<RouteRequestBuilder> possibleRouteMatches = GenerateRoutePaths(shell, request, uri, enableRelativeShellRoutes);

			if (possibleRouteMatches.Count == 0)
				throw new ArgumentException($"unable to figure out route for: {uri}", nameof(uri));
			else if (possibleRouteMatches.Count > 1)
			{
				string[] matches = new string[possibleRouteMatches.Count];
				int i = 0;
				foreach (var match in possibleRouteMatches)
				{
					matches[i] = match.PathFull;
					i++;
				}

				string matchesFound = String.Join(",", matches);
				throw new ArgumentException($"Ambiguous routes matched for: {uri} matches found: {matchesFound}", nameof(uri));

			}

			var theWinningRoute = possibleRouteMatches[0];
			List<PathPart> pathParts = new List<PathPart>();
			ShellRouteState returnValue = shell.RouteState ?? new ShellRouteState();

			if (theWinningRoute.Item == null)
			{
				for (var i = 0; i < theWinningRoute.GlobalRouteMatches.Count; i++)
				{
					var item = new ShellContent()
					{
						Route = theWinningRoute.GlobalRouteMatches[i]
					};

					pathParts.Add(new PathPart(item, Shell.GetNavigationParameters(item, request.Query, theWinningRoute.GlobalRouteMatches.Count == (i + 1))));
				}

				returnValue = returnValue.Add(pathParts);
			}
			else
			{
				var shellItem = theWinningRoute.Item;
				ShellSection shellSection = theWinningRoute.Section;

				if (shellSection == null)
					shellSection = shellItem.Items[0];

				ShellContent shellContent = theWinningRoute.Content;
				if (shellContent == null)
					shellContent = shellSection.Items[0];

				pathParts.Add(new PathPart(shellItem, Shell.GetNavigationParameters(shellItem, request.Query, false)));
				pathParts.Add(new PathPart(shellSection, Shell.GetNavigationParameters(shellSection, request.Query, false)));

				if(shellContent != null)
					pathParts.Add(new PathPart(shellContent, Shell.GetNavigationParameters(shellContent, request.Query, theWinningRoute.GlobalRouteMatches.Count == 0)));
				
				for (var i = 0; i < theWinningRoute.GlobalRouteMatches.Count; i++)
				{
					var item = new ShellContent() { Route = theWinningRoute.GlobalRouteMatches[i] };
					pathParts.Add(new PathPart(item, Shell.GetNavigationParameters(item, request.Query, theWinningRoute.GlobalRouteMatches.Count == (i + 1))));
				}
				returnValue = new ShellRouteState(new RoutePath(pathParts, Shell.GetNavigationParameters(shell, request.Query, false)));
			}

			return returnValue;
		}

		internal static List<RouteRequestBuilder> GenerateRoutePaths(Shell shell, Uri request)
		{
			request = FormatUri(request);
			return GenerateRoutePaths(shell, request, request, false);
		}

		internal static List<RouteRequestBuilder> GenerateRoutePaths(Shell shell, Uri request, Uri originalRequest, bool enableRelativeShellRoutes)
		{
			var routeKeys = Routing.GetRouteKeys();
			for (int i = 0; i < routeKeys.Length; i++)
			{
				if (routeKeys[i] == originalRequest.OriginalString)
				{
					var builder = new RouteRequestBuilder(routeKeys[i], routeKeys[i], null, new string[] { routeKeys[i] });
					return new List<RouteRequestBuilder> { builder };
				}
				routeKeys[i] = FormatUri(routeKeys[i]);
			}

			request = FormatUri(request);
			originalRequest = FormatUri(originalRequest);

			List<RouteRequestBuilder> possibleRoutePaths = new List<RouteRequestBuilder>();
			if (!request.IsAbsoluteUri)
				request = ConvertToStandardFormat(request);

			string localPath = request.LocalPath;

			bool relativeMatch = false;
			if (!originalRequest.IsAbsoluteUri &&
				!originalRequest.OriginalString.StartsWith("//", StringComparison.Ordinal))
				relativeMatch = true;

			var segments = localPath.Split(_pathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

			if (!relativeMatch)
			{
				for (int i = 0; i < routeKeys.Length; i++)
				{
					var route = routeKeys[i];
					var uri = ConvertToStandardFormat(CreateUri(route));
					if (uri.Equals(request))
					{
						throw new Exception($"Global routes currently cannot be the only page on the stack, so absolute routing to global routes is not supported. For now, just navigate to: {originalRequest.OriginalString.Replace("//", "")}");
						//var builder = new RouteRequestBuilder(route, route, null, segments);
						//return new List<RouteRequestBuilder> { builder };
					}
				}
			}

			var depthStart = 0;

			if (segments[0] == shell.Route)
			{
				segments = segments.Skip(1).ToArray();
				depthStart = 1;
			}
			else
			{
				depthStart = 0;
			}

			if (relativeMatch && shell?.CurrentItem != null)
			{
				// retrieve current location
				var currentLocation = NodeLocation.Create(shell);

				while (currentLocation.Shell != null)
				{
					var pureRoutesMatch = new List<RouteRequestBuilder>();
					var pureGlobalRoutesMatch = new List<RouteRequestBuilder>();

					//currently relative routes to shell routes isn't supported as we aren't creating navigation stacks
					if (enableRelativeShellRoutes)
					{
						SearchPath(currentLocation.LowestChild, null, segments, pureRoutesMatch, 0);
						ExpandOutGlobalRoutes(pureRoutesMatch, routeKeys);
						pureRoutesMatch = GetBestMatches(pureRoutesMatch);
						if (pureRoutesMatch.Count > 0)
						{
							return pureRoutesMatch;
						}
					}


					SearchPath(currentLocation.LowestChild, null, segments, pureGlobalRoutesMatch, 0, ignoreGlobalRoutes: false);
					ExpandOutGlobalRoutes(pureGlobalRoutesMatch, routeKeys);
					pureGlobalRoutesMatch = GetBestMatches(pureGlobalRoutesMatch);
					if (pureGlobalRoutesMatch.Count > 0)
					{
						// currently relative routes to shell routes isn't supported as we aren't creating navigation stacks
						// So right now we will just throw an exception so that once this is implemented
						// GotoAsync doesn't start acting inconsistently and all of a suddent starts creating routes
						if (!enableRelativeShellRoutes && pureGlobalRoutesMatch[0].SegmentsMatched.Count > 0)
						{
							throw new Exception($"Relative routing to shell elements is currently not supported. Try prefixing your uri with ///: ///{originalRequest}");
						}

						return pureGlobalRoutesMatch;
					}

					currentLocation.Pop();
				}

				string searchPath = String.Join(_pathSeparator, segments);

				if (routeKeys.Contains(searchPath))
				{
					return new List<RouteRequestBuilder> { new RouteRequestBuilder(searchPath, searchPath, null, segments) };
				}

				RouteRequestBuilder builder = null;
				foreach (var segment in segments)
				{
					if (routeKeys.Contains(segment))
					{
						if (builder == null)
							builder = new RouteRequestBuilder(segment, segment, null, segments);
						else
							builder.AddGlobalRoute(segment, segment);
					}
				}

				if (builder != null && builder.IsFullMatch)
					return new List<RouteRequestBuilder> { builder };
			}
			else
			{
				possibleRoutePaths.Clear();
				SearchPath(shell, null, segments, possibleRoutePaths, depthStart);

				var bestMatches = GetBestMatches(possibleRoutePaths);
				if (bestMatches.Count > 0)
					return bestMatches;

				bestMatches.Clear();
				ExpandOutGlobalRoutes(possibleRoutePaths, routeKeys);
			}

			possibleRoutePaths = GetBestMatches(possibleRoutePaths);
			return possibleRoutePaths;
		}

		internal static void ExpandOutGlobalRoutes(List<RouteRequestBuilder> possibleRoutePaths, string[] routeKeys)
		{
			foreach (var possibleRoutePath in possibleRoutePaths)
			{
				while (routeKeys.Contains(possibleRoutePath.NextSegment) || routeKeys.Contains(possibleRoutePath.RemainingPath))
				{
					if (routeKeys.Contains(possibleRoutePath.NextSegment))
						possibleRoutePath.AddGlobalRoute(possibleRoutePath.NextSegment, possibleRoutePath.NextSegment);
					else
						possibleRoutePath.AddGlobalRoute(possibleRoutePath.RemainingPath, possibleRoutePath.RemainingPath);
				}

				while (!possibleRoutePath.IsFullMatch)
				{
					NodeLocation nodeLocation = new NodeLocation();
					nodeLocation.SetNode(possibleRoutePath.LowestChild);
					List<RouteRequestBuilder> pureGlobalRoutesMatch = new List<RouteRequestBuilder>();
					while (nodeLocation.Shell != null && pureGlobalRoutesMatch.Count == 0)
					{
						SearchPath(nodeLocation.LowestChild, null, possibleRoutePath.RemainingSegments, pureGlobalRoutesMatch, 0, ignoreGlobalRoutes: false);
						nodeLocation.Pop();
					}

					// nothing found or too many things found
					if (pureGlobalRoutesMatch.Count != 1 || pureGlobalRoutesMatch[0].GlobalRouteMatches.Count == 0)
					{
						break;
					}


					for (var i = 0; i < pureGlobalRoutesMatch[0].GlobalRouteMatches.Count; i++)
					{
						var match = pureGlobalRoutesMatch[0];
						possibleRoutePath.AddGlobalRoute(match.GlobalRouteMatches[i], match.SegmentsMatched[i]);
					}
				}
			}
		}

		internal static List<RouteRequestBuilder> GetBestMatches(List<RouteRequestBuilder> possibleRoutePaths)
		{
			List<RouteRequestBuilder> bestMatches = new List<RouteRequestBuilder>();
			foreach (var match in possibleRoutePaths)
			{
				if (match.IsFullMatch)
					bestMatches.Add(match);
			}

			return bestMatches;
		}

		internal class NodeLocation
		{
			public Shell Shell { get; private set; }
			public ShellItem Item { get; private set; }
			public ShellSection Section { get; private set; }
			public ShellContent Content { get; private set; }
			public object LowestChild =>
				(object)Content ?? (object)Section ?? (object)Item ?? (object)Shell;


			public static NodeLocation Create(Shell shell)
			{
				NodeLocation location = new NodeLocation();
				location.SetNode(
					(object)shell.CurrentItem?.CurrentItem?.CurrentItem ??
					(object)shell.CurrentItem?.CurrentItem ??
					(object)shell.CurrentItem ??
					(object)shell);

				return location;
			}

			public void SetNode(object node)
			{
				switch (node)
				{
					case Shell shell:
						Shell = shell;
						Item = null;
						Section = null;
						Content = null;
						break;
					case ShellItem item:
						Item = item;
						Section = null;
						Content = null;
						if (Shell == null)
							Shell = (Shell)Item.Parent;
						break;
					case ShellSection section:
						Section = section;

						if (Item == null)
							Item = Section.Parent as ShellItem;

						if (Shell == null)
							Shell = (Shell)Item.Parent;

						Content = null;

						break;
					case ShellContent content:
						Content = content;
						if (Section == null)
							Section = Content.Parent as ShellSection;

						if (Item == null)
							Item = Section.Parent as ShellItem;

						if (Shell == null)
							Shell = (Shell)Item.Parent;

						break;

				}
			}

			public Uri GetUri()
			{
				List<string> paths = new List<string>();
				paths.Add(Shell.RouteHost);
				paths.Add(Shell.Route);
				if (Item != null && !Routing.IsImplicit(Item))
					paths.Add(Item.Route);
				if (Section != null && !Routing.IsImplicit(Section))
					paths.Add(Section.Route);
				if (Content != null && !Routing.IsImplicit(Content))
					paths.Add(Content.Route);

				string uri = String.Join(_pathSeparator, paths);
				return new Uri($"{Shell.RouteScheme}://{uri}");
			}

			public void Pop()
			{
				if (Content != null)
					Content = null;
				else if (Section != null)
					Section = null;
				else if (Item != null)
					Item = null;
				else if (Shell != null)
					Shell = null;
			}
		}

		static void SearchPath(
			object node,
			RouteRequestBuilder currentMatchedPath,
			string[] segments,
			List<RouteRequestBuilder> possibleRoutePaths,
			int depthToStart,
			int myDepth = -1,
			NodeLocation currentLocation = null,
			bool ignoreGlobalRoutes = true)
		{
			if (node is GlobalRouteItem && ignoreGlobalRoutes)
				return;

			++myDepth;
			currentLocation = currentLocation ?? new NodeLocation();
			currentLocation.SetNode(node);

			IEnumerable items = null;
			if (depthToStart > myDepth)
			{
				items = GetItems(node);
				if (items == null)
					return;

				foreach (var nextNode in items)
				{
					SearchPath(nextNode, null, segments, possibleRoutePaths, depthToStart, myDepth, currentLocation, ignoreGlobalRoutes);
				}
				return;
			}

			string shellSegment = GetRoute(node);
			string userSegment = null;

			if (currentMatchedPath == null)
			{
				userSegment = segments[0];
			}
			else
			{
				userSegment = currentMatchedPath.NextSegment;
			}

			if (userSegment == null)
				return;

			RouteRequestBuilder builder = null;
			if (shellSegment == userSegment || Routing.IsImplicit(shellSegment))
			{
				if (currentMatchedPath == null)
					builder = new RouteRequestBuilder(shellSegment, userSegment, node, segments);
				else
				{
					builder = new RouteRequestBuilder(currentMatchedPath);
					builder.AddMatch(shellSegment, userSegment, node);
				}

				if (!Routing.IsImplicit(shellSegment) || shellSegment == userSegment)
					possibleRoutePaths.Add(builder);
			}

			items = GetItems(node);
			if (items == null)
				return;

			foreach (var nextNode in items)
			{
				SearchPath(nextNode, builder, segments, possibleRoutePaths, depthToStart, myDepth, currentLocation, ignoreGlobalRoutes);
			}
		}

		static string GetRoute(object node)
		{
			switch (node)
			{
				case Shell shell:
					return shell.Route;
				case ShellItem item:
					return item.Route;
				case ShellSection section:
					return section.Route;
				case ShellContent content:
					return content.Route;
				case GlobalRouteItem routeItem:
					return routeItem.Route;

			}

			throw new ArgumentException($"{node}", nameof(node));
		}

		static IEnumerable GetItems(object node)
		{
			IEnumerable results = null;
			switch (node)
			{
				case Shell shell:
					results = shell.Items;
					break;
				case ShellItem item:
					results = item.Items;
					break;
				case ShellSection section:
					results = section.Items;
					break;
				case ShellContent content:
					results = new object[0];
					break;
				case GlobalRouteItem routeITem:
					results = routeITem.Items;
					break;
			}

			if (results == null)
				throw new ArgumentException($"{node}", nameof(node));

			foreach (var result in results)
				yield return result;

			if (node is GlobalRouteItem)
				yield break;

			var keys = Routing.GetRouteKeys();
			string route = GetRoute(node);
			for (var i = 0; i < keys.Length; i++)
			{
				var key = FormatUri(keys[i]);
				if (key.StartsWith(_pathSeparator, StringComparison.Ordinal) && !(node is Shell))
					continue;

				var segments = key.Split(_pathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

				if (segments[0] == route)
				{
					yield return new GlobalRouteItem(key, key);
				}
			}
		}


		internal class GlobalRouteItem
		{
			readonly string _path;
			public GlobalRouteItem(string path, string sourceRoute)
			{
				_path = path;
				SourceRoute = sourceRoute;
			}

			public IEnumerable Items
			{
				get
				{
					var segments = _path.Split(_pathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList().Skip(1).ToList();

					if (segments.Count == 0)
						return new object[0];

					var route = Routing.FormatRoute(segments);

					return new[] { new GlobalRouteItem(route, SourceRoute) };
				}
			}

			public string Route
			{
				get
				{
					var segments = _path.Split(_pathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

					if (segments.Length == 0)
						return string.Empty;

					return segments[0];
				}
			}

			public bool IsFinished
			{
				get
				{
					var segments = _path.Split(_pathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList().Skip(1).ToList();

					if (segments.Count == 0)
						return true;

					return false;
				}
			}

			public string SourceRoute { get; }
		}
	}



	//[DebuggerDisplay("RequestDefinition = {Request}, StackRequest = {StackRequest}")]
	//internal class NavigationRequest
	//{
	//	public enum WhatToDoWithTheStack
	//	{
	//		ReplaceIt,
	//		PushToIt
	//	}

	//	public NavigationRequest(RequestDefinition definition, WhatToDoWithTheStack stackRequest, string query, string fragment)
	//	{
	//		StackRequest = stackRequest;
	//		Query = query;
	//		Fragment = fragment;
	//		Request = definition;
	//	}

	//	public WhatToDoWithTheStack StackRequest { get; }
	//	public string Query { get; }
	//	public string Fragment { get; }
	//	public RequestDefinition Request { get; }
	//}


	//[DebuggerDisplay("Full = {FullUri}, Short = {ShortUri}")]
	//internal class RequestDefinition
	//{
	//	public RequestDefinition(Uri fullUri, ShellItem item, ShellSection section, ShellContent content, List<string> globalRoutes)
	//	{
	//		FullUri = fullUri;
	//		Item = item;
	//		Section = section;
	//		Content = content;
	//		GlobalRoutes = globalRoutes;
	//	}

	//	public RequestDefinition(string fullUri, ShellItem item, ShellSection section, ShellContent content, List<string> globalRoutes) :
	//		this(new Uri(fullUri, UriKind.Absolute), item, section, content, globalRoutes)
	//	{
	//	}

	//	public Uri FullUri { get; }
	//	public ShellItem Item { get; }
	//	public ShellSection Section { get; }
	//	public ShellContent Content { get; }
	//	public List<string> GlobalRoutes { get; }
	//}
}
