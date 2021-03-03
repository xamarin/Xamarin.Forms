using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Maui.Controls.Maps
{
	public class PinClickedEventArgs : EventArgs
	{
		public bool HideInfoWindow { get; set; }

		public PinClickedEventArgs()
		{
			HideInfoWindow = false;
		}
	}
}
