using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellFlyoutRenderer : StackLayout, IFlyoutBehaviorObserver
	{
		private ShellRenderer _shellContext;
		private ShellFlyoutTemplatedContentRenderer _flyoutContent;

		internal ShellFlyoutRenderer(ShellRenderer renderer)
		{
			_shellContext = renderer;
			_shellContext.DisplayMode = SplitViewDisplayMode.Overlay;
			_shellContext.IsPaneOpen = Shell.FlyoutIsPresented;
			_shellContext.PaneClosing += ShellContext_PaneClosing;
			_shellContext.PaneClosed += ShellContext_PaneClosed;
			_shellContext.PaneOpened += ShellContext_PaneOpened;
			Shell.PropertyChanged += OnShellPropertyChanged;
		}

		private void ShellContext_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
		{
			if (Shell.FlyoutBehavior == FlyoutBehavior.Locked)
				args.Cancel = true;
		}

		private void ShellContext_PaneClosed(SplitView sender, object args)
		{
			Shell.SetValueFromRenderer(Shell.FlyoutIsPresentedProperty, false);
		}

		private void ShellContext_PaneOpened(SplitView sender, object args)
		{
			Shell.SetValueFromRenderer(Shell.FlyoutIsPresentedProperty, true);
		}

		Shell Shell => _shellContext.Element;

		internal void AttachFlyout(ShellRenderer shellRenderer, Windows.UI.Xaml.Controls.Frame frameLayout)
		{
			_flyoutContent = new ShellFlyoutTemplatedContentRenderer(_shellContext);
			((IShellController)shellRenderer.Element).AddFlyoutBehaviorObserver(this);
		}

		protected virtual void OnShellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Shell.FlyoutIsPresentedProperty.PropertyName)
			{
				_shellContext.IsPaneOpen = Shell.FlyoutIsPresented && Shell.FlyoutBehavior != FlyoutBehavior.Disabled;
			}
		}

		#region IFlyoutBehaviorObserver

		void IFlyoutBehaviorObserver.OnFlyoutBehaviorChanged(FlyoutBehavior behavior)
		{
			UpdatePaneDisplayMode(behavior);
		}

		#endregion IFlyoutBehaviorObserver

		protected virtual void UpdatePaneDisplayMode(FlyoutBehavior behavior)
		{
			switch (behavior)
			{
				case FlyoutBehavior.Disabled:
					_shellContext.DisplayMode = SplitViewDisplayMode.Overlay;
					_shellContext.IsPaneOpen = false;
					break;

				case FlyoutBehavior.Flyout:
					_shellContext.DisplayMode = SplitViewDisplayMode.Overlay;
					_shellContext.IsPaneOpen = Shell.FlyoutIsPresented;
					break;

				case FlyoutBehavior.Locked:
					_shellContext.IsPaneOpen = true;
					_shellContext.DisplayMode = SplitViewDisplayMode.Inline;
					break;
			}
		}
	}
}
