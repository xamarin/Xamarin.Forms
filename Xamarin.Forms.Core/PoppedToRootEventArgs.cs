using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms
{
	public class PoppedToRootEventArgs : NavigationEventArgs
	{
		public PoppedToRootEventArgs(IEnumerable<Page> poppedPages) : base(poppedPages.Last())
		{
			if (poppedPages == null)
				throw new ArgumentNullException(nameof(poppedPages));

			PoppedPages = poppedPages;
		}

		public IEnumerable<Page> PoppedPages { get; private set; }
	}
}
