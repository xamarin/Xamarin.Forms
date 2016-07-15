using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImAVendor.Forms.PlatformConfiguration.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.iOS;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.PlatformConfiguration.iOS;

[assembly: ExportEffect(typeof(NavigationPageTranslucentEffect), "NavigationPageTranslucentEffect")]
namespace Xamarin.Forms.ControlGallery.iOS
{
	public class NavigationPageTranslucentEffect : PlatformEffect
	{
		UINavigationBar _navigationBar;

		protected override void OnAttached()
		{
			UpdateTranslucent();
		}

		protected override void OnDetached()
		{

		}

		protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(e);

			try
			{
				if (e.PropertyName == FakeVendorConfig.IsNavigationBarTranslucentProperty.PropertyName)
					UpdateTranslucent();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
			}
		}

		void UpdateTranslucent()
		{
			var navBar = GetNavigationBar();
			if (navBar == null)
				return;

			//if (!Forms.IsiOS7OrNewer)
			//{
			//	return;
			//}
			navBar.Translucent = ((NavigationPage)Element).On<IConfigIOS>().GetNavigationBarIsTranslucentVendor();
		}

		UINavigationBar GetNavigationBar()
		{
			if (_navigationBar == null)
			{
				foreach (var subView in Container.Subviews)
				{
					if (subView is UINavigationBar)
						_navigationBar = subView as UINavigationBar;
				}
			}
			return _navigationBar;
		}
	}
}
