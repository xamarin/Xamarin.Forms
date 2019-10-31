﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.Markup
{
	public static class EnumsForGridRowsAndColumns
	{
		public static GridLength Auto => GridLength.Auto;
		public static GridLength Star => GridLength.Star;
		static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);

		public static class Columns
		{
			public static ColumnDefinitionCollection Define<TEnum>(params (TEnum name, GridLength width)[] cols) where TEnum : Enum
			{
				var columnDefinitions = new ColumnDefinitionCollection();
				for (int i = 0; i < cols.Length; i++)
				{
					if (i != cols[i].name.ToInt())
						throw new ArgumentException(
							$"Value of column name { cols[i].name } is not { i }. " +
							"Columns must be defined with enum names whose values form the sequence 0,1,2,..."
						);
					columnDefinitions.Add(new ColumnDefinition { Width = cols[i].width });
				}
				return columnDefinitions;
			}
		}

		public static class Rows
		{
			public static RowDefinitionCollection Define<TEnum>(params (TEnum name, GridLength height)[] rows) where TEnum : Enum
			{
				var rowDefinitions = new RowDefinitionCollection();
				for (int i = 0; i < rows.Length; i++)
				{
					if (i != rows[i].name.ToInt())
						throw new ArgumentException(
							$"Value of row name { rows[i].name } is not { i }. " +
							"Rows must be defined with enum names whose values form the sequence 0,1,2,..."
						);
					rowDefinitions.Add(new RowDefinition { Height = rows[i].height });
				}
				return rowDefinitions;
			}
		}

		public static int All<TEnum>() where TEnum : Enum
		{
			var values = Enum.GetValues(typeof(TEnum));
			int span = (int)values.GetValue(values.Length - 1) + 1;
			return span;
		}

		public static int Last<TEnum>() where TEnum : Enum
		{
			var values = Enum.GetValues(typeof(TEnum));
			int last = (int)values.GetValue(values.Length - 1);
			return last;
		}
	}
}
