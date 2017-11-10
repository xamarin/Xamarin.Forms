using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.FlexLayoutEngine.Flex
{
	public class FlexEngine : Xamarin.Flex.Item, IFlexNode
	{

		object _data;
		Justify _justify;
		Overflow _overflow;
		bool _isDirty;

		public FlexEngine()
		{
		}

		float IFlexNode.LayoutTop => FrameY;

		float IFlexNode.LayoutLeft => FrameX;

		float IFlexNode.LayoutHeight => FrameHeight;

		float IFlexNode.LayoutWidth => FrameWidth;


		float IFlexNode.FlexGrow { get => Grow; set => Grow = (int)value; }
		float IFlexNode.FlexShrink { get => Shrink; set => Shrink = (int)value; }
		float IFlexNode.FlexBasis { get => Basis; set => Basis = value; }
		FlexPosition IFlexNode.PositionType { get => Position.ConvertTo<FlexPosition>(); set => Position = value.ConvertTo<Xamarin.Flex.Position>(); }
		Align IFlexNode.AlignItems { get => AlignItems.ConvertTo<Align>(); set => AlignItems = value.ConvertTo<Xamarin.Flex.Align>(); }
		Align IFlexNode.AlignSelf { get => AlignSelf.ConvertTo<Align>(); set => AlignSelf = value.ConvertTo<Xamarin.Flex.Align>(); }
		Align IFlexNode.AlignContent
		{
			get => AlignContent.ConvertTo<Align>(); set
			{
				var newvalue = value.ConvertTo<Xamarin.Flex.Align>();
				if (value == Align.Stretch)
				{
					newvalue = Xamarin.Flex.Align.Center;
				}
				AlignContent = newvalue;
			}
		}
		Wrap IFlexNode.Wrap { get => Wrap.ConvertTo<Wrap>(); set => Wrap = value.ConvertTo<Xamarin.Flex.Wrap>(); }
		Overflow IFlexNode.Overflow { get => _overflow; set => _overflow = value; }
		Justify IFlexNode.JustifyContent { get => _justify; set => _justify = value; }
		FlexDirection IFlexNode.FlexDirection { get => Direction.ConvertTo<FlexDirection>(); set => Direction = value.ConvertTo<Xamarin.Flex.Direction>(); }
		float IFlexNode.MarginLeft { get => MarginLeft; set => MarginLeft = value; }
		float IFlexNode.MarginTop { get => MarginTop; set => MarginTop = value; }
		float IFlexNode.MarginRight { get => MarginRight; set => MarginRight = value; }
		float IFlexNode.MarginBottom { get => MarginBottom; set => MarginBottom = value; }
		float IFlexNode.MinHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		float IFlexNode.MinWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		object IFlexNode.Data { get => _data; set => _data = value; }

		void IFlexNode.CalculateLayout()
		{
			if (_measure == null)
			{
				Layout();
				_isDirty = false;
			}

		}

		void IFlexNode.Clear()
		{
			for (int i = Count - 1; i >= 0; i--)
			{
				RemoveAt(i);
			}
		}

		IEnumerator<IFlexNode> IEnumerable<IFlexNode>.GetEnumerator() => GetEnumerator().Cast<IFlexNode>();

		void IFlexNode.Insert(int i, IFlexNode subViewNode)
		{
			InsertAt(i, subViewNode as Xamarin.Flex.Item);
		}

		bool IFlexNode.IsDirty => _isDirty;


		void IFlexNode.MarkDirty()
		{
			_isDirty = true;
		}

		MeasureFunc _measure;
		void IFlexNode.SetMeasure(MeasureFunc measureView)
		{
			_measure = measureView;
			if (_measure != null)
				this.SelfSizing = HandleSelfSizingDelegate;

		}

		void HandleSelfSizingDelegate(Xamarin.Flex.Item item, ref float width, ref float height)
		{
			var availableWidth = item.Parent.Width;
			var availableHeight = item.Parent.Height;
			var size = _measure?.Invoke(item as IFlexNode, availableWidth, availableHeight);
			width = (float)size.Value.Width;
			height = (float)size.Value.Height;
		}

	}

	internal static class ItemExtensions
	{
		public static T ConvertTo<T>(this Enum value)
		{
			var str = value.ToString();

			if (str == "FlexStart" && typeof(T) == typeof(Xamarin.Flex.Align))
			{
				str = "Start";
			}

			if (str == "FlexEnd" && typeof(T) == typeof(Xamarin.Flex.Align))
			{
				str = "End";
			}

			if (str == "Start" && typeof(T) == typeof(Align))
			{
				str = "FlexStart";
			}

			if (str == "End" && typeof(T) == typeof(Align))
			{
				str = "FlexEnd";
			}


			return (T)Enum.Parse(typeof(T), str);
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
