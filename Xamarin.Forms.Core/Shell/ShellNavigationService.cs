using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Dependency(typeof(ShellNavigationService))]
namespace Xamarin.Forms
{
	public class ShellNavigationService :
		IShellUriParser,
		IShellContentCreator,
		IShellApplyParameters,
		IShellPartAppearing,
		IShellPartAppeared,
		IShellNavigationRequest
	{
		public ShellNavigationService()
		{
		}

		public Task AppearedAsync(ShellLifecycleArgs args)
		{
			return Task.Delay(0);
		}

		public Task AppearingAsync(ShellLifecycleArgs args)
		{
			return Task.Delay(0);
		}

		public void ApplyParameters(ShellLifecycleArgs args)
		{
			Shell.ApplyQueryAttributes(args.Element, args.PathPart?.NavigationParameters ?? args.RoutePath?.NavigationParameters, args.IsLast);
		}

		public Page Create(ShellContentCreateArgs args)
		{
			var shellContent = args.Content;
			var template = shellContent.ContentTemplate;
			var content = shellContent.Content;

			Page result = null;
			if (template == null)
			{
				if (content is Page page)
					result = page;
				else
					result = (Page)Routing.GetOrCreateContent(args.Content.Route);
			}
			else
			{
				result = (Page)template.CreateContent(content, shellContent);
			}

			return result;

		}
		public Task<ShellRouteState> NavigatingToAsync(ShellNavigationArgs args)
		{
			return Task.FromResult(args.FutureState);
		}

		public Task<ShellRouteState> ParseAsync(ShellUriParserArgs args)
		{
			var navigationRequest = ShellUriHandler.GetNavigationRequest(args.Shell, args.Uri, false);
			return Task.FromResult(navigationRequest);
		}
	}

	public class PathPart
	{
		public PathPart(BaseShellItem baseShellItem, Dictionary<string, string> navigationParameters)
		{
			ShellPart = baseShellItem;
			Path = ShellPart.Route;
			NavigationParameters = navigationParameters;
		}

		public string Path { get; }

		public Dictionary<string, string> NavigationParameters { get; }

		public BaseShellItem ShellPart { get; }

		//// This describes how you will transition to and away from this Path Part
		//ITransitionPlan Transition { get; }

		//// how am I presented? Modally? as a page?
		//PresentationHint Presentation { get; }
	}

	// Do better at immutable stuff
	public class RoutePath
	{
		public RoutePath(IList<PathPart> pathParts, Dictionary<string, string> navigationParameters)
		{
			PathParts = new ReadOnlyCollection<PathPart>(pathParts);
			NavigationParameters = navigationParameters;

			StringBuilder builder = new StringBuilder();

			for (var i = 0; i < pathParts.Count; i++)
			{
				var path = pathParts[i];
				builder.Append(path.ShellPart.Route);
				builder.Append("/");
			}

			FullUriWithImplicit = ShellUriHandler.ConvertToStandardFormat(new Uri(builder.ToString(), UriKind.Relative));
		}

		internal Uri FullUriWithImplicit { get; }
		public Dictionary<string, string> NavigationParameters { get; }
		public IReadOnlyList<PathPart> PathParts { get; }
	}

	public class ShellRouteState
	{
		internal ShellRouteState() : this(new RoutePath(new List<PathPart>(), new Dictionary<string, string>()))
		{

		}

		internal ShellRouteState(Shell shell)
		{
			List<PathPart> pathParts = new List<PathPart>();
			pathParts.Add(new PathPart(shell.CurrentItem, null));
			pathParts.Add(new PathPart(shell.CurrentItem.CurrentItem, null));
			pathParts.Add(new PathPart(shell.CurrentItem.CurrentItem.CurrentItem, null));
			CurrentRoute = new RoutePath(pathParts, null);
			Routes = new[] { CurrentRoute };
		}


		public ShellRouteState(RoutePath routePath)
		{
			CurrentRoute = routePath;
			Routes = new[] { CurrentRoute };
		}

		private ShellRouteState(RoutePath[] routePaths, RoutePath currentRoute)
		{
			Routes = routePaths;
			CurrentRoute = currentRoute;
		}

		public RoutePath[] Routes { get; }
		public RoutePath CurrentRoute { get; }

		public ShellRouteState Add(PathPart pathPart)
		{
			List<PathPart> newPathPArts = new List<PathPart>(CurrentRoute.PathParts);
			newPathPArts.Add(pathPart);

			RoutePath[] newRoutes = new RoutePath[Routes.Length];
			Array.Copy(Routes, newRoutes, Routes.Length);

			RoutePath newCurrentRoute = null;
			for (var i = 0; i < newRoutes.Length; i++)
			{
				var route = newRoutes[i];
				if (route == CurrentRoute)
				{
					newCurrentRoute = new RoutePath(newPathPArts, route.NavigationParameters);
					newRoutes[i] = newCurrentRoute;
				}
			}

			return new ShellRouteState(newRoutes, newCurrentRoute);
		}
		public ShellRouteState Add(IList<PathPart> pathParts)
		{
			List<PathPart> newPathPArts = new List<PathPart>(CurrentRoute.PathParts);
			newPathPArts.AddRange(pathParts);

			RoutePath[] newRoutes = new RoutePath[Routes.Length];
			Array.Copy(Routes, newRoutes, Routes.Length);

			RoutePath newCurrentRoute = null;
			for (var i = 0; i < newRoutes.Length; i++)
			{
				var route = newRoutes[i];
				if (route == CurrentRoute)
				{
					newCurrentRoute = new RoutePath(newPathPArts, route.NavigationParameters);
					newRoutes[i] = newCurrentRoute;
				}
			}

			return new ShellRouteState(newRoutes, newCurrentRoute);
		}
	}

	public interface IShellUriParser
	{
		// based on the current state and this uri what should the new state look like?
		// the uri could completely demolish the current state and just return a new setup completely
		Task<ShellRouteState> ParseAsync(ShellUriParserArgs args);
	}

	public class ShellUriParserArgs : EventArgs
	{
		public ShellUriParserArgs(Shell shell, Uri uri)
		{
			Shell = shell;
			Uri = uri;
		}

		public Shell Shell { get; }
		public Uri Uri { get; }
	}

	public interface IShellNavigationRequest
	{
		// this will return the state change. If you want to cancel navigation then just return null or current state
		Task<ShellRouteState> NavigatingToAsync(ShellNavigationArgs args);
	}

	public interface IShellContentCreator
	{
		Page Create(ShellContentCreateArgs content);
	}


	public interface IShellApplyParameters
	{
		// this is where we will apply query parameters to the shell content 
		// this may be called multiple times. For example when the bindingcontext changes it will be called again
		void ApplyParameters(ShellLifecycleArgs args);
	}

	public interface IShellPartAppearing
	{
		// this will get called for each piece that appears shellitem, shellsection, shellcontent
		Task AppearingAsync(ShellLifecycleArgs args);
	}

	public interface IShellPartAppeared
	{
		// this will get called for each piece that appeared shellitem, shellsection, shellcontent
		Task AppearedAsync(ShellLifecycleArgs args);
	}

	public interface IShellPartDisappeared
	{
		// this will get called for each piece that disappeared shellitem, shellsection, shellcontent
		Task DisappearedAsync(ShellLifecycleArgs args);
	}

	public class ShellNavigationArgs : EventArgs
	{
		public ShellNavigationArgs(Shell shell, ShellRouteState futureState)
		{
			Shell = shell;
			FutureState = futureState;
		}

		public Shell Shell { get; }
		public ShellRouteState FutureState { get; }
	}

	public class ShellContentCreateArgs : EventArgs
	{
		public ShellContentCreateArgs(ShellContent content)
		{
			Content = content;
		}

		public ShellContent Content { get; }
	}
	public class ShellLifecycleArgs : EventArgs
	{
		public ShellLifecycleArgs(Element element, PathPart pathPart, RoutePath routePath)
		{
			Element = element;
			PathPart = pathPart;
			RoutePath = routePath;
		}

		public Element Element { get; }
		public PathPart PathPart { get; }
		public RoutePath RoutePath { get; }

		public bool IsLast
		{
			get
			{
				if (RoutePath.PathParts[RoutePath.PathParts.Count - 1] == PathPart)
					return true;

				return false;
			}
		}
	}




	//// possible any interface
	//public enum PresentationHint
	//{
	//	Page,
	//	Modal,
	//	Dialog
	//}

	//interface ITransitionPlan
	//{
	//	ITransition TransitionTo { get; }
	//	ITransition TransitionFrom { get; }
	//}

	//interface ITransition
	//{

	//}

}
