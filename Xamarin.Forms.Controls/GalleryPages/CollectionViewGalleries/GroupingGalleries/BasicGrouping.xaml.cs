using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.GroupingGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	[Preserve (AllMembers = true)]
	public partial class BasicGrouping : ContentPage
	{
		// TODO ezhart and a gallery that allows moving items and has grouping (groupable observable items sources)

		// TODO ezhart We need a test of grouping that also uses the itemsizingstrategy measurefirst to verify that works (check ios 10 especially)

		public BasicGrouping ()
		{
			InitializeComponent ();

			CollectionView.ItemsSource = new SuperTeams();
		}
	}
}