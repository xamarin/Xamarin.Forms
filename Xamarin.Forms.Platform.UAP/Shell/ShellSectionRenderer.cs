using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Navigation;

namespace Xamarin.Forms.Platform.UWP
{
	internal class ShellSectionRenderer : Windows.UI.Xaml.Controls.Page
	{
		Windows.UI.Xaml.Controls.Frame _Frame;
		Page Page;
		ShellContent CurrentContent;
		ShellSection ShellSection;

		public ShellSectionRenderer()
		{
			var root = new Windows.UI.Xaml.Controls.Grid();
			root.RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
			root.RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
			_Frame = new Windows.UI.Xaml.Controls.Frame();
			root.Children.Add(_Frame);
			this.Content = root;
			this.SizeChanged += ShellSectionRenderer_SizeChanged;
		}

		void ShellSectionRenderer_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
		{
			Page.ContainerArea = new Rectangle(0, 0, e.NewSize.Width, e.NewSize.Height);
		}

		protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			var section = ShellSection = e.Parameter as ShellSection;
			NavigateToContent(section.CurrentItem);
			ShellSection.PropertyChanged += OnShellSectionPropertyChanged;
		}

		void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ShellSection.CurrentItemProperty.PropertyName)
			{
				NavigateToContent(ShellSection.CurrentItem);
			}
		}

		internal void NavigateToContent(ShellContent shellContent)
		{
			if(CurrentContent != null && Page != null)
				((IShellContentController)CurrentContent).RecyclePage(Page);
			CurrentContent = shellContent;
			 Page = ((IShellContentController)shellContent).GetOrCreateContent();
			_Frame.Navigate((ContentPage)Page);
		}

		protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			_Frame.Content = null;
			ShellSection.PropertyChanged -= OnShellSectionPropertyChanged;
			((IShellContentController)CurrentContent).RecyclePage(Page);
			CurrentContent = null;
			Page = null;
			base.OnNavigatedFrom(e);
		}
	}
}
