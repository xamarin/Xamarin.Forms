namespace Xamarin.Forms.Controls
{
	public class RootContentPage : ContentPage
	{
		public RootContentPage(string hierarchy)
		{
			AutomationId = hierarchy + "PageId";
			Content = new SwapHierachyStackLayout(hierarchy);
		}
	}
}