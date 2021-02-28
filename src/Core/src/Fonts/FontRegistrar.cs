using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Maui
{
	public class FontRegistrar : IFontRegistrar
	{
		readonly Dictionary<string, (string Filename, string Alias, Assembly Assembly)> _embeddedFonts =
			new Dictionary<string, (string Filename, string Alias, Assembly Assembly)>();

		readonly Dictionary<string, (bool Success, string? Path)> _fontLookupCache =
			new Dictionary<string, (bool Success, string? Path)>();

		IEmbeddedFontLoader? _fontLoader;

		public FontRegistrar(IEmbeddedFontLoader? fontLoader = null)
		{
			_fontLoader = fontLoader;
		}

		public void SetFontLoader(IEmbeddedFontLoader? fontLoader)
		{
			_fontLoader = fontLoader;
		}

		public void Register(string filename, string alias, Assembly assembly)
		{
			_embeddedFonts[filename] = (filename, alias, assembly);

			if (!string.IsNullOrWhiteSpace(alias))
				_embeddedFonts[alias] = (filename, alias, assembly);
		}

		public (bool hasFont, string? fontPath) HasFont(string font)
		{
			if (_fontLookupCache.TryGetValue(font, out var foundResult))
				return foundResult;

			try
			{
				if (_embeddedFonts.TryGetValue(font, out var foundFont))
				{
					using var stream = GetEmbeddedResourceStream(foundFont.Assembly, foundFont.Filename);

					var fontStream = new EmbeddedFont { FontName = foundFont.Filename, ResourceStream = stream };

					if (_fontLoader == null)
						throw new InvalidOperationException("Font loader was not set on the font registrar.");

					var result = _fontLoader.LoadFont(fontStream);

					return _fontLookupCache[font] = result;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return _fontLookupCache[font] = (false, null);
		}

		Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName)
		{
			var resourceNames = assembly.GetManifestResourceNames();

			var resourcePaths = resourceNames
				.Where(x => x.EndsWith(resourceFileName, StringComparison.CurrentCultureIgnoreCase))
				.ToArray();

			if (resourcePaths.Length == 0)
				throw new Exception($"Resource ending with {resourceFileName} not found.");

			if (resourcePaths.Length > 1)
				resourcePaths = resourcePaths.Where(x => IsFile(x, resourceFileName)).ToArray();

			return assembly.GetManifestResourceStream(resourcePaths[0]);
		}

		bool IsFile(string path, string file)
		{
			if (!path.EndsWith(file, StringComparison.Ordinal))
				return false;

			return path.Replace(file, "").EndsWith(".", StringComparison.Ordinal);
		}
	}
}