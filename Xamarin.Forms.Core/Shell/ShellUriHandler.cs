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

		public NavigationRequest GetNavigationRequest(Shell shell, Uri uri)
		{
			// figure out the intent of the Uri
			NavigationRequest.WhatToDoWithTheStack whatDoIDo = NavigationRequest.WhatToDoWithTheStack.PushToIt;
			if (uri.IsAbsoluteUri)
				whatDoIDo = NavigationRequest.WhatToDoWithTheStack.ReplaceIt;
			else if (uri.OriginalString.StartsWith("//"))
				whatDoIDo = NavigationRequest.WhatToDoWithTheStack.ReplaceIt;
			else
				whatDoIDo = NavigationRequest.WhatToDoWithTheStack.PushToIt;

			Uri request = ConvertToStandardFormat(shell, uri);

			var possibleRouteMatches = GenerateRoutePaths(shell, request);
			var routeKeys = Routing.GetRouteKeys();
			List<RouteRequestBuilder> bestMatches = new List<RouteRequestBuilder>();

			foreach (var match in possibleRouteMatches)
			{
				if (!match.IsFullMatch)
				{
					List<string> keyLookup = new List<string>(routeKeys);

					for (var i = 0; i < keyLookup.Count; i++)
					{
						var key = keyLookup[i];

						if (match.NextSegment == key)
						{
							match.AddMatch(key, key, null);
							keyLookup.Remove(key);
							i = 0;
						}
					}
				}

				if (match.IsFullMatch)
					bestMatches.Add(match);
			}

			int matchCount = 0;

			if (bestMatches.Count == 0)
			{
				foreach (var match in possibleRouteMatches)
				{
					if (match.SegmentsMatched > matchCount)
					{
						bestMatches.Clear();
						bestMatches.Add(match);
						matchCount = match.SegmentsMatched;
					}
					else if (match.SegmentsMatched == matchCount)
						bestMatches.Add(match);
				}
			}


			if (bestMatches.Count == 0)
				throw new ArgumentException($"unable to figure out route for: {uri}", nameof(uri));
			else if (bestMatches.Count > 1)
			{
				string[] matches = new string[bestMatches.Count];
				int i = 0;
				foreach (var match in bestMatches)
				{
					matches[i] = match.PathFull;
					i++;
				}

				string matchesFound = String.Join(",", matches);
				throw new ArgumentException($"Ambiguous routes matched for: {uri} matches found: {matchesFound}", nameof(uri));

			}

			var theWinningRoute = bestMatches[0];
			RequestDefinition definition =
				new RequestDefinition(
					ConvertToStandardFormat(shell, new Uri(theWinningRoute.PathFull, UriKind.RelativeOrAbsolute)),
					ConvertToStandardFormat(shell, new Uri(theWinningRoute.PathNoImplicit, UriKind.RelativeOrAbsolute))
					, theWinningRoute.Item, theWinningRoute.Section, theWinningRoute.Content);

			NavigationRequest navigationRequest = new NavigationRequest(definition, whatDoIDo, request.Query, request.Fragment);

			return navigationRequest;
		}

		internal static List<RouteRequestBuilder> GenerateRoutePaths(Shell shell, Uri request)
		{
			List<RouteRequestBuilder> possibleRoutePaths = new List<RouteRequestBuilder>();

			string pathAndQuery = null;
			if (request.IsAbsoluteUri)
				pathAndQuery = request.PathAndQuery;
			else
				pathAndQuery = request.OriginalString;

			var segments = pathAndQuery.Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries);
			if (segments[0] == shell.Route)
			{
				segments = segments.Skip(1).ToArray();
				SearchPath(shell, null, segments, possibleRoutePaths, 1);
			}
			else
			{
				SearchPath(shell, null, segments, possibleRoutePaths, 0);
			}

			return possibleRoutePaths;
		}

		static void SearchPath(object node, RouteRequestBuilder currentSearchPath, string[] segments, List<RouteRequestBuilder> possibleRoutePaths, int depthToStart, int myDepth = 0)
		{
			IEnumerable items = null;
			if (depthToStart > myDepth)
			{
				items = GetItems(node);
				if (items == null)
					return;

				foreach (var nextNode in GetItems(node))
				{
					SearchPath(nextNode, null, segments, possibleRoutePaths, depthToStart, ++myDepth);
				}
				return;
			}

			string shellSegment = GetRoute(node);
			string userSegment = null;

			if (currentSearchPath == null)
			{
				userSegment = segments[0];
			}
			else
			{
				userSegment = currentSearchPath.NextSegment;
			}

			if (userSegment == null)
				return;

			RouteRequestBuilder builder = null;
			if (shellSegment == userSegment || Routing.IsImplicit(shellSegment))
			{
				if (currentSearchPath == null)
					builder = new RouteRequestBuilder(shellSegment, userSegment, node, segments);
				else
				{
					builder = new RouteRequestBuilder(currentSearchPath);
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
				SearchPath(nextNode, builder, segments, possibleRoutePaths, depthToStart, ++myDepth);
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
			AddMatch(shellSegment, userSegment, node);
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

		public void AddMatch(string shellSegment, string userSegment, object node)
		{
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
						_fullSegments.Add(Item.Route);
					}

					break;

			}

			if (node == null)
				_globalRouteMatches.Add(shellSegment);

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

		public string PathNoImplicit => $"//{String.Join(_uriSeparator, _matchedSegments)}";
		public string PathFull => $"//{String.Join(_uriSeparator, _fullSegments)}";
		public bool IsFullMatch => _matchedSegments.Count == _allSegments.Length;
		public List<string> GlobalRouteMatches => _globalRouteMatches;
		public int SegmentsMatched => _matchedSegments.Count;

	}



	[DebuggerDisplay("RequestDefinition = {Request}, StackRequest = {StackRequest}")]
	internal class NavigationRequest
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
	internal class RequestDefinition
	{
		public RequestDefinition(Uri fullUri, Uri shortUri, ShellItem item, ShellSection section, ShellContent content)
		{
			FullUri = fullUri;
			ShortUri = shortUri;
			Item = item;
			Section = section;
			Content = content;
		}

		public RequestDefinition(string fullUri, string shortUri, ShellItem item, ShellSection section, ShellContent content) :
			this(new Uri(fullUri, UriKind.Absolute), new Uri(shortUri, UriKind.Absolute), item, section, content)
		{
		}

		public Uri FullUri { get; }
		public Uri ShortUri { get; }
		public ShellItem Item { get; }
		public ShellSection Section { get; }
		public ShellContent Content { get; }
	}


}
