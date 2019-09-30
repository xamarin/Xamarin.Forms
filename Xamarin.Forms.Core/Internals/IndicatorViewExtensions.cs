using System.ComponentModel;
using static Xamarin.Forms.IndicatorView;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class IndicatorViewExtensions
	{
		public static void SetItemsSourceBy(this IndicatorView indicatorView, CarouselView carouselView)
		{
			indicatorView.SetBinding(PositionProperty, new Binding
			{
				Path = nameof(CarouselView.Position),
				Source = carouselView
			});
			indicatorView.SetBinding(ItemsSourceProperty, new Binding
			{
				Path = nameof(ItemsView.ItemsSource),
				Source = carouselView
			});
		}
	}
}
