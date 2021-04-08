using System.Runtime.InteropServices;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Tizen
{
	public static class FontExtensions
	{
		public static string ToNativeFontFamily(this string self)
		{
			if (string.IsNullOrEmpty(self))
				return null;

			var cleansedFont = CleanseFontName(self);
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

		static string CleanseFontName(string fontName)
		{
			//First check Alias
			var (hasFontAlias, fontPostScriptName) = Controls.Internals.Registrar.FontRegistrar.HasFont(fontName);
			if (hasFontAlias)
				return fontPostScriptName;
			var fontFile = FontFile.FromString(fontName);

			if (!string.IsNullOrWhiteSpace(fontFile.Extension))
			{
				var (hasFont, _) = Controls.Internals.Registrar.FontRegistrar.HasFont(fontFile.FileNameWithExtension());
				if (hasFont)
					return fontFile.PostScriptName;
			}
			else
			{
				foreach (var ext in FontFile.Extensions)
				{
					var formated = fontFile.FileNameWithExtension(ext);
					var (hasFont, filePath) = Controls.Internals.Registrar.FontRegistrar.HasFont(formated);
					if (hasFont)
						return fontFile.PostScriptName;
				}
			}
			return fontFile.PostScriptName;
		}

		public static void FontReinit()
		{
			evas_font_reinit();
		}

		[DllImport("libelementary.so.1")]
		static extern void evas_font_reinit();
	}
}
