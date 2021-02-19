using Xamarin.Platform.Layouts;

namespace Maui.Controls.Sample
{
	public class VerticalStackLayout : StackLayout
	{
		protected override ILayoutManager CreateLayoutManager() => new VerticalStackLayoutManager(this);
	}
}
