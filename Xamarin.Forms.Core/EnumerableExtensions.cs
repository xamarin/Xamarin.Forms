using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections.Specialized;

namespace Xamarin.Forms
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> GetGesturesFor<T>(this IEnumerable<IGestureRecognizer> gestures, Func<T, bool> predicate = null) where T : GestureRecognizer
		{
			if (gestures == null)
				yield break;

			if (predicate == null)
				predicate = x => true;

			foreach (IGestureRecognizer item in gestures)
			{
				var gesture = item as T;
				if (gesture != null && predicate(gesture))
				{
					yield return gesture;
				}
			}
		}

		internal static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T item)
		{
			foreach (T x in enumerable)
				yield return x;

			yield return item;
		}

		internal static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
		{
			foreach (T item in enumeration)
			{
				action(item);
			}
		}

		internal static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
		{
			if (enumerable == null)
				throw new ArgumentNullException("enumerable");

			var i = 0;
			foreach (T element in enumerable)
			{
				if (Equals(element, item))
					return i;

				i++;
			}

			return -1;
		}

		internal static int IndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
		{
			var i = 0;
			foreach (T element in enumerable)
			{
				if (predicate(element))
					return i;

				i++;
			}

			return -1;
		}

		internal static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, T item)
		{
			yield return item;

			foreach (T x in enumerable)
				yield return x;
		}

		internal static IReadOnlyList<object> ToReadOnlyList(this IEnumerable enumerable)
		{
			var readOnlyList = enumerable as IReadOnlyList<object>;
			if (readOnlyList != null)
				return readOnlyList;

			var list = enumerable as IList;
			if (list != null)
				return new ListAsReadOnlyList(list);

			var objectList = enumerable as IList<object>;
			if (objectList != null)
				return new GenericListAsReadOnlyList<object>(objectList);

			// allow IList<AnyType> without falling through to the array copy below
			var typedList = (IReadOnlyList<object>)(
				from iface in enumerable.GetType().GetTypeInfo().ImplementedInterfaces
				where iface.Name == typeof(IList<>).Name && iface.GetGenericTypeDefinition() == typeof(IList<>)
				let type = typeof(GenericListAsReadOnlyList<>).MakeGenericType(iface.GenericTypeArguments[0])
				select Activator.CreateInstance(type, enumerable)
			).FirstOrDefault();
			if (typedList != null)
				return typedList;

			// ToArray instead of ToList to save memory
			return enumerable.Cast<object>().ToArray();
		}

		class ListAsReadOnlyList : IReadOnlyList<object>
		{
			IList _list;

			internal ListAsReadOnlyList(IList list)
			{
				_list = list;
			}

			public object this[int index] => _list[index];
			public int Count => _list.Count;
			public IEnumerator<object> GetEnumerator() => _list.Cast<object>().GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
		class GenericListAsReadOnlyList<T> : IReadOnlyList<object>
		{
			IList<T> _list;

			public GenericListAsReadOnlyList(IList<T> list)
			{
				_list = list;
			}

			public object this[int index] => _list[index];
			public int Count => _list.Count;
			public IEnumerator<object> GetEnumerator() => _list.Cast<object>().GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}