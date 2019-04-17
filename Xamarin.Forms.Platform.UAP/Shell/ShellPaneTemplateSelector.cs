using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Xamarin.Forms.Platform.UWP
{
	internal class ShellPaneTemplateSelector : Windows.UI.Xaml.Controls.DataTemplateSelector
	{
		static Windows.UI.Xaml.DataTemplate ShellContentTemplate { get; }
		static Windows.UI.Xaml.DataTemplate MenuItemTemplate { get; }
		static Windows.UI.Xaml.DataTemplate SeperatorTemplate { get; }

		static ShellPaneTemplateSelector()
		{
			var template = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:xf=""using:Xamarin.Forms.Platform.UWP""><xf:ShellNavigationViewItemRenderer Content=""{Binding Title}"" ImageSource=""{Binding FlyoutIcon}"" /></DataTemplate>";
			ShellContentTemplate = (Windows.UI.Xaml.DataTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(template);
			template = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:xf=""using:Xamarin.Forms.Platform.UWP""><xf:ShellNavigationViewItemRenderer Content=""{Binding Text}"" ImageSource=""{Binding Icon}"" /></DataTemplate>";
			MenuItemTemplate = (Windows.UI.Xaml.DataTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(template);
			template = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><NavigationViewItemSeparator /></DataTemplate>";
			SeperatorTemplate = (Windows.UI.Xaml.DataTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(template);
		}

		protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item)
		{
			if (item is ShellItem)
				return ShellContentTemplate;
			if(item is MenuItem)
				return MenuItemTemplate;
			if(item == null)
				return SeperatorTemplate;
			return base.SelectTemplateCore(item);
		}
	}
}
