using System;
using System.Globalization;

namespace Xamarin.Forms.Markup
{
	public static class ViewInGridExtensions
	{
		public static TView Row<TView>(this TView view, int row) where TView : View
		{
			view.SetValue(Grid.RowProperty, row);
			return view;
		}

		public static TView Row<TView>(this TView view, int row, int span) where TView : View
		{
			view.SetValue(Grid.RowProperty, row);
			view.SetValue(Grid.RowSpanProperty, span);
			return view;
		}

		public static TView RowSpan<TView>(this TView view, int span) where TView : View
		{
			view.SetValue(Grid.RowSpanProperty, span);
			return view;
		}

		public static TView Col<TView>(this TView view, int col) where TView : View
		{
			view.SetValue(Grid.ColumnProperty, col);
			return view;
		}

		public static TView Col<TView>(this TView view, int col, int span) where TView : View
		{
			view.SetValue(Grid.ColumnProperty, col);
			view.SetValue(Grid.ColumnSpanProperty, span);
			return view;
		}

		public static TView ColSpan<TView>(this TView view, int span) where TView : View
		{
			view.SetValue(Grid.ColumnSpanProperty, span);
			return view;
		}

		public static TView Row<TView, TRow>(this TView view, TRow row) where TView : View where TRow : Enum
		{
			int rowIndex = row.ToInt();
			view.SetValue(Grid.RowProperty, rowIndex);
			return view;
		}

		public static TView Row<TView, TRow>(this TView view, TRow first, TRow last) where TView : View where TRow : Enum
		{
			int rowIndex = first.ToInt();
			int span = last.ToInt() - rowIndex + 1;
			view.SetValue(Grid.RowProperty, rowIndex);
			view.SetValue(Grid.RowSpanProperty, span);
			return view;
		}

		public static TView Col<TView, TCol>(this TView view, TCol col) where TView : View where TCol : Enum
		{
			int colIndex = col.ToInt();
			view.SetValue(Grid.ColumnProperty, colIndex);
			return view;
		}

		public static TView Col<TView, TCol>(this TView view, TCol first, TCol last) where TView : View where TCol : Enum
		{
			int colIndex = first.ToInt();
			view.SetValue(Grid.ColumnProperty, colIndex);

			int span = last.ToInt() + 1 - colIndex;
			view.SetValue(Grid.ColumnSpanProperty, span);

			return view;
		}

		static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);
	}
}
