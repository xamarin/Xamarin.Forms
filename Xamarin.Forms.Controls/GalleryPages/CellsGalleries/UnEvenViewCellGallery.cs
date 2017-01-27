using Xamarin.Forms.Maps;

namespace Xamarin.Forms.Controls
{
	public class UnevenViewCellGallery : ContentPage
	{
		public UnevenViewCellGallery()
		{
			Title = "UnevenViewCell Gallery - Legacy";

			Map map = MapGallery.MakeMap();
			map.HasScrollEnabled = false;

			Content = new TableView
			{
				RowHeight = 150,
				HasUnevenRows = true,
				Root = new TableRoot
				{
					new TableSection("Testing")
					{
						new ViewCell { View = map, Height = 250 },
						new ViewCell { View = new ProductCellView("1 day") },
						new ViewCell { View = new ProductCellView("2 days") },
						new ViewCell { View = new ProductCellView("3 days") },
						new ViewCell { View = new ProductCellView("4 days") },
						new ViewCell { View = new ProductCellView("5 days") }
					}
				}
			};
		}
	}
}