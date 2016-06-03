using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms
{
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

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var listView = (this.Parent as ListView);
			if(View != null && listView != null && listView.HasUnevenRows && listView.CachingStrategy == ListViewCachingStrategy.RecycleElement)
				View.InvalidateMeasureInternal(Xamarin.Forms.Internals.InvalidationTrigger.RendererReady);
		}

		internal override ReadOnlyCollection<Element> LogicalChildren => _logicalChildren ?? base.LogicalChildren;
	}
}