using System;
using Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.GroupingGalleries;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.ScrollToGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScrollToGroup : ContentPage
	{
		public ScrollToGroup()
		{
			InitializeComponent();
			CollectionView.ItemsSource = new SuperTeams();

			ScrollTo.Clicked += ScrollToClicked;
		}

		void ScrollToClicked(object sender, EventArgs e)
		{
			var groupIndex = int.Parse(GroupIndex.Text);
			var itemIndex = int.Parse(ItemIndex.Text);

			CollectionView.ScrollTo(itemIndex, groupIndex);
		}
	}
}