using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Maui.Controls.Internals
{
	public class FontRegistrar : IFontRegistrar
	{
		readonly Dictionary<string, (ExportFontAttribute attribute, Assembly assembly)> _embeddedFonts =
			new Dictionary<string, (ExportFontAttribute attribute, Assembly assembly)>();

		readonly Dictionary<string, (bool, string)> _fontLookupCache =
			new Dictionary<string, (bool, string)>();

		public void Register(ExportFontAttribute fontAttribute, Assembly assembly)
		{
			_embeddedFonts[fontAttribute.FontFileName] = (fontAttribute, assembly);

			if (!string.IsNullOrWhiteSpace(fontAttribute.Alias))
				_embeddedFonts[fontAttribute.Alias] = (fontAttribute, assembly);
		}

		public (bool hasFont, string fontPath) HasFont(string font)
		{
			try
			{
				if (!_embeddedFonts.TryGetValue(font, out var foundFont))
					return (false, null);

				if (_fontLookupCache.TryGetValue(font, out var foundResult))
					return foundResult;

				var fontStream = GetEmbeddedResourceStream(foundFont.assembly, foundFont.attribute.FontFileName);

				var type = Registrar.Registered.GetHandlerType(typeof(EmbeddedFont));
				var fontHandler = (IEmbeddedFontLoader)Activator.CreateInstance(type);
				var result = fontHandler.LoadFont(new EmbeddedFont { FontName = foundFont.attribute.FontFileName, ResourceStream = fontStream });

				return _fontLookupCache[font] = result;

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

			if (!resourcePaths.Any())
				throw new Exception($"Resource ending with {resourceFileName} not found.");

			if (resourcePaths.Length > 1)
				resourcePaths = resourcePaths.Where(x => IsFile(x, resourceFileName)).ToArray();

			return assembly.GetManifestResourceStream(resourcePaths.FirstOrDefault());
		}

		bool IsFile(string path, string file)
		{
			if (!path.EndsWith(file, StringComparison.Ordinal))
				return false;

			return path.Replace(file, "").EndsWith(".", StringComparison.Ordinal);
		}
	}

	public interface IFontRegistrar
	{
		void Register(ExportFontAttribute fontAttribute, Assembly assembly);

		//TODO: Investigate making this Async
		(bool hasFont, string fontPath) HasFont(string font);
	}
}