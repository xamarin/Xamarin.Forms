using System.Collections.Generic;
using Windows.UI.Xaml;

namespace Xamarin.Forms.Platform.UWP
{
	internal static class ViewExtensions
	{
		public static IEnumerable<Page> GetParentPages(this Page target)
		{
			var result = new List<Page>();
			var parent = target.Parent as Page;
			while (!Application.IsApplicationOrNull(parent))
			{
				result.Add(parent);
				parent = parent.Parent as Page;
			}

			return result;
		}

        public static FrameworkElement ToWindows(this Xamarin.Forms.View view, Rectangle size)
        {
            //var vRenderer = RendererFactory.GetRenderer (view);

            if (Platform.GetRenderer(view) == null)
                Platform.SetRenderer(view, Platform.CreateRenderer(view));

            var vRenderer = Platform.GetRenderer(view);

            view.Layout(new Rectangle(0, 0, size.Width, size.Height));

            //vRenderer.ContainerElement.Arrange(new Windows.Foundation.Rect(0, 0, size.Width, size.Height));

            return vRenderer.ContainerElement;
        }
	}
}