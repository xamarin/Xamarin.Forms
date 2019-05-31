using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.iOS;
using Xamarin.Forms.Controls;

[assembly: Dependency(typeof(PlatformSpecificCoreGalleryFactory))]

namespace Xamarin.Forms.ControlGallery.iOS
{
	public class PlatformSpecificCoreGalleryFactory : IPlatformSpecificCoreGalleryFactory
	{
		public string Title => "iOS Core Gallery";

		public IEnumerable<(Func<Page> Create, string Title)> GetPages()
		{
			yield return (() => new ContextActionDisplayTest(), "Context Action Display Test");
#if HAVE_OPENTK
			yield return (() => new AdvancedOpenGLGallery(), "Advanced OpenGL Gallery - Legacy");
#else
			return null;
#endif
		}
	}
}
