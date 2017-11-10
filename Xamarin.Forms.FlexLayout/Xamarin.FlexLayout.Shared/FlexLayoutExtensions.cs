using System.Collections.Generic;

#if FORMS
using NativeView = Xamarin.Forms.View;

namespace Xamarin.Forms
#else
namespace Xamarin.FlexLayout
#endif
{
	internal static class FlexLayoutExtensions
	{
		public static bool IsLeaf(this NativeView view)
		{
			if (view.IsEnabled)
			{
				foreach (NativeView subview in GetChildren(view))
				{
					if (subview.IsEnabled && FlexLayout.GetIsIncluded(subview))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static IEnumerable<NativeView> GetChildren(NativeView view)
		{
			var result = new List<NativeView>();
			if (view is FlexLayout)
			{
				result.AddRange((view as FlexLayout).Children);
			}

			return result;
		}
	}
}
