using System;
using System.Collections.Concurrent;

namespace Microsoft.Maui
{
	public class FontManager : IFontManager
	{
		readonly ConcurrentDictionary<(string family, float size, FontAttributes attributes), string> _fonts =
			new ConcurrentDictionary<(string family, float size, FontAttributes attributes), string>();

		readonly IFontRegistrar _fontRegistrar;

		public FontManager(IFontRegistrar fontRegistrar)
		{
			_fontRegistrar = fontRegistrar;
		}

		public string GetFont(Font font)
		{
			var size = (float)font.FontSize;
			if (font.UseNamedSize)
			{
				switch (font.NamedSize)
				{
					case NamedSize.Micro:
						size = 12;
						break;
					case NamedSize.Small:
						size = 14;
						break;
					case NamedSize.Medium:
						size = 17;
						break;
					case NamedSize.Large:
						size = 22;
						break;
					default:
						size = 17;
						break;
				}
			}

#pragma warning disable CS8621 // Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).
			return GetFont(font.FontFamily, size, font.FontAttributes, GetNativeFontFamily);
#pragma warning restore CS8621 // Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).
		}

		public string? GetFontFamily(string fontFamliy)
		{
			if (string.IsNullOrEmpty(fontFamliy))
				return null;

			var cleansedFont = CleanseFontName(fontFamliy);
			if (cleansedFont == null)
				return null;

			int index = cleansedFont.LastIndexOf('-');
			if (index != -1)
			{
				string font = cleansedFont.Substring(0, index);
				string style = cleansedFont.Substring(index + 1);
				return $"{font}:style={style}";
			}
			else
			{
				return cleansedFont;
			}
		}

		string GetFont(string family, float size, FontAttributes attributes, Func<(string, float, FontAttributes), string> factory)
		{
			return _fonts.GetOrAdd((family, size, attributes), factory);
		}

		string? GetNativeFontFamily((string family, float size, FontAttributes attributes) fontKey)
		{
			if (string.IsNullOrEmpty(fontKey.family))
				return null;

			var cleansedFont = CleanseFontName(fontKey.family);

			if (cleansedFont == null)
				return null;

			int index = cleansedFont.LastIndexOf('-');
			if (index != -1)
			{
				string font = cleansedFont.Substring(0, index);
				string style = cleansedFont.Substring(index + 1);
				return $"{font}:style={style}";
			}
			else
			{
				return cleansedFont;
			}
		}

		string? CleanseFontName(string fontName)
		{
			// First check Alias
			var (hasFontAlias, fontPostScriptName) = _fontRegistrar.HasFont(fontName);
			if (hasFontAlias)
				return fontPostScriptName;

			var fontFile = FontFile.FromString(fontName);

			if (!string.IsNullOrWhiteSpace(fontFile.Extension))
			{
				var (hasFont, filePath) = _fontRegistrar.HasFont(fontFile.FileNameWithExtension());
				if (hasFont)
					return filePath ?? fontFile.PostScriptName;
			}
			else
			{
				foreach (var ext in FontFile.Extensions)
				{

					var formated = fontFile.FileNameWithExtension(ext);
					var (hasFont, filePath) = _fontRegistrar.HasFont(formated);
					if (hasFont)
						return filePath;
				}
			}

			return fontFile.PostScriptName;
		}
	}
}