using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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

			if(segments[1] != shell.Route)
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
			List<RouteRequestBuilder> fullMatches = new List<RouteRequestBuilder>();

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
							match.AddMatch(key, key);
							keyLookup.Remove(key);
							i = 0;
						}
					}
				}

				if (match.IsFullMatch)
				{
					fullMatches.Add(match);
				}
			}

			if (fullMatches.Count == 0)
				throw new ArgumentException($"unable to figure out route for: {uri}", nameof(uri));
			else if (fullMatches.Count > 1)
			{
				string[] matches = new string[fullMatches.Count];
				int i = 0;
				foreach(var match in fullMatches)
				{
					matches[i] = match.PathFull;
					i++;
				}

				string matchesFound = String.Join(",", matches);
				throw new ArgumentException($"Ambiguous routes matched for: {uri} matches found: {matchesFound}", nameof(uri));

			}

			var theWinningRoute = fullMatches[0];
			RequestDefinition definition = 
				new RequestDefinition(
					ConvertToStandardFormat(shell, new Uri(theWinningRoute.PathFull, UriKind.RelativeOrAbsolute)),
					ConvertToStandardFormat(shell, new Uri(theWinningRoute.PathNoImplicit, UriKind.RelativeOrAbsolute))
					, null, null, null);

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
			SearchPath(shell, null, segments, possibleRoutePaths);

			return possibleRoutePaths;
		}

		static void SearchPath(object node, RouteRequestBuilder currentSearchPath, string[] segments, List<RouteRequestBuilder> possibleRoutePaths)
		{
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

			if (shellSegment == userSegment || Routing.IsImplicit(shellSegment))
			{
				RouteRequestBuilder builder = null;
				if (currentSearchPath == null)
					builder = new RouteRequestBuilder(shellSegment, userSegment, segments);
				else
				{
					builder = new RouteRequestBuilder(currentSearchPath);
					builder.AddMatch(shellSegment, userSegment);
				}

				if (!Routing.IsImplicit(shellSegment) || shellSegment == userSegment)
					possibleRoutePaths.Add(builder);

				var items = GetItems(node);
				if (items == null)
					return;

				foreach (var nextNode in GetItems(node))
				{
					SearchPath(nextNode, builder, segments, possibleRoutePaths);
				}
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
		List<string> _matchedPaths = new List<string>();
		List<string> _fullPaths = new List<string>();
		string[] _allSegments = null;
		readonly static string _uriSeparator = "/";

		public RouteRequestBuilder(string shellSegment, string userSegment, string[] allSegments)
		{
			_allSegments = allSegments;
			AddMatch(shellSegment, userSegment);
		}
		public RouteRequestBuilder(RouteRequestBuilder builder)
		{
			_allSegments = builder._allSegments;
			_matchedPaths.AddRange(builder._matchedPaths);
			_fullPaths.AddRange(builder._fullPaths);
		}

		public void AddMatch(string shellSegment, string userSegment)
		{
			// if shellSegment == userSegment it means the implicit route is part of the request
			if (!Routing.IsImplicit(shellSegment) || shellSegment == userSegment)
				_matchedPaths.Add(shellSegment);

			_fullPaths.Add(shellSegment);
		}

		public string NextSegment
		{
			get
			{
				var nextMatch = _matchedPaths.Count;
				if (nextMatch >= _allSegments.Length)
					return null;

				return _allSegments[nextMatch];
			}
		}

		public string PathNoImplicit => $"//{String.Join(_uriSeparator, _matchedPaths)}";
		public string PathFull => $"//{String.Join(_uriSeparator, _fullPaths)}";

		public bool IsFullMatch => _matchedPaths.Count == _allSegments.Length;
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
