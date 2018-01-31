using System;
namespace Xamarin.Forms
{
	//TODO require a typeConverter for css values
	public enum FlexJustify
	{
		FlexStart = Flex.Align.Start,
		Center = Flex.Align.Center,
		FlexEnd = Flex.Align.End,
		SpaceBetween = Flex.Align.SpaceBetween,
		SpaceAround = Flex.Align.SpaceAround,
		SpaceEvenly = Flex.Align.SpaceEvenly,
	}

	public enum FlexPosition
	{
		Relative = Flex.Position.Relative,
		Absolute = Flex.Position.Absolute,
	}

	//TODO require a typeConverter for css values
	public enum FlexDirection
	{
		Column = Flex.Direction.Column,
		ColumnReverse = Flex.Direction.ColumnReverse,
		Row = Flex.Direction.Row,
		RowReverse = Flex.Direction.RowReverse,
	}

	//TODO require a typeConverter for css values
	public enum FlexAlignContent
	{
		Stretch = Flex.Align.Stretch,
		Center = Flex.Align.Center,
		FlexStart = Flex.Align.Start,
		FlexEnd = Flex.Align.End,
		SpaceBetween = Flex.Align.SpaceBetween,
		SpaceAround = Flex.Align.SpaceAround,
	}

	//TODO require a typeConverter for css values
	public enum FlexAlignItems
	{
		Stretch = Flex.Align.Stretch,
		Center = Flex.Align.Center,
		FlexStart = Flex.Align.Start,
		FlexEnd = Flex.Align.End,
		//Baseline = Flex.Align.Baseline,
	}

	//TODO require a typeConverter for css values
	public enum FlexAlignSelf
	{
		Auto = Flex.Align.Auto,
		Stretch = Flex.Align.Stretch,
		Center = Flex.Align.Center,
		FlexStart = Flex.Align.Start,
		FlexEnd = Flex.Align.End,
		//Baseline = Flex.Align.Baseline,
	}

	//TODO require a typeConverter for css values
	public enum FlexWrap
	{
		NoWrap = Flex.Wrap.NoWrap,
		Wrap = Flex.Wrap.Wrap,
		Reverse = Flex.Wrap.WrapReverse,
	}

	//TODO require a typeConverter
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
	}
}