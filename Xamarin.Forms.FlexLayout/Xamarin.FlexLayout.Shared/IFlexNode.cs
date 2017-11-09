
using System;
using System.Collections;
using System.Collections.Generic;

#if FORMS
namespace Xamarin.Forms
#else
namespace Xamarin.FlexLayout
#endif
{
	public interface IFlexNode : IEnumerable<IFlexNode>
	{
		bool IsDirty { get; }
		float LayoutTop { get; }
		float LayoutLeft { get; }
		float LayoutHeight { get; }
		float LayoutWidth { get; }

		float Top { get; set; }
		float Height { get; set; }
		float Left { get; set; }
		float Width { get; set; }
		float FlexGrow { get; set; }
		float FlexShrink { get; set; }
		float FlexBasis { get; set; }
		FlexPosition PositionType { get; set; }
		Align AlignItems { get; set; }
		Align AlignSelf { get; set; }
		Align AlignContent { get; set; }
		Wrap Wrap { get; set; }
		Overflow Overflow { get; set; }
		Justify JustifyContent { get; set; }
		FlexDirection FlexDirection { get; set; }
		float MarginLeft { get; set; }
		float MarginTop { get; set; }
		float MarginRight { get; set; }
		float MarginBottom { get; set; }
		float MinHeight { get; set; }
		float MinWidth { get; set; }

		void CalculateLayout();
		void Clear();
		void Insert(int i, IFlexNode subViewNode);
		void SetMeasure(MeasureFunc measureView);
		void MarkDirty();

		object Data { get; set; }
	}
}
