using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms
{
	// TODO hartez 2017/07/11 20:24:59 Get a count going on this thing, see if maybe it's holding up CellControl or vice versa	(check for roots)
	[ContentProperty("View")]
	public class ViewCell : Cell
	{
		ReadOnlyCollection<Element> _logicalChildren;

		View _view;

		public View View
		{
			get { return _view; }
			set
			{
				if (_view == value)
					return;

				OnPropertyChanging();

				if (_view != null)
				{
					OnChildRemoved(_view);
					_view.ComputedConstraint = LayoutConstraint.None;
				}

				_view = value;

				if (_view != null)
				{
					_view.ComputedConstraint = LayoutConstraint.Fixed;
					OnChildAdded(_view);
					_logicalChildren = new ReadOnlyCollection<Element>(new List<Element>(new[] { View }));
				}
				else
				{
					_logicalChildren = null;
				}

				OnPropertyChanged();
			}
		}

		internal override ReadOnlyCollection<Element> LogicalChildrenInternal => _logicalChildren ?? base.LogicalChildrenInternal;
	}
}