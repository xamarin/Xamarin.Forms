using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Xamarin.Forms.Platform.UWP
{
	public class ToolbarItemRenderer : Windows.UI.Xaml.Controls.Button
	{
		public ToolbarItemRenderer()
		{
			Click += OnClick;
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			if (ToolbarItem is IMenuItemController controller)
				controller?.Activate();
		}

		public ToolbarItem ToolbarItem
		{
			get { return (ToolbarItem)GetValue(ToolbarItemProperty); }
			set { SetValue(ToolbarItemProperty, value); }
		}

		public static readonly DependencyProperty ToolbarItemProperty =
			DependencyProperty.Register("ToolbarItem", typeof(ToolbarItem), typeof(ToolbarItemRenderer), new PropertyMetadata(null));
	}
}
