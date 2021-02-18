using Xamarin.Platform.Layouts;

namespace Maui.Controls.Sample.Controls
{
	public class HorizontalStackLayout : StackLayout
	{
		protected override ILayoutManager CreateLayoutManager() => new HorizontalStackLayoutManager(this);
	}
}
