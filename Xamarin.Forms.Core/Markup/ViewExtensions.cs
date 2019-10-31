﻿namespace Xamarin.Forms.Markup
{
	public static class ViewExtensions
	{
		public static TView Left<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.Start; return view; }

		public static TView CenterH<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.Center; return view; }

		public static TView FillH<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.Fill; return view; }

		public static TView Right<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.End; return view; }

		public static TView LeftExpand<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.StartAndExpand; return view; }

		public static TView CenterExpandH<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.CenterAndExpand; return view; }

		public static TView FillExpandH<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.FillAndExpand; return view; }

		public static TView RightExpand<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.EndAndExpand; return view; }

		public static TView Top<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.Start; return view; }

		public static TView Bottom<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.End; return view; }

		public static TView CenterV<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.Center; return view; }

		public static TView FillV<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.Fill; return view; }

		public static TView TopExpand<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.StartAndExpand; return view; }

		public static TView BottomExpand<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.EndAndExpand; return view; }

		public static TView CenterExpandV<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.CenterAndExpand; return view; }

		public static TView FillExpandV<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.FillAndExpand; return view; }

		public static TView Center<TView>(this TView view) where TView : View
			=> view.CenterH().CenterV();

		public static TView Fill<TView>(this TView view) where TView : View
			=> view.FillH().FillV();

		public static TView CenterExpand<TView>(this TView view) where TView : View
			=> view.CenterExpandH().CenterExpandV();

		public static TView FillExpand<TView>(this TView view) where TView : View
			=> view.FillExpandH().FillExpandV();

		public static TView Margin<TView>(this TView view, Thickness margin) where TView : View
		{ view.Margin = margin; return view; }

		public static TView Margin<TView>(this TView view, double horizontal, double vertical) where TView : View
		{ view.Margin = new Thickness(horizontal, vertical); return view; }

		public static TView Margins<TView>(this TView view, double left = 0, double top = 0, double right = 0, double bottom = 0) where TView : View
		{ view.Margin = new Thickness(left, top, right, bottom); return view; }

		public static TView Font<TView>(
			this TView view,
			double? fontSize = null,
			bool? bold = null,
			bool? italic = null,
			string family = null
		) where TView : View
		{
			var attributes = bold == true ? FontAttributes.Bold :
							 italic == true ? FontAttributes.Italic :
							 bold.HasValue || italic.HasValue ? FontAttributes.None :
							 (FontAttributes?)null;

			switch (view)
			{
				case Button button:
					if (fontSize.HasValue)
						button.FontSize = fontSize.Value;
					if (attributes.HasValue)
						button.FontAttributes = attributes.Value;
					if (family != null)
						button.FontFamily = family;
					break;
				case Label label:
					if (fontSize.HasValue)
						label.FontSize = fontSize.Value;
					if (attributes.HasValue)
						label.FontAttributes = attributes.Value;
					if (family != null)
						label.FontFamily = family;
					break;
				case Entry entry:
					if (fontSize.HasValue)
						entry.FontSize = fontSize.Value;
					if (attributes.HasValue)
						entry.FontAttributes = attributes.Value;
					if (family != null)
						entry.FontFamily = family;
					break;
				case Picker picker:
					if (fontSize.HasValue)
						picker.FontSize = fontSize.Value;
					if (attributes.HasValue)
						picker.FontAttributes = attributes.Value;
					if (family != null)
						picker.FontFamily = family;
					break;
			}
			return view;
		}
	}
}
