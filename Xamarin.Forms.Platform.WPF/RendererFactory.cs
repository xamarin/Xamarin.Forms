using System;

namespace Xamarin.Forms.Platform.WPF
{
	public static class RendererFactory
	{
		[Obsolete("Use Platform.CreateRenderer")]
		public static IVisualElementRenderer GetRenderer(VisualElement view)
		{
			return Platform.CreateRenderer(view);
		}
	}
}