using System.Linq;
using System.Collections;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.SelectionGalleries
{
	internal static class SelectionHelpers
	{
		public static string ToCommaSeparatedList(this IEnumerable items)
		{
			if (items == null)
			{
				return string.Empty;
			}

			return string.Join(", ", items.Cast<CollectionViewGalleryTestItem>().Select(i => i.Caption));
		}
	}
}