using System.Collections.Generic;
using System.Linq;
using Android.Widget;
using AView = Android.Views.View;
using AViewGroup = Android.Views.ViewGroup;

namespace Xamarin.Forms.Platform.Android
{
	internal static class ViewGroupExtensions
	{
		internal static IEnumerable<T> GetChildrenOfType<T>(this AViewGroup self) where T : AView
		{
			for (var i = 0; i < self.ChildCount; i++)
			{
				AView child = self.GetChildAt(i);
				var typedChild = child as T;
				if (typedChild != null)
					yield return typedChild;

				if (child is AViewGroup)
				{
					IEnumerable<T> myChildren = (child as AViewGroup).GetChildrenOfType<T>();
					foreach (T nextChild in myChildren)
						yield return nextChild;
				}
			}
		}

		internal static void SetChildrenTextViewsFont(this AViewGroup self, Font font) 
		{
			var texts = self.GetChildrenOfType<TextView>();
			if (texts?.Count() > 0)
			{
				var fontFace = font.ToTypeface();
				var sizeSp = font.ToScaledPixel();
				foreach (var text in texts)
					text.SetFont(fontFace, sizeSp);
			}
		}
	}
}