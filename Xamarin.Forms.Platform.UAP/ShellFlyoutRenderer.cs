using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellFlyoutRenderer : StackLayout, IFlyoutBehaviorObserver
	{
		private ShellRenderer _renderer;

		internal ShellFlyoutRenderer(ShellRenderer renderer)
		{
			_renderer = renderer;
			_renderer.IsPaneOpen = true;
			_renderer.PaneBackground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Gray);
		}

		public void OnFlyoutBehaviorChanged(FlyoutBehavior behavior)
		{
			//behavior == FlyoutBehavior
		}
	}
}
