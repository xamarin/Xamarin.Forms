using System;
using System.Diagnostics;

namespace Xamarin.Forms.Platform.Android
{
	public static class VisualElementExtensions
	{
		public static IVisualElementRenderer GetRenderer(this VisualElement self)
		{
			if (self == null)
				throw new ArgumentNullException("self");

			IVisualElementRenderer renderer = Platform.GetRenderer(self);

			return renderer;
		}

		public static bool ShouldBeMadeClickable(this View view)
		{
            for (var i = 0; i < view.GestureRecognizers.Count; i++)
            {
                IGestureRecognizer gesture = view.GestureRecognizers[i];
                if (gesture is TapGestureRecognizer || gesture is PinchGestureRecognizer || gesture is PanGestureRecognizer)
                {
                    return true;
                }
            }

            return false;
		}

		static bool IsInViewCell(this View view)
		{
			Element parent = view.RealParent;
			while (parent != null)
			{
				if (parent is ViewCell)
				{
					return true;
				}
				parent = parent.RealParent;
			}

			return false;
		}
	}
}