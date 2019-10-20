using System;
using System.Globalization;

namespace Xamarin.Forms
{
	[Xaml.TypeConversion(typeof(IItemsLayout))]
	public class ItemsLayoutTypeConverter : TypeConverter
	{
		private const string HorizontalGridPrefix = "HorizontalGrid,";
		private const string VerticalGridPrefix = "VerticalGrid,";

		public override object ConvertFromInvariantString(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			switch (value)
			{
				case "HorizontalList":
					return LinearItemsLayout.Horizontal;
				case "VerticalList":
					return LinearItemsLayout.Vertical;
				case "HorizontalGrid":
					return new GridItemsLayout(ItemsLayoutOrientation.Horizontal);
				case "VerticalGrid":
					return new GridItemsLayout(ItemsLayoutOrientation.Vertical);
			}

			if (value.StartsWith(HorizontalGridPrefix, StringComparison.Ordinal))
			{
				var span = ParseGridSpan(value, HorizontalGridPrefix);
				return new GridItemsLayout(span, ItemsLayoutOrientation.Horizontal);
			}
			else if (value.StartsWith(VerticalGridPrefix, StringComparison.Ordinal))
			{
				var span = ParseGridSpan(value, VerticalGridPrefix);
				return new GridItemsLayout(span, ItemsLayoutOrientation.Vertical);
			}

			throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(IItemsLayout)}");
		}

		private static int ParseGridSpan(string value, string prefix)
		{
			var argument = value.Substring(prefix.Length);
			return int.Parse(argument, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture);
		}
	}
}