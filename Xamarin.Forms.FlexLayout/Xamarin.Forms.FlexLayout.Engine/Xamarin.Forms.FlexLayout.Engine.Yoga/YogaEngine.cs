using Facebook.Yoga;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System.Collections;

namespace Xamarin.FlexLayoutEngine.Yoga
{
	public partial class YogaEngine : YogaNode, IFlexNode
	{

		public YogaEngine() : base(new YogaConfig { })
		{

		}
		MeasureFunc _internalMeasure;

		float IFlexNode.LayoutLeft => LayoutX;
		float IFlexNode.LayoutTop => LayoutY;

		Align IFlexNode.AlignContent
		{
			get
			{
				return AlignContent.ConvertTo<Align>();
			}

			set
			{
				AlignContent = value.ConvertTo<YogaAlign>();
			}
		}

		Align IFlexNode.AlignItems
		{
			get
			{
				return AlignItems.ConvertTo<Align>();
			}

			set
			{
				AlignItems = value.ConvertTo<YogaAlign>();
			}
		}

		Align IFlexNode.AlignSelf
		{
			get
			{
				return AlignSelf.ConvertTo<Align>();
			}

			set
			{
				AlignSelf = value.ConvertTo<YogaAlign>();
			}
		}

		float IFlexNode.FlexBasis
		{
			get
			{
				return FlexBasis.Value;
			}

			set
			{
				FlexBasis = value;
			}
		}

		FlexDirection IFlexNode.FlexDirection
		{
			get
			{
				return FlexDirection.ConvertTo<FlexDirection>();
			}

			set
			{
				FlexDirection = value.ConvertTo<YogaFlexDirection>();
			}
		}



		Justify IFlexNode.JustifyContent
		{
			get
			{
				return JustifyContent.ConvertTo<Justify>();
			}

			set
			{
				JustifyContent = value.ConvertTo<YogaJustify>();
			}
		}

		Overflow IFlexNode.Overflow
		{
			get
			{
				return Overflow.ConvertTo<Overflow>();
			}

			set
			{
				Overflow = value.ConvertTo<YogaOverflow>();
			}
		}

		FlexPosition IFlexNode.PositionType
		{
			get
			{
				return PositionType.ConvertTo<FlexPosition>();
			}

			set
			{
				PositionType = value.ConvertTo<YogaPositionType>();
			}
		}

		Wrap IFlexNode.Wrap
		{
			get
			{
				return Wrap.ConvertTo<Wrap>();
			}

			set
			{
				Wrap = value.ConvertTo<YogaWrap>();
			}
		}
		float IFlexNode.Width
		{
			get
			{
				return (float)Width.Value;
			}

			set
			{
				Width = value;
			}
		}

		float IFlexNode.Height
		{
			get
			{
				return Height.Value;
			}

			set
			{
				Height = value;
			}
		}

		float IFlexNode.Left
		{
			get
			{
				return Left.Value;
			}

			set
			{
				Left = value;
			}
		}

		float IFlexNode.Top
		{
			get
			{
				return Top.Value;
			}

			set
			{
				Top = value;
			}
		}

		float IFlexNode.MarginBottom
		{
			get
			{
				return MarginBottom.Value;
			}

			set
			{
				MarginBottom = value;
			}
		}

		float IFlexNode.MarginLeft
		{
			get
			{
				return MarginLeft.Value;
			}

			set
			{
				MarginLeft = value;
			}
		}

		float IFlexNode.MarginRight
		{
			get
			{
				return MarginRight.Value;
			}

			set
			{
				MarginRight = value;
			}
		}

		float IFlexNode.MarginTop
		{
			get
			{
				return MarginRight.Value;
			}

			set
			{
				MarginTop = value;
			}
		}

		float IFlexNode.MinHeight
		{
			get
			{
				return MinHeight.Value;
			}

			set
			{
				MinHeight = value;
			}
		}

		float IFlexNode.MinWidth
		{
			get
			{
				return MinWidth.Value;
			}

			set
			{
				MinWidth = value;
			}
		}

		public void Insert(int i, IFlexNode subViewNode)
		{
			Insert(i, subViewNode as YogaNode);
		}

		public void SetMeasure(MeasureFunc measureView)
		{
			_internalMeasure = measureView;
			if (_internalMeasure != null)
				SetMeasureFunction(MeasureView);
		}

		YogaSize MeasureView(YogaNode node, float width, YogaMeasureMode widthMode, float height, YogaMeasureMode heightMode)
		{
			var flexWidthMode = widthMode.ConvertTo<FlexMeasureMode>();
			var flexheightMode = heightMode.ConvertTo<FlexMeasureMode>();

			Size size = _internalMeasure(this, width, flexWidthMode, height, flexheightMode);

			return MeasureOutput.Make((float)size.Width, (float)size.Height);
		}

		IEnumerator<IFlexNode> IEnumerable<IFlexNode>.GetEnumerator()
		{
			return GetEnumerator().Cast<IFlexNode>();
		}
	}

	internal static class YogaNodeExtensions
	{
		public static T ConvertTo<T>(this Enum value)
		{
			return (T)Enum.Parse(typeof(T), value.ToString());
		}

		public static IEnumerator<T> Cast<T>(this IEnumerator iterator)
		{
			while (iterator.MoveNext())
			{
				yield return (T)iterator.Current;
			}
		}
	}
}
