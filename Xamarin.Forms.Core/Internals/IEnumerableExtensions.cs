using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Forms
{
	internal static class IEnumerableExtensions
	{
		public static object GetItem(this IEnumerable e, int index)
		{
			var enumerator = e.GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext())
			{
				if (i == index)
					return enumerator.Current;
				i++;
			}
			return null;
		}

		public static int GetCount(this IEnumerable e)
		{
			if (e == null)
				return 0;
			var enumerator = e.GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext())
			{
				i++;
			}
			return i;
		}

		public static List<object> GetList(this IEnumerable e)
		{
			var enumerator = e.GetEnumerator();
			var list = new List<object>();
			while (enumerator.MoveNext())
			{
				list.Add(enumerator.Current);
			}
			return list;
		}
	}
}
