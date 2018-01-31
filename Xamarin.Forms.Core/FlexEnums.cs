using System;
using System.Globalization;
            
namespace Xamarin.Forms
{
	[TypeConverter(typeof(FlexJustifyTypeConverter))]
	public enum FlexJustify
	{
		FlexStart = Flex.Align.Start,
		Center = Flex.Align.Center,
		FlexEnd = Flex.Align.End,
		SpaceBetween = Flex.Align.SpaceBetween,
		SpaceAround = Flex.Align.SpaceAround,
		//SpaceEvenly = Flex.Align.SpaceEvenly,
	}

	[Xaml.TypeConversion(typeof(FlexJustify))]
	public class FlexJustifyTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null) {
				if (Enum.TryParse(value, true, out FlexJustify justify))
					return justify;
				if (value.Equals("flex-start", StringComparison.OrdinalIgnoreCase))
					return FlexJustify.FlexStart;
				if (value.Equals("flex-end", StringComparison.OrdinalIgnoreCase))
					return FlexJustify.FlexEnd;
				if (value.Equals("space-between", StringComparison.OrdinalIgnoreCase))
					return FlexJustify.SpaceBetween;
				if (value.Equals("space-around", StringComparison.OrdinalIgnoreCase))
					return FlexJustify.SpaceAround;
			}
			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(FlexJustify)));
		}
	}

	public enum FlexPosition
	{
		Relative = Flex.Position.Relative,
		Absolute = Flex.Position.Absolute,
	}

	[TypeConverter(typeof(FlexDirectionTypeConverter))]
	public enum FlexDirection
	{
		Column = Flex.Direction.Column,
		ColumnReverse = Flex.Direction.ColumnReverse,
		Row = Flex.Direction.Row,
		RowReverse = Flex.Direction.RowReverse,
	}

	[Xaml.TypeConversion(typeof(FlexDirection))]
	public class FlexDirectionTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null) {
				if (Enum.TryParse(value, true, out FlexDirection aligncontent))
					return aligncontent;
				if (value.Equals("row-reverse", StringComparison.OrdinalIgnoreCase))
					return FlexDirection.RowReverse;
				if (value.Equals("column-reverse", StringComparison.OrdinalIgnoreCase))
					return FlexDirection.ColumnReverse;
			}
			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(FlexDirection)));
		}
	}

	[TypeConverter(typeof(FlexAlignContentTypeConverter))]
	public enum FlexAlignContent
	{
		Stretch = Flex.Align.Stretch,
		Center = Flex.Align.Center,
		FlexStart = Flex.Align.Start,
		FlexEnd = Flex.Align.End,
		SpaceBetween = Flex.Align.SpaceBetween,
		SpaceAround = Flex.Align.SpaceAround,
	}

	[Xaml.TypeConversion(typeof(FlexAlignContent))]
	public class FlexAlignContentTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null) {
				if (Enum.TryParse(value, true, out FlexAlignContent aligncontent))
					return aligncontent;
				if (value.Equals("flex-start", StringComparison.OrdinalIgnoreCase))
					return FlexAlignContent.FlexStart;
				if (value.Equals("flex-end", StringComparison.OrdinalIgnoreCase))
					return FlexAlignContent.FlexEnd;
				if (value.Equals("space-between", StringComparison.OrdinalIgnoreCase))
					return FlexAlignContent.SpaceBetween;
				if (value.Equals("space-around", StringComparison.OrdinalIgnoreCase))
					return FlexAlignContent.SpaceAround;
			}
			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(FlexAlignContent)));
		}
	}

	[TypeConverter(typeof(FlexAlignItemsTypeConverter))]
	public enum FlexAlignItems
	{
		Stretch = Flex.Align.Stretch,
		Center = Flex.Align.Center,
		FlexStart = Flex.Align.Start,
		FlexEnd = Flex.Align.End,
		//Baseline = Flex.Align.Baseline,
	}

	[Xaml.TypeConversion(typeof(FlexAlignItems))]
	public class FlexAlignItemsTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null) {
				if (Enum.TryParse(value, true, out FlexAlignItems alignitems))
					return alignitems;
				if (value.Equals("flex-start", StringComparison.OrdinalIgnoreCase))
					return FlexAlignItems.FlexStart;
				if (value.Equals("flex-end", StringComparison.OrdinalIgnoreCase))
					return FlexAlignItems.FlexEnd;
			}
			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(FlexAlignItems)));
		}
	}

	[TypeConverter(typeof(FlexAlignSelfTypeConverter))]
	public enum FlexAlignSelf
	{
		Auto = Flex.Align.Auto,
		Stretch = Flex.Align.Stretch,
		Center = Flex.Align.Center,
		FlexStart = Flex.Align.Start,
		FlexEnd = Flex.Align.End,
		//Baseline = Flex.Align.Baseline,
	}

	[Xaml.TypeConversion(typeof(FlexAlignSelf))]
	public class FlexAlignSelfTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null) {
				if (Enum.TryParse(value, true, out FlexAlignSelf alignself))
					return alignself;
				if (value.Equals("flex-start", StringComparison.OrdinalIgnoreCase))
					return FlexAlignSelf.FlexStart;
				if (value.Equals("flex-end", StringComparison.OrdinalIgnoreCase))
					return FlexAlignSelf.FlexEnd;
			}
			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(FlexAlignSelf)));
		}
	}

	[TypeConverter(typeof(FlexWrapTypeConverter))]
	public enum FlexWrap
	{
		NoWrap = Flex.Wrap.NoWrap,
		Wrap = Flex.Wrap.Wrap,
		Reverse = Flex.Wrap.WrapReverse,
	}

	[Xaml.TypeConversion(typeof(FlexWrap))]
	public class FlexWrapTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null) {
				if (Enum.TryParse(value, true, out FlexWrap wrap))
					return wrap;
				if (value.Equals("wrap-reverse", StringComparison.OrdinalIgnoreCase))
					return FlexWrap.Reverse;
			}
			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(FlexWrap)));
		}
	}

	[TypeConverter(typeof(FlexBasisTypeConverter))]
	public struct FlexBasis
	{
		bool _isLength;
		public static FlexBasis Auto = new FlexBasis();
		public float Length { get; }
		internal bool IsAuto => !_isLength;

		public FlexBasis(float length)
		{
			if (length < 0)
				throw new ArgumentException("should be a positive value", nameof(length));
			_isLength = true;
			Length = length;
		}

		public static implicit operator FlexBasis(float length)
		{
			return new FlexBasis(length);
		}

		[Xaml.TypeConversion(typeof(FlexBasis))]
		public class FlexBasisTypeConverter : TypeConverter
		{
			public override object ConvertFromInvariantString(string value)
			{
				if (value != null) {
					if (value.Equals("auto", StringComparison.OrdinalIgnoreCase))
						return Auto;
					if (float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out float flex))
						return new FlexBasis(flex);
				}
				throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(FlexBasis)));
			}
		}
	}
}