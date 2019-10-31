namespace Xamarin.Forms.Markup
{
	public static class ViewInFlexLayoutExtensions
	{
		public static TView AlignSelf<TView>(this TView view, FlexAlignSelf value) where TView : View
		{
			FlexLayout.SetAlignSelf(view, value);
			return view;
		}

		public static TView Basis<TView>(this TView view, FlexBasis value) where TView : View
		{
			FlexLayout.SetBasis(view, value);
			return view;
		}

		public static TView Grow<TView>(this TView view, float value) where TView : View
		{
			FlexLayout.SetGrow(view, value);
			return view;
		}

		public static TView Menu<TView>(this TView view, Menu value) where TView : View
		{
			FlexLayout.SetMenu(view, value);
			return view;
		}

		public static TView Order<TView>(this TView view, int value) where TView : View
		{
			FlexLayout.SetOrder(view, value);
			return view;
		}

		public static TView Shrink<TView>(this TView view, float value) where TView : View
		{
			FlexLayout.SetShrink(view, value);
			return view;
		}
	}
}
