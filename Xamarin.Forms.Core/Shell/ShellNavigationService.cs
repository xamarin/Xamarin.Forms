using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public class ShellNavigationService
	{
		internal class ShellNavigationImpl : IUriProjectionParser
		{
			public Task<ShellRouteState> ParseAsync(Shell currentState, Uri uri)
			{
				var navigationRequest = ShellUriHandler.GetNavigationRequest(currentState, uri, false);
				return Task.FromResult(navigationRequest);
			}
		}
	}


	// possible any interface
	public enum PresentationHint
	{
		Page,
		Modal,
		Dialog
	}

	interface ITransitionPlan
	{
		ITransition TransitionTo { get; }
		ITransition TransitionFrom { get; }
	}

	interface ITransition
	{

	}

	public class NavigationProjection
	{
		public ShellRouteState CurrentState { get; }
		public ShellRouteState FutureState { get; set; }
	}

	public class PathPart
	{
		public PathPart(BaseShellItem baseShellItem, Dictionary<string, string> navigationParameters)
		{
			ShellItem = baseShellItem;
			Path = ShellItem.Route;
			NavigationParameters = navigationParameters;
		}

		public string Path { get; }
		
		public Dictionary<string, string> NavigationParameters { get; }

		public BaseShellItem ShellItem { get; }

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
				builder.Append(path.ShellItem.Route);
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
		internal ShellRouteState(): this(new RoutePath(new List<PathPart>(), new Dictionary<string, string>()))
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

	public interface IUriProjectionParser
	{
		// based on the current state and this uri what should the new state look like?
		// the uri could completely demolish the current state and just return a new setup completely
		Task<ShellRouteState> ParseAsync(Shell currentState, Uri uri);
	}

	public interface IShellProjectionHandler
	{
		// this is used to modify the shell structure based on how the shell is changing
		// this allows a hook where you would hide or show different shell items
		void Project(Shell shell, Shell projection);
	}

	public interface IShellNavigationRequest
	{
		// this will return the state change. If you want to cancel navigation then just return null or current state
		ShellRouteState NavigatingTo(ShellRouteState currentState, ShellRouteState futureState);
	}

	public interface IShellContentCreator
	{
		ContentPage Create(ShellContent content, Shell routeState);
	}

	public interface IShellApplyParameters
	{
		// this is where we will apply query parameters to the shell content 
		// this may be called multiple times. For example when the bindingcontext changes it will be called again
		void ApplyParameters(BaseShellItem shellItem, Shell routeState);
	}

	public interface IShellPartAppearing
	{
		// this will get called for each piece that appears shellitem, shellsection, shellcontent
		void Appearing(BaseShellItem shellItem, Shell routeState);
	}

	public interface IShellPartAppeared
	{
		// this will get called for each piece that appeared shellitem, shellsection, shellcontent
		void Appeared(BaseShellItem shellItem, Shell routeState);
	}

	public interface IShellPartDisappeared
	{
		// this will get called for each piece that disappeared shellitem, shellsection, shellcontent
		void Disappeared(BaseShellItem shellItem, Shell routeState);
	}

	public class FormsShellNavigationService
	//Implements all the interfaces all virtual so if users want to just use ours for a baseline that can then override what they want to change
	{

	}
}
