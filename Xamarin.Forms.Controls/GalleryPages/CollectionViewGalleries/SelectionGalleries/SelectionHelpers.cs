using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.SelectionGalleries
{
	internal static class SelectionHelpers
	{
		public static string ToCommaSeparatedList(this IEnumerable<object> items)
		{
			if (items == null)
			{
				return string.Empty;
			}

			return items.Aggregate(string.Empty,
				(s, o) => s + (s.Length == 0 ? "" : ", ") + ((CollectionViewGalleryTestItem)o).Caption);
		}
	}
}