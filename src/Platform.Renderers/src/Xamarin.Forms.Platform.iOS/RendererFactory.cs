using System;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.iOS
{
	public static class RendererFactory
	{
		[Obsolete("GetRenderer is obsolete as of version 2.0.1. Please use Platform.CreateRenderer instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IVisualElementRenderer GetRenderer(VisualElement view)
		{
			return Platform.CreateRenderer(view);
		}
	}
}