using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xamarin.Forms
{

	public class ShellUriHandler
	{
		static readonly char[] _pathSeparator = { '/', '\\' };

		internal static List<RouteRequestBuilder> GenerateRoutePaths(Shell shell, Uri request)
		{
			List<RouteRequestBuilder> possibleRoutePaths = new List<RouteRequestBuilder>();

			string pathAndQuery = null;
			if (request.IsAbsoluteUri)
				pathAndQuery = request.PathAndQuery;
			else
				pathAndQuery = request.OriginalString;

			var segments = pathAndQuery.Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries);

			foreach(var item in shell.Items)
				SearchPath(item, null, segments, possibleRoutePaths);

			return possibleRoutePaths;
		}

		static void SearchPath(object node, RouteRequestBuilder currentSearchPath, string[] segments, List<RouteRequestBuilder> possibleRoutePaths)
		{
			string route = GetRoute(node);
			string segmentMatch = null;

			if(currentSearchPath == null)
			{
				segmentMatch = segments[0];
			}
			else
			{
				segmentMatch = currentSearchPath.NextSegment;
			}

			if (segmentMatch == null)
				return;

			if (route == segmentMatch || Routing.IsImplicit(route))
			{
				RouteRequestBuilder builder = null;
				if(currentSearchPath == null)
					builder = new RouteRequestBuilder(route, segments);
				else
				{
					builder = new RouteRequestBuilder(currentSearchPath);
					builder.AddMatch(route);
				}

				if (!Routing.IsImplicit(route))
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
					return null;
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

		public RouteRequestBuilder(string startingPath, string[] allSegments)
		{
			_allSegments = allSegments;
			AddMatch(startingPath);
		}
		public RouteRequestBuilder(RouteRequestBuilder builder)
		{
			_allSegments = builder._allSegments;
			_matchedPaths.AddRange(builder._matchedPaths);
			_fullPaths.AddRange(builder._fullPaths);
		}

		public void AddMatch(string path)
		{
			if(!Routing.IsImplicit(path))
				_matchedPaths.Add(path);

			_fullPaths.Add(path);
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
