using System;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Xamarin.Forms.Platform.UWP
{
	public class ImageSourceProviderImageSource : Windows.UI.Xaml.Media.Imaging.SurfaceImageSource, IRecreateImageSource
	{
		private const float _minimumDpi = 300;
		private readonly string fontFamily;
		private readonly float fontSize;
		private readonly string glyph;
		private readonly Windows.UI.Color iconColor;

		public Windows.UI.Xaml.Media.ImageSource InitialSource { get; private set; }

		public ImageSourceProviderImageSource(string fontFamily, float fontSize,string glyph, Windows.UI.Color iconColor) :base(0,0)
		{
			this.fontFamily = fontFamily;
			this.fontSize = fontSize;
			this.glyph = glyph;
			this.iconColor = iconColor;
			InitialSource = CreateImageSource();
		}

		public Windows.UI.Xaml.Media.ImageSource CreateImageSource()
		{
			var device = CanvasDevice.GetSharedDevice();
			var dpi = Math.Max(_minimumDpi, Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi);

			var textFormat = new CanvasTextFormat
			{
				FontFamily = fontFamily,
				FontSize = fontSize,
				HorizontalAlignment = CanvasHorizontalAlignment.Left,
				VerticalAlignment = CanvasVerticalAlignment.Center,
				Options = CanvasDrawTextOptions.Default
			};

			using (var layout = new CanvasTextLayout(device, glyph, textFormat, (float)fontSize, (float)fontSize))
			{
				var canvasWidth = (float)layout.LayoutBounds.Width + 2;
				var canvasHeight = (float)layout.LayoutBounds.Height + 2;

				var imageSource = new CanvasImageSource(device, canvasWidth, canvasHeight, dpi);
				using (var ds = imageSource.CreateDrawingSession(Windows.UI.Colors.Transparent))
				{
					// offset by 1 as we added a 1 inset
					ds.DrawTextLayout(layout, 1f, 1f, iconColor);
				}
				return imageSource;
			}
		}
	}
}
