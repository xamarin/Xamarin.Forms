using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Xamarin.Forms
{

	internal class ShellUriHandler
	{
		static readonly char[] _pathSeparator = { '/', '\\' };

		public static Uri ConvertToStandardFormat(Shell shell, Uri request)
		{
			string pathAndQuery = null;
			if (request.IsAbsoluteUri)
				pathAndQuery = $"{request.Host}/{request.PathAndQuery}";
			else
				pathAndQuery = request.OriginalString;

			var segments = new List<string>(pathAndQuery.Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries));


			if (segments[0] != shell.RouteHost)
				segments.Insert(0, shell.RouteHost);

			if (segments[1] != shell.Route)
				segments.Insert(1, shell.Route);

			var path = String.Join("/", segments.ToArray());
			string uri = $"{shell.RouteScheme}://{path}";

			return new Uri(uri);
		}

		public static NavigationRequest GetNavigationRequest(Shell shell, Uri uri)
		{
			// figure out the intent of the Uri
			NavigationRequest.WhatToDoWithTheStack whatDoIDo = NavigationRequest.WhatToDoWithTheStack.PushToIt;
			if (uri.IsAbsoluteUri)
				whatDoIDo = NavigationRequest.WhatToDoWithTheStack.ReplaceIt;
			else if (uri.OriginalString.StartsWith("//") || uri.OriginalString.StartsWith("\\\\"))
				whatDoIDo = NavigationRequest.WhatToDoWithTheStack.ReplaceIt;
			else
				whatDoIDo = NavigationRequest.WhatToDoWithTheStack.PushToIt;

			Uri request = ConvertToStandardFormat(shell, uri);

			var possibleRouteMatches = GenerateRoutePaths(shell, request, uri);


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
			RequestDefinition definition =
				new RequestDefinition(
					ConvertToStandardFormat(shell, new Uri(theWinningRoute.PathFull, UriKind.RelativeOrAbsolute)),
					new Uri(theWinningRoute.PathNoImplicit, UriKind.RelativeOrAbsolute),
					theWinningRoute.Item, 
					theWinningRoute.Section, 
					theWinningRoute.Content,
					theWinningRoute.GlobalRouteMatches);

			NavigationRequest navigationRequest = new NavigationRequest(definition, whatDoIDo, request.Query, request.Fragment);

			return navigationRequest;
		}

		internal static List<RouteRequestBuilder> GenerateRoutePaths(Shell shell, Uri request)
		{
			return GenerateRoutePaths(shell, request, request);
		}

		internal static List<RouteRequestBuilder> GenerateRoutePaths(Shell shell, Uri request, Uri originalRequest)
		{
			List<RouteRequestBuilder> possibleRoutePaths = new List<RouteRequestBuilder>();
			if(!request.IsAbsoluteUri)
				request = ConvertToStandardFormat(shell, request);

			string pathAndQuery = request.LocalPath;

			bool relativeMatch = false;
			if (originalRequest.OriginalString.StartsWith("/") || originalRequest.OriginalString.StartsWith("\\"))
				relativeMatch = true;

			var segments = pathAndQuery.Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries);

			if (!relativeMatch)
			{
				var registeredRoutes =
					Routing
						.GetRouteKeys()
						.Select(x => new { Uri = ConvertToStandardFormat(shell, new Uri(x, UriKind.RelativeOrAbsolute)), route = x })
						.ToArray();

				// Todo is this supported?
				foreach(var registeredRoute in registeredRoutes)
				{
					if(registeredRoute.Uri.Equals(request))
					{
						RouteRequestBuilder builder = new RouteRequestBuilder(registeredRoute.route, registeredRoute.route, null, segments);
						return new List<RouteRequestBuilder> { builder };
					}
				}
			}

			var depthStart = 0;
			//var depthDirection = 1;
			//var depthLimit = 3;

			if (segments[0] == shell.Route)
			{
				segments = segments.Skip(1).ToArray();
				depthStart = 1;
			}
			else
			{
				depthStart = 0;
			}

			// build this out
			//if(relativeMatch && shell.CurrentItem?.CurrentItem?.CurrentItem  != null)
			//{
			//	NodeLocation location = new NodeLocation();
			//	location.SetNode(shell.CurrentItem?.CurrentItem?.CurrentItem);
			//	var routeMatch = LocateRegisteredRoute(location, "edit", shell);
			//}

			//for (int i = depthStart; i != depthLimit; i += depthDirection)
			{
				SearchPath(shell, null, segments, possibleRoutePaths, depthStart);
				possibleRoutePaths = LocateRegisteredRoute(possibleRoutePaths, shell);
				//if (possibleRoutePaths.Count > 0)
					//break;
			}

			//if(shell.CurrentState != null)
			//{
			//	var location = shell.CurrentState.Location;
			//}

			return possibleRoutePaths;
		}

		internal static List<RouteRequestBuilder> LocateRegisteredRoute(List<RouteRequestBuilder> possibleRoutePaths, Shell shell)
		{
			List<RouteRequestBuilder> bestMatches = new List<RouteRequestBuilder>();

			foreach (var match in possibleRoutePaths)
			{
				bool matchFound = true;
				while(matchFound && !match.IsFullMatch)
				{
					matchFound = false;

					NodeLocation nodeLocation = new NodeLocation();
					nodeLocation.SetNode((object)match.Content ?? (object)match.Section ?? (object)match.Item);

					string locatedRoute = LocateRegisteredRoute(nodeLocation, match.NextSegment, shell);
					if (!String.IsNullOrWhiteSpace(locatedRoute))
					{
						match.AddGlobalRoute(locatedRoute, match.NextSegment);
						matchFound = true;
					}
				}

				if (match.IsFullMatch)
					bestMatches.Add(match);
			}

			return bestMatches;
		}

		internal static string LocateRegisteredRoute(NodeLocation startingPoint, string route, Shell shell)
		{			
			var registeredRoutes = 
				Routing
					.GetRouteKeys()
					.Select(x => new { Uri = ConvertToStandardFormat(shell, new Uri(x, UriKind.RelativeOrAbsolute)), route = x })
					.ToArray();

			while (startingPoint.Shell != null)
			{
				foreach (var registeredRoute in registeredRoutes)
				{
					Uri combined = new Uri($"{startingPoint.GetUri()}/{route}");
					if(combined.Equals(registeredRoute.Uri))
					{
						return registeredRoute.route;
					}
				}

				startingPoint.Pop();
			}

			return String.Empty;
		}

		internal class NodeLocation
		{
			public Shell Shell { get; private set; }
			public ShellItem Item { get; private set; }
			public ShellSection Section { get; private set; }
			public ShellContent Content { get; private set; }

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

				string uri = String.Join("/", paths.ToArray());
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
			NodeLocation currentLocation = null)
		{
			++myDepth;
			currentLocation = currentLocation ?? new NodeLocation();
			currentLocation.SetNode(node);

			IEnumerable items = null;
			if (depthToStart > myDepth)
			{
				items = GetItems(node);
				if (items == null)
					return;

				foreach (var nextNode in GetItems(node))
				{
					SearchPath(nextNode, null, segments, possibleRoutePaths, depthToStart, myDepth, currentLocation);
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

			foreach (var nextNode in GetItems(node))
			{
				SearchPath(nextNode, builder, segments, possibleRoutePaths, depthToStart, myDepth, currentLocation);
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

			}

			throw new ArgumentException($"{node}", nameof(node));
		}

		static IEnumerable GetItems(object node)
		{
			switch (node)
			{
				case Shell shell:
					return shell.Items;
				case ShellItem item:
					return item.Items;
				case ShellSection section:
					return section.Items;
				case ShellContent content:
					return null;
			}

			throw new ArgumentException($"{node}", nameof(node));
		}
	}

	/// <summary>
	/// This attempts to locate the intended route trying to be navigated to
	/// </summary>
	internal class RouteRequestBuilder
	{
		readonly List<string> _globalRouteMatches = new List<string>();
		readonly List<string> _matchedSegments = new List<string>();
		readonly List<string> _fullSegments = new List<string>();
		readonly string[] _allSegments = null;
		readonly static string _uriSeparator = "/";

		public Shell Shell { get; private set; }
		public ShellItem Item { get; private set; }
		public ShellSection Section { get; private set; }
		public ShellContent Content { get; private set; }

		public RouteRequestBuilder(string shellSegment, string userSegment, object node, string[] allSegments)
		{
			_allSegments = allSegments;
			if (node != null)
				AddMatch(shellSegment, userSegment, node);
			else
				AddGlobalRoute(userSegment, shellSegment);
		}
		public RouteRequestBuilder(RouteRequestBuilder builder)
		{
			_allSegments = builder._allSegments;
			_matchedSegments.AddRange(builder._matchedSegments);
			_fullSegments.AddRange(builder._fullSegments);
			_globalRouteMatches.AddRange(builder._globalRouteMatches);
			Shell = builder.Shell;
			Item = builder.Item;
			Section = builder.Section;
			Content = builder.Content;
		}

		public void AddGlobalRoute(string routeName, string segment)
		{
			_globalRouteMatches.Add(routeName);
			_fullSegments.Add(segment);
			_matchedSegments.Add(segment);
		}

		public void AddMatch(string shellSegment, string userSegment, object node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			switch (node)
			{
				case Shell shell:
					Shell = shell;
					break;
				case ShellItem item:
					Item = item;
					break;
				case ShellSection section:
					Section = section;

					if (Item == null)
					{
						Item = Section.Parent as ShellItem;
						_fullSegments.Add(Item.Route);
					}

					break;
				case ShellContent content:
					Content = content;
					if (Section == null)
					{
						Section = Content.Parent as ShellSection;
						_fullSegments.Add(Section.Route);
					}

					if (Item == null)
					{
						Item = Section.Parent as ShellItem;
						_fullSegments.Insert(0, Item.Route);
					}

					break;

			}

			// if shellSegment == userSegment it means the implicit route is part of the request
			if (!Routing.IsImplicit(shellSegment) || shellSegment == userSegment)
				_matchedSegments.Add(shellSegment);

			_fullSegments.Add(shellSegment);
		}

		public string NextSegment
		{
			get
			{
				var nextMatch = _matchedSegments.Count;
				if (nextMatch >= _allSegments.Length)
					return null;

				return _allSegments[nextMatch];
			}
		}

		string MakeUriString(List<string> segments)
		{
			if(segments[0].StartsWith("/") || segments[0].StartsWith("\\"))
				return $"{String.Join(_uriSeparator, segments)}";

			return $"//{String.Join(_uriSeparator, segments)}";
		}

		public string PathNoImplicit => MakeUriString(_matchedSegments);
		public string PathFull => MakeUriString(_fullSegments);

		public bool IsFullMatch => _matchedSegments.Count == _allSegments.Length;
		public List<string> GlobalRouteMatches => _globalRouteMatches;
		public int SegmentsMatched => _matchedSegments.Count;

	}



	[DebuggerDisplay("RequestDefinition = {Request}, StackRequest = {StackRequest}")]
	public class NavigationRequest
	{
		public enum WhatToDoWithTheStack
		{
			ReplaceIt,
			PushToIt
		}

		public NavigationRequest(RequestDefinition definition, WhatToDoWithTheStack stackRequest, string query, string fragment)
		{
			StackRequest = stackRequest;
			Query = query;
			Fragment = fragment;
			Request = definition;
		}

		public WhatToDoWithTheStack StackRequest { get; }
		public string Query { get; }
		public string Fragment { get; }
		public RequestDefinition Request { get; }
	}


	[DebuggerDisplay("Full = {FullUri}, Short = {ShortUri}")]
	public class RequestDefinition
	{
		public RequestDefinition(Uri fullUri, Uri shortUri, ShellItem item, ShellSection section, ShellContent content, List<string> globalRoutes)
		{
			FullUri = fullUri;
			ShortUri = shortUri;
			Item = item;
			Section = section;
			Content = content;
			GlobalRoutes = globalRoutes;
		}

		public RequestDefinition(string fullUri, string shortUri, ShellItem item, ShellSection section, ShellContent content, List<string> globalRoutes) :
			this(new Uri(fullUri, UriKind.Absolute), new Uri(shortUri, UriKind.Absolute), item, section, content, globalRoutes)
		{
		}

		public Uri FullUri { get; }
		public Uri ShortUri { get; }
		public ShellItem Item { get; }
		public ShellSection Section { get; }
		public ShellContent Content { get; }
		public List<string> GlobalRoutes { get; }
	}


}
