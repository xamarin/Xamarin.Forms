using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Maui.Controls.Internals
{
	public static class FontRegistrar
	{
		internal static readonly Dictionary<string, (ExportFontAttribute attribute, Assembly assembly)> EmbeddedFonts = new Dictionary<string, (ExportFontAttribute attribute, Assembly assembly)>();
		static Dictionary<string, (bool, string)> fontLookupCache = new Dictionary<string, (bool, string)>();
		public static void Register(ExportFontAttribute fontAttribute, Assembly assembly)
		{
			EmbeddedFonts[fontAttribute.FontFileName] = (fontAttribute, assembly);
			if (!string.IsNullOrWhiteSpace(fontAttribute.Alias))
				EmbeddedFonts[fontAttribute.Alias] = (fontAttribute, assembly);
		}

		static bool TryLoadFont(string cacheKey, Stream fontStream)
		{
			var type = Registrar.Registered.GetHandlerType(typeof(EmbeddedFont));
			var fontHandler = (IEmbeddedFontLoader)Activator.CreateInstance(type);

			var result = fontHandler.LoadFont(new EmbeddedFont { ResourceStream = fontStream });

			fontLookupCache[cacheKey] = result;

			return result.success;
		}

		internal static bool RegisterNative(string fontName, string alias = null)
		{
			var cacheKey = alias ?? fontName;

#if __IOS__ || IOS
			var mainBundlePath = NSBundle.MainBundle.BundlePath;
			var fontBundlePath = Path.Combine(mainBundlePath, fontName);

			if (!File.Exists(fontBundlePath))
				fontBundlePath = Path.Combine(mainBundlePath, "Resources", fontName);

			if (!File.Exists(fontBundlePath))
				fontBundlePath = Path.Combine(mainBundlePath, "Fonts", fontName);

			if (!File.Exists(fontBundlePath))
				fontBundlePath = Path.Combine(mainBundlePath, "Resources", "Fonts", fontName);

			if (!File.Exists(fontBundlePath))
				return false;

			using (var fontStream = File.OpenRead(fontBundlePath))
			{
				return TryLoadFont(cacheKey, fontStream);
			}
#elif __ANDROID__ || ANDROID
			var assets = Android.App.Application.Context.Assets;
			using (var fontStream = assets.Open(fontName))
			{
				return TryLoadFont(cacheKey, fontStream);
			}
#endif
			return false;
		}

		//TODO: Investigate making this Async
		public static (bool hasFont, string fontPath) HasFont(string font)
		{
			try
			{
				if (!EmbeddedFonts.TryGetValue(font, out var foundFont))
				{
					return (false, null);
				}

				if (fontLookupCache.TryGetValue(font, out var foundResult))
					return foundResult;


				var fontStream = GetEmbeddedResourceStream(foundFont.assembly, foundFont.attribute.FontFileName);

				var type = Registrar.Registered.GetHandlerType(typeof(EmbeddedFont));
				var fontHandler = (IEmbeddedFontLoader)Activator.CreateInstance(type);
				var result = fontHandler.LoadFont(new EmbeddedFont { FontName = foundFont.attribute.FontFileName, ResourceStream = fontStream });
				return fontLookupCache[font] = result;

			}
			catch (Exception ex)
			{
#if !__ANDROID__ && !__IOS__
				Debug.WriteLine(ex);
#else
				System.Console.WriteLine(ex);
#endif
			}
			return fontLookupCache[font] = (false, null);
		}

		static Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName)
		{
			var resourceNames = assembly.GetManifestResourceNames();

			var resourcePaths = resourceNames
				.Where(x => x.EndsWith(resourceFileName, StringComparison.CurrentCultureIgnoreCase))
				.ToArray();

			if (!resourcePaths.Any())
			{
				throw new Exception(string.Format("Resource ending with {0} not found.", resourceFileName));
			}
			if (resourcePaths.Length > 1)
			{
				resourcePaths = resourcePaths.Where(x => IsFile(x, resourceFileName)).ToArray();
			}

			return assembly.GetManifestResourceStream(resourcePaths.FirstOrDefault());
		}

		static bool IsFile(string path, string file)
		{
			if (!path.EndsWith(file, StringComparison.Ordinal))
				return false;
			return path.Replace(file, "").EndsWith(".", StringComparison.Ordinal);
		}
	}
}
