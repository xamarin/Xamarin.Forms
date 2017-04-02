using System;
using System.ComponentModel;
using WPage=System.Windows.Controls.Page;

namespace Xamarin.Forms.Platform.WPF
{
	public class WPFPage : WPage
	{
	    public WPFPage()
	    {
	  //      var grid=new Grid();
			//grid.RowDefinitions.Add(new RowDefinition()
			//{
			//    Height = 50
			//});
			//grid.RowDefinitions.Add(new RowDefinition()
			//{
			//    Height = GridLength.Star
			//});

	  //      var boxView = new BoxView()
	  //      {
	  //          BackgroundColor = Color.Green,

	  //      };
			//Grid.SetRow(boxView,1);
			//grid.Children.Add(boxView);
	        this.ShowsNavigationUI = false;
	    }

	    public event EventHandler<CancelEventArgs> BackKeyPress;
	}
}
