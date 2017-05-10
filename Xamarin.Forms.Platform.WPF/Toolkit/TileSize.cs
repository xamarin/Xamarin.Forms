using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Xamarin.Forms.Platform.WPF.Toolkit
{
    public enum TileSize
	{
	    Default,
	    Medium
	}

    public class HubTile : System.Windows.Controls.Grid
    {
        public string Title { get; set; }

        public BitmapImage Source { get; set; }
		
        public TileSize Size { get; set; }
    }
}
