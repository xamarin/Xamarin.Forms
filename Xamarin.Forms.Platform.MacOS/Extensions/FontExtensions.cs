using System;
using System.Collections.Generic;
using System.Diagnostics;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public static class FontExtensions
	{
		static readonly Dictionary<ToNSFontKey, NSFont> ToNsFont = new Dictionary<ToNSFontKey, NSFont>();

		public static NSFont ToNSFont(this Font self)
		{
			var size = (float)self.FontSize;
			if (self.UseNamedSize)
			{
				switch (self.NamedSize)
				{
					case NamedSize.Micro:
						size = 12;
						break;
					case NamedSize.Small:
						size = 14;
						break;
					case NamedSize.Medium:
						size = 17; // as defined by iOS documentation
						break;
					case NamedSize.Large:
						size = 22;
						break;
					default:
						size = 17;
						break;
				}
			}

			var bold = self.FontAttributes.HasFlag(FontAttributes.Bold);
			var italic = self.FontAttributes.HasFlag(FontAttributes.Italic);

			if (self.FontFamily != null)
			{
				try
				{
					Console.WriteLine("ToNSFont not implement FontFamily != null");

					var descriptor = new NSFontDescriptor().FontDescriptorWithFamily(self.FontFamily);

					if (bold || italic)
					{
						var traits = (NSFontSymbolicTraits)0;
						if (bold)
							traits = traits | NSFontSymbolicTraits.BoldTrait;
						if (italic)
							traits = traits | NSFontSymbolicTraits.ItalicTrait;

						descriptor = descriptor.FontDescriptorWithSymbolicTraits(traits);
						return NSFont.FromDescription(descriptor, size);
					}

					return NSFont.FromFontName(self.FontFamily, size);
				}
				catch
				{
					Debug.WriteLine("Could not load font named: {0}", self.FontFamily);
				}
			}
			if (bold && italic)
			{

				var defaultFont = NSFont.SystemFontOfSize(size);
				var descriptor = defaultFont.FontDescriptor.FontDescriptorWithSymbolicTraits(
					NSFontSymbolicTraits.BoldTrait |
					NSFontSymbolicTraits.ItalicTrait);

				NSFont.FromDescription(descriptor, 0);

			}
			if (bold)
				return NSFont.BoldSystemFontOfSize(size);
			if (italic)
			{
				Console.WriteLine("Italic font requested, passing regular one");
				return NSFont.UserFontOfSize(size);
			}
			return NSFont.SystemFontOfSize(size);
		}

		internal static bool IsDefault(this Span self)
		{
			return self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true) && self.FontAttributes == FontAttributes.None;
		}

		internal static NSFont ToNSFont(this Label label)
		{
			var values = label.GetValues(Label.FontFamilyProperty, Label.FontSizeProperty, Label.FontAttributesProperty);
			return ToNSFont((string)values[0], (float)(double)values[1], (FontAttributes)values[2]) ?? NSFont.SystemFontOfSize(NSFont.LabelFontSize);
		}

		internal static NSFont ToNSFont(this IFontElement element)
		{
			return ToNSFont(element.FontFamily, (float)element.FontSize, element.FontAttributes);
		}

		static NSFont _ToNSFont(string family, float size, FontAttributes attributes)
		{
			var bold = (attributes & FontAttributes.Bold) != 0;
			var italic = (attributes & FontAttributes.Italic) != 0;

			if (family != null)
			{
				try
				{
					NSFont result;

					var descriptor = new NSFontDescriptor().FontDescriptorWithFamily(family);

					if (bold || italic)
					{
						var traits = (NSFontSymbolicTraits)0;
						if (bold)
							traits = traits | NSFontSymbolicTraits.BoldTrait;
						if (italic)
							traits = traits | NSFontSymbolicTraits.ItalicTrait;

						descriptor = descriptor.FontDescriptorWithSymbolicTraits(traits);
						result = NSFont.FromDescription(descriptor, size);
						if (result != null)
							return result;
					}


					result = NSFont.FromFontName(family, size);
					if (result != null)
						return result;
				}
				catch
				{
					Debug.WriteLine("Could not load font named: {0}", family);
				}
			}

			if (bold && italic)
			{
				var defaultFont = NSFont.SystemFontOfSize(size);

				var descriptor = defaultFont.FontDescriptor.FontDescriptorWithSymbolicTraits(NSFontSymbolicTraits.BoldTrait | NSFontSymbolicTraits.ItalicTrait);
				return NSFont.FromDescription(descriptor, 0);
			}
			if (bold)
				return NSFont.BoldSystemFontOfSize(size);
			if (italic)
				return NSFont.SystemFontOfSize(size);

			return NSFont.SystemFontOfSize(size);
		}

		static NSFont ToNSFont(string family, float size, FontAttributes attributes)
		{
			var key = new ToNSFontKey(family, size, attributes);

			lock (ToNsFont)
			{
				NSFont value;
				if (ToNsFont.TryGetValue(key, out value))
					return value;
			}

			var generatedValue = _ToNSFont(family, size, attributes);

			lock (ToNsFont)
			{
				NSFont value;
				if (!ToNsFont.TryGetValue(key, out value))
					ToNsFont.Add(key, value = generatedValue);
				return value;
			}
		}

		struct ToNSFontKey
		{
			internal ToNSFontKey(string family, float size, FontAttributes attributes)
			{
				_family = family;
				_size = size;
				_attributes = attributes;
			}
#pragma warning disable 0414 // these are not called explicitly, but they are used to establish uniqueness. allow it!
			string _family;
			float _size;
			FontAttributes _attributes;
#pragma warning restore 0414
		}
	}
}

