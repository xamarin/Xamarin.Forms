using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.Markup
{
	public static class ViewInGridExtensions
	{
		public static TView Row<TView>(this TView view, int row, int span = 1) where TView : View
		{
			if (row != 0)
				view.SetValue(Grid.RowProperty, row);
			if (span != 1)
				view.SetValue(Grid.RowSpanProperty, span);
			return view;
		}

		public static TView RowSpan<TView>(this TView view, int span) where TView : View
		{
			if (span != 1)
				view.SetValue(Grid.RowSpanProperty, span);
			return view;
		}

		public static TView Col<TView>(this TView view, int col, int span = 1) where TView : View
		{
			if (col != 0)
				view.SetValue(Grid.ColumnProperty, col);
			if (span != 1)
				view.SetValue(Grid.ColumnSpanProperty, span);
			return view;
		}

		public static TView ColSpan<TView>(this TView view, int span) where TView : View
		{
			if (span != 1)
				view.SetValue(Grid.ColumnSpanProperty, span);
			return view;
		}

		public static TView RowCol<TView>(this TView view, int row, int col, int rowSpan = 1, int colSpan = 1)
			where TView : View
		{
			if (row != 0)
				view.SetValue(Grid.RowProperty, row);
			if (col != 0)
				view.SetValue(Grid.ColumnProperty, col);
			if (rowSpan != 1)
				view.SetValue(Grid.RowSpanProperty, rowSpan);
			if (colSpan != 1)
				view.SetValue(Grid.ColumnSpanProperty, colSpan);
			return view;
		}

		#region Use enum for Row / Col for better readability + avoid manual renumbering

		public static TView Row<TView, TRow>(this TView view, TRow row) where TView : View where TRow : Enum
		{
			int rowIndex = row.ToInt();
			if (rowIndex != 0)
				view.SetValue(Grid.RowProperty, rowIndex);
			return view;
		}

		public static TView Row<TView, TRow>(this TView view, TRow first, TRow last) where TView : View where TRow : Enum
		{
			int rowIndex = first.ToInt();
			int span = last.ToInt() - rowIndex + 1;
			if (rowIndex != 0)
				view.SetValue(Grid.RowProperty, rowIndex);
			if (span != 1)
				view.SetValue(Grid.RowSpanProperty, span);
			return view;
		}

		public static TView Col<TView, TCol>(this TView view, TCol col) where TView : View where TCol : Enum
		{
			int colIndex = col.ToInt();
			if (colIndex != 0)
				view.SetValue(Grid.ColumnProperty, colIndex);
			return view;
		}

		public static TView Col<TView, TCol>(this TView view, TCol first, TCol last) where TView : View where TCol : Enum
		{
			int colIndex = first.ToInt();
			if (colIndex != 0)
				view.SetValue(Grid.ColumnProperty, colIndex);

			int span = last.ToInt() + 1 - colIndex;
			if (span != 1)
				view.SetValue(Grid.ColumnSpanProperty, span);

			return view;
		}

		public static TView RowCol<TView, TRow, TCol>(
			this TView view,
			TRow? firstRow = null,
			TCol? firstCol = null,
			TRow? lastRow = null,
			TCol? lastCol = null
		) where TView : View where TRow : struct, Enum where TCol : struct, Enum
		{
			int firstRowIndex = firstRow.ToInt();
			if (firstRowIndex != 0)
				view.SetValue(Grid.RowProperty, firstRowIndex);

			int firstColIndex = firstCol.ToInt();
			if (firstColIndex != 0)
				view.SetValue(Grid.ColumnProperty, firstColIndex);

			if (lastRow != null)
			{
				int lastRowIndex = lastRow.ToInt();
				int rowSpan = lastRowIndex + 1 - firstRowIndex;
				if (rowSpan != 1)
					view.SetValue(Grid.RowSpanProperty, rowSpan);
			}

			if (lastCol != null)
			{
				int lastColIndex = lastCol.ToInt();
				int colSpan = lastColIndex + 1 - firstColIndex;
				if (colSpan != 1)
					view.SetValue(Grid.ColumnSpanProperty, colSpan);
			}

			return view;
		}

		static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);

		#endregion Use enum for Row / Col for better readability + avoid manual renumbering
	}
}
