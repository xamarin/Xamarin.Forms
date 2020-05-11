using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.Platform.UAP.Extensions
{
	public static class CompositionExtensions
	{
		public static void SetShadow(this FrameworkElement element, DropShadow shadow, VisualElement visualElement)
		{
			if (element == null || shadow == null)
				return;

			var visualHost = ElementCompositionPreview.GetElementVisual(element);
			var compositor = visualHost.Compositor;

			var dropShadow = compositor.CreateDropShadow();

			dropShadow.Offset = new Vector3((float)shadow.Offset.X, (float)shadow.Offset.Y, 0);
			dropShadow.BlurRadius = (float)shadow.Radius;

			byte opacity = (byte)(shadow.Opacity * 255);
			var color = shadow.Color.ToWindowsColor();
			dropShadow.Color = Windows.UI.Color.FromArgb(opacity, color.R, color.G, color.B);

			var spriteVisual = compositor.CreateSpriteVisual();

			spriteVisual.Size = new Vector2((float)element.ActualWidth, (float)element.ActualHeight);
			spriteVisual.Shadow = dropShadow;

			// TODO: Refactor. By modifying the hierarchy used this is not necessary.
			var backgroundColor = visualElement.BackgroundColor;
			spriteVisual.Brush = compositor.CreateColorBrush(backgroundColor.ToWindowsColor());

			ElementCompositionPreview.SetElementChildVisual(element, spriteVisual);
		}
	}
}
