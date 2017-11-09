#if FORMS
namespace Xamarin.Forms
{
	public delegate Size MeasureFunc(IFlexNode node, float width, FlexMeasureMode widthMode, float height, FlexMeasureMode heightMode);
}
#else
namespace Xamarin.FlexLayout
{
	public delegate object MeasureFunc(IFlexNode node, float width, FlexMeasureMode widthMode, float height, FlexMeasureMode heightMode);
}

#endif
