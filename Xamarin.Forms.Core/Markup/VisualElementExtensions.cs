using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.Markup
{
	public static class VisualElementExtensions
	{
		public static TElement Height<TElement>(this TElement element, double request) where TElement : VisualElement
		{ element.HeightRequest = request; return element; }
		public static TElement Width<TElement>(this TElement element, double request) where TElement : VisualElement
		{ element.WidthRequest = request; return element; }
		public static TElement MinHeight<TElement>(this TElement element, double request) where TElement : VisualElement
		{ element.MinimumHeightRequest = request; return element; }
		public static TElement MinWidth<TElement>(this TElement element, double request) where TElement : VisualElement
		{ element.MinimumWidthRequest = request; return element; }

		public static TElement Size<TElement>(this TElement element, double widthRequest, double heightRequest) where TElement : VisualElement
			=> element.Width(widthRequest).Height(heightRequest);
		public static TElement Size<TElement>(this TElement element, double sizeRequest) where TElement : VisualElement
			=> element.Width(sizeRequest).Height(sizeRequest);
		public static TElement MinSize<TElement>(this TElement element, double widthRequest, double heightRequest) where TElement : VisualElement
			=> element.MinWidth(widthRequest).MinHeight(heightRequest);
		public static TElement MinSize<TElement>(this TElement element, double sizeRequest) where TElement : VisualElement
			=> element.MinWidth(sizeRequest).MinHeight(sizeRequest);

		public static T Style<T>(this T view, Style<T> style) where T : VisualElement
		{ view.Style = style; return view; }
	}
}
