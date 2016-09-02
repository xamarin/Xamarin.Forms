using System;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class PageWrapper : NSObject
	{
		public PageWrapper(Page page)
		{
			Page = page;
			Identifier = Guid.NewGuid().ToString();
		}

		public string Identifier { get; set; }
		public Page Page { get; private set; }
	}
}
