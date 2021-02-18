using Xamarin.Platform.Layouts;

namespace Maui.Controls.Sample.Controls
{
	public class VerticalStackLayout : StackLayout
	{
		protected override ILayoutManager CreateLayoutManager() => new VerticalStackLayoutManager(this);
	}
}
