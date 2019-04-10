﻿using System.Diagnostics;
using Xamarin.Forms.Internals;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public static partial class FontExtensions
	{
		static readonly string DefaultFontName = NSFont.SystemFontOfSize(12).FontName;

		public static NSFont ToNSFont(this Font self) => ToNativeFont(self);

		internal static NSFont ToNSFont(this IFontElement element) => ToNativeFont(element);

		static NSFont _ToNativeFont(string family, float size, FontAttributes attributes)
		{
			NSFont defaultFont = NSFont.SystemFontOfSize(size);
			NSFont font = null;
			NSFontDescriptor descriptor = null;
			var bold = (attributes & FontAttributes.Bold) != 0;
			var italic = (attributes & FontAttributes.Italic) != 0;

			if (family != null && family != DefaultFontName)
			{
				try
				{
					descriptor = new NSFontDescriptor().FontDescriptorWithFamily(family);
					font = NSFont.FromDescription(descriptor, size);

					if (font == null)
						font = NSFont.FromFontName(family, size);
				}
				catch
				{
					Debug.WriteLine("Could not load font named: {0}", family);
				}
			}

			//if we didn't found a Font or Descriptor for the FontFamily use the default one 
			if (font == null)
				font = defaultFont;
		
			if (descriptor == null)
				descriptor = defaultFont.FontDescriptor;

			if (bold || italic)
			{
				var traits = (NSFontSymbolicTraits)0;
				if (bold)
					traits = traits | NSFontSymbolicTraits.BoldTrait;
				if (italic)
					traits = traits | NSFontSymbolicTraits.ItalicTrait;

				var fontDescriptoWithTraits = descriptor.FontDescriptorWithSymbolicTraits(traits);

				font = NSFont.FromDescription(fontDescriptoWithTraits, size);
			}

			return font.ScreenFontWithRenderingMode(NSFontRenderingMode.AntialiasedIntegerAdvancements);
		}
	}
}
