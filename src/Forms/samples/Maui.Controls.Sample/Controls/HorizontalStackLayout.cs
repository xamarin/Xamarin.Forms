using Xamarin.Platform.Layouts;

namespace Maui.Controls.Sample
{
	public class HorizontalStackLayout : StackLayout
	{
		protected override ILayoutManager CreateLayoutManager() => new HorizontalStackLayoutManager(this);
	}
}
