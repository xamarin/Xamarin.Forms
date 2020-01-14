using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Markup
{
	public static class ElementExtensions
	{
		public static TElement Effects<TElement>(this TElement element, params Effect[] effects) where TElement : Element
		{
			for (int i = 0; i < effects.Length; i++)
				element.Effects.Add(effects[i]);
			return element;
		}

		public static TFontElement Font<TFontElement>(
			this TFontElement fontElement,
			double? fontSize = null,
			bool? bold = null,
			bool? italic = null,
			string family = null
		) where TFontElement : Element, IFontElement
		{
			var attributes = bold.HasValue || italic.HasValue ? FontAttributes.None : (FontAttributes?)null;
			if (bold == true)
				attributes |= FontAttributes.Bold;
			if (italic == true)
				attributes |= FontAttributes.Italic;

			if (fontSize.HasValue)
				fontElement.SetValue(FontElement.FontSizeProperty, fontSize.Value);
			if (attributes.HasValue)
				fontElement.SetValue(FontElement.FontSizeProperty, attributes.Value);
			if (family != null)
				fontElement.SetValue(FontElement.FontFamilyProperty, family);
			return fontElement;
		}
	}
}
