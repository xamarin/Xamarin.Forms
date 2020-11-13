using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using ElmSharp;
using Size = Xamarin.Forms.Size;
using Rectangle = Xamarin.Forms.Rectangle;

namespace Xamarin.Platform.Tizen
{
	public class Canvas : Box, IContainable<EvasObject>
	{
		public Canvas(EvasObject parent) : base(parent)
		{
			Initilize();
		}

		readonly ObservableCollection<EvasObject> _children = new ObservableCollection<EvasObject>();

		public new IList<EvasObject> Children
		{
			get
			{
				return _children;
			}
		}

		protected override void OnUnrealize()
		{
			foreach (var child in _children)
			{
				child.Unrealize();
			}

			base.OnUnrealize();
		}

		void Initilize()
		{
			_children.CollectionChanged += (o, e) =>
			{
				if (e.Action == NotifyCollectionChangedAction.Add)
				{
					foreach (var v in e.NewItems)
					{
						var view = v as EvasObject;
						if (null != view)
						{
							OnAdd(view);
						}
					}
				}
				else if (e.Action == NotifyCollectionChangedAction.Remove)
				{
					foreach (var v in e.OldItems)
					{
						var view = v as EvasObject;
						if (null != view)
						{
							OnRemove(view);
						}
					}
				}
				else if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					OnRemoveAll();
				}
			};
		}

		void OnAdd(EvasObject view)
		{
			PackEnd(view);
			// TODO : Currently, all native views are temporarily forced to show. 
			// If IFrameworkElement.IsVisible is defined later, Show()/Hide() will be determined accordingly.
			view.Show();
		}

		void OnRemove(EvasObject view)
		{
			UnPack(view);
		}

		void OnRemoveAll()
		{
			UnPackAll();
		}

		internal Func<double, double, Size>? CrossPlatformMeasure { get; set; }
		internal Action<Rectangle>? CrossPlatformArrange { get; set; }
	}
}
