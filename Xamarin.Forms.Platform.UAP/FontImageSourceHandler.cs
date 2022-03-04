using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using WFontIconSource = Microsoft.UI.Xaml.Controls.FontIconSource;

namespace Xamarin.Forms.Platform.UWP
{
	public sealed class FontImageSourceHandler : IImageSourceHandler, IIconElementHandler
	{
		public Task<Windows.UI.Xaml.Media.ImageSource> LoadImageAsync(ImageSource imagesource,
			CancellationToken cancelationToken = default(CancellationToken))
		{
			if (!(imagesource is FontImageSource fontsource))
				return null;

			
			var fontFamily = GetFontSource(fontsource);
			var fontSize = (float)fontsource.Size;
			var iconcolor = (fontsource.Color != Color.Default ? fontsource.Color : Color.White).ToWindowsColor();
			return Task.FromResult((Windows.UI.Xaml.Media.ImageSource)new ImageSourceProviderImageSource(fontFamily, fontSize, fontsource.Glyph, iconcolor));
		}

		public Task<Microsoft.UI.Xaml.Controls.IconSource> LoadIconSourceAsync(ImageSource imagesource, CancellationToken cancellationToken = default(CancellationToken))
		{
			Microsoft.UI.Xaml.Controls.IconSource image = null;

			if (imagesource is FontImageSource fontImageSource)
			{
				image = new WFontIconSource
				{
					Glyph = fontImageSource.Glyph,
					FontSize = fontImageSource.Size,
					Foreground = fontImageSource.Color.ToBrush()
				};

				var uwpFontFamily = fontImageSource.FontFamily.ToFontFamily();

				if (!string.IsNullOrEmpty(uwpFontFamily.Source))
					((WFontIconSource)image).FontFamily = uwpFontFamily;
			}

			return Task.FromResult(image);
		}

		public Task<IconElement> LoadIconElementAsync(ImageSource imagesource, CancellationToken cancellationToken = default(CancellationToken))
		{
			IconElement image = null;

			if (imagesource is FontImageSource fontImageSource)
			{
				image = new FontIcon
				{
					Glyph = fontImageSource.Glyph,
					FontSize = fontImageSource.Size,
					Foreground = fontImageSource.Color.ToBrush()
				};

				var uwpFontFamily = fontImageSource.FontFamily.ToFontFamily();

				if (!string.IsNullOrEmpty(uwpFontFamily.Source))
					((FontIcon)image).FontFamily = uwpFontFamily;
			}

			return Task.FromResult(image);
		}

		string GetFontSource(FontImageSource fontImageSource)
		{
			if (fontImageSource == null)
				return string.Empty;

			var fontFamily = fontImageSource.FontFamily.ToFontFamily();

			string fontSource = fontFamily.Source;

			var allFamilies = fontFamily.Source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (allFamilies.Length > 1)
			{       
				// There's really no perfect solution to handle font families with fallbacks (comma-separated)	
				// So if the font family has fallbacks, only one is taken, because CanvasTextFormat	
				// only supports one font family
				string source = fontImageSource.FontFamily;

				foreach(var family in allFamilies)
				{
					if(family.Contains(source))
					{
						fontSource = family;
						break;
					}
				}
			}

			return fontSource;
		}
	}
}
