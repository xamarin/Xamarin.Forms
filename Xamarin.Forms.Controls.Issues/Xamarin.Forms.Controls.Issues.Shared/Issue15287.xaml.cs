using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 15287, "[Bug] [iOS] Changing CollectionView.ItemSizingStrategy that contains a CarouselView causes the CarouselView to size its cells incorrectly.", PlatformAffected.iOS)]
	public partial class Issue15287 : ContentPage
	{
		public Issue15287()
		{
#if APP
			InitializeComponent();
#endif
			BindingContext = new ViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
#if APP
			MainCollection.ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem;
#endif
		}

		public class ViewModel
		{
			public List<Item> Items { get; private set; }

			public ViewModel()
			{
				Items = Enumerable.Range(0, 30).Select(x => new Item()).ToList();
			}
		}

		public class Item
		{
			public IList<string> Images { get; } = new[]
			{
				"cover1.jpg",
				"oasis.jpg",
				"photo.jpg",
				"Vegetables.jpg",
				"Fruits.jpg",
				"FlowerBuds.jpg",
				"Legumes.jpg"
			};
		}
	}
}