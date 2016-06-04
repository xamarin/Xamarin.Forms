using System;
using System.Collections.Generic;

namespace Xamarin.Forms
{
	public class PoppedToRootEventArgs : NavigationEventArgs
	{
		public PoppedToRootEventArgs(Page page) : base(page)
		{
		}

		public List<Page> PoppedPages { get; set; }
	}
}