using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.GroupingGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SomeEmptyGroups : ContentPage
	{
		public SomeEmptyGroups()
		{
			InitializeComponent();

			var teams = new List<Team>
			{
				new Team("Avengers", "New York City", new List<Member>
				{
					new Member("Thor"),
					new Member("Captain America")
				}),

				new Team("Thundercats", "Cats Lair", new List<Member>()),

				new Team("Avengers", "New York City", new List<Member>
				{
					new Member("Thor"),
					new Member("Captain America")
				}),
							   			
				new Team("Bionic Six", "Cypress Cove", new List<Member>()),

				new Team("Fantastic Four", "New York City", new List<Member>
				{
					new Member("The Thing"),
					new Member("The Human Torch"),
					new Member("The Invisible Woman"),
					new Member("Mr. Fantastic"),
				})
			};

			CollectionView.ItemsSource = teams;
		}
	}
}