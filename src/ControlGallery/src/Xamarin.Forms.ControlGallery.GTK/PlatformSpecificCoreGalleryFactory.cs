﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.GTK;
using Xamarin.Forms.Controls;

[assembly: Dependency(typeof(PlatformSpecificCoreGalleryFactory))]

namespace Xamarin.Forms.ControlGallery.GTK
{
	public class PlatformSpecificCoreGalleryFactory : IPlatformSpecificCoreGalleryFactory
	{
		public string Title => "GTK# Core Gallery";

		public IEnumerable<(Func<Page> Create, string Title)> GetPages()
		{
#if HAVE_OPENTK
			yield return (() => new BasicOpenGLGallery(), "Basic OpenGL Gallery - Legacy");
			yield return (() => new AdvancedOpenGLGallery(), "Advanced OpenGL Gallery - Legacy");
#else
			return null;
#endif
		}
	}
}
