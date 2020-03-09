using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellFlyouItemRenderer : ContentControl
	{
		public ShellFlyouItemRenderer()
		{
			this.DataContextChanged += OnDataContextChanged;
		}

		void OnDataContextChanged(Windows.UI.Xaml.FrameworkElement sender, Windows.UI.Xaml.DataContextChangedEventArgs args)
		{
			if(Content is ViewToRendererConverter.WrapperControl oldControl)
			{				
				oldControl.CleanUp();
			}

			var bo = (BindableObject)DataContext;
			DataTemplate dataTemplate;
			if (bo is IMenuItemController)
			{
				dataTemplate = Shell.GetMenuItemTemplate(bo);
			}
			else
			{
				dataTemplate = Shell.GetItemTemplate(bo);
			}

			if(dataTemplate != null)
			{
				var content = (View)dataTemplate.CreateContent();
				content.BindingContext = bo;
				content.Parent = (bo as Element)?.FindParent<Shell>();
				Content = new ViewToRendererConverter.WrapperControl(content);
			}
		}
	}
}
