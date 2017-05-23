using System;

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
			var shouldBeClickable = false;
			for (var i = 0; i < view.GestureRecognizers.Count; i++)
			{
				IGestureRecognizer gesture = view.GestureRecognizers[i];
				if (gesture is TapGestureRecognizer || gesture is PinchGestureRecognizer || gesture is PanGestureRecognizer)
				{
					shouldBeClickable = true;
					break;
				}
			}

			// Most layouts should be clickable, but not if they're in a ViewCell because that
			// interferes with the ViewCell's handlers
			if (!(view is Layout))
			{
				return shouldBeClickable;
			}

			// Walk up the view tree to ensure we aren't in a ViewCell
			Element parent = view.RealParent;
			while (parent != null)
			{
				if (parent is ViewCell)
				{
					// We're in a ViewCell, so clickable should be false
					return false;
				}
				parent = parent.RealParent;
			}

			// If we're not, we can be clickable
			return true;
		}
	}
}