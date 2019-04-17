using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellNavigationViewItemRenderer : NavigationViewItem
	{
		void UpdateIcon()
		{
			if (ImageSource is FileImageSource fis)
				Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///" + fis.File) };
			else
				Icon = null;
		}
		
		public ImageSource ImageSource
		{
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register(nameof(ImageSource), typeof(ShellItem), typeof(ShellNavigationViewItemRenderer), new PropertyMetadata(null, OnImageSourcePropertyChanged));

		static void OnImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((ShellNavigationViewItemRenderer)d).UpdateIcon();
		}
	}
}
