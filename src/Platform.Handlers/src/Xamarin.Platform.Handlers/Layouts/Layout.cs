using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Platform
{
	public abstract class Layout : View, ILayout, IEnumerable<IView>
	{
		readonly List<IView> _children = new List<IView>();

		public IReadOnlyList<IView> Children { get => _children.AsReadOnly(); }

		public void Add(IView view)
		{
			if (view == null)
				return;

			_children.Add(view);

			InvalidateMeasure();
		}

		public IEnumerator<IView> GetEnumerator() => _children.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();
	}
}
