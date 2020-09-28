using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Platform
{
	public abstract class Layout : View, ILayout, IEnumerable<IView>
	{
		public IList<IView> Children { get; } = new List<IView>();

		public void Add(IView view)
		{
			if (view == null)
				return;

			Children.Add(view);
		}

		public IEnumerator<IView> GetEnumerator() => Children.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
	}
}
