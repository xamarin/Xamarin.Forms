using Xamarin.Forms.Internals;
using static Xamarin.Forms.Core.Markup.Markup;

namespace Xamarin.Forms.Markup
{
	public static class ElementExtensions
	{
		public static TElement DynamicResource<TElement>(this TElement element, BindableProperty property, string key) where TElement : Element
		{ VerifyExperimental(); element.SetDynamicResource(property, key); return element; }

		public static TElement DynamicResources<TElement>(this TElement element, params (BindableProperty property, string key)[] resources) where TElement : Element
		{ VerifyExperimental(); foreach (var resource in resources) element.SetDynamicResource(resource.property, resource.key); return element; }

		public static TElement RemoveDynamicResources<TElement>(this TElement element, params BindableProperty[] properties) where TElement : Element
		{ VerifyExperimental(); foreach (var property in properties) element.RemoveDynamicResource(property); return element; }

		public static TElement Effects<TElement>(this TElement element, params Effect[] effects) where TElement : Element
		{
			VerifyExperimental();
			for (int i = 0; i < effects.Length; i++)
				element.Effects.Add(effects[i]);
			return element;
		}

		public static TFontElement FontSize<TFontElement>(this TFontElement fontElement, double size) where TFontElement : Element, IFontElement
		{ VerifyExperimental(); fontElement.SetValue(FontElement.FontSizeProperty, size); return fontElement; }

		public static TFontElement Bold<TFontElement>(this TFontElement fontElement) where TFontElement : Element, IFontElement
		{ VerifyExperimental(); fontElement.SetValue(FontElement.FontAttributesProperty, FontAttributes.Bold); return fontElement; }

		public static TFontElement Italic<TFontElement>(this TFontElement fontElement) where TFontElement : Element, IFontElement
		{ VerifyExperimental(); fontElement.SetValue(FontElement.FontAttributesProperty, FontAttributes.Italic); return fontElement; }

		public static TFontElement Font<TFontElement>(
			this TFontElement fontElement,
			double? size = null,
			bool? bold = null,
			bool? italic = null,
			string family = null
		) where TFontElement : Element, IFontElement
		{
			VerifyExperimental();
			if (size.HasValue)
				fontElement.SetValue(FontElement.FontSizeProperty, size.Value);

			if (bold.HasValue || italic.HasValue)
			{
				var attributes = FontAttributes.None;
				if (bold == true)
					attributes |= FontAttributes.Bold;
				if (italic == true)
					attributes |= FontAttributes.Italic;
				fontElement.SetValue(FontElement.FontAttributesProperty, attributes);
			}

			if (family != null)
				fontElement.SetValue(FontElement.FontFamilyProperty, family);

			return fontElement;
		}
	}
}
