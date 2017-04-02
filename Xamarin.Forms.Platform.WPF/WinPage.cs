using WPage=System.Windows.Controls.Page;

namespace Xamarin.Forms.Platform.WPF
{
	public class WinPage : WPage
	{
	    public WinPage()
	    {
	        var grid=new Grid();
			grid.RowDefinitions.Add(new RowDefinition()
			{
			    Height = 50
			});
			grid.RowDefinitions.Add(new RowDefinition()
			{
			    Height = GridLength.Star
			});

	        var boxView = new BoxView()
	        {
	            BackgroundColor = Color.Green,

	        };
			Grid.SetRow(boxView,1);
			grid.Children.Add(boxView);

	    }
	}
}
