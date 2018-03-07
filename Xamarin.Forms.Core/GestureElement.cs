using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public class GestureElement : Element, ISpatialElement
	{
		readonly GestureRecognizerCollection _gestureRecognizers = new GestureRecognizerCollection();

		public GestureElement()
		{
			_gestureRecognizers.CollectionChanged += (sender, args) =>
			{
				switch (args.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (IElement item in args.NewItems.OfType<IElement>())
						{
							ValidateGesture(item as IGestureRecognizer);
							item.Parent = this;
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (IElement item in args.OldItems.OfType<IElement>())
							item.Parent = null;
						break;
					case NotifyCollectionChangedAction.Replace:
						foreach (IElement item in args.NewItems.OfType<IElement>())
						{
							ValidateGesture(item as IGestureRecognizer);
							item.Parent = this;
						}
						foreach (IElement item in args.OldItems.OfType<IElement>())
							item.Parent = null;
						break;
					case NotifyCollectionChangedAction.Reset:
						foreach (IElement item in _gestureRecognizers.OfType<IElement>())
							item.Parent = this;
						break;
				}
			};
		}
		
		Region ISpatialElement.Region { get; set; }
		
		public IList<IGestureRecognizer> GestureRecognizers
		{
			get { return _gestureRecognizers; }
		}
		
		internal virtual void ValidateGesture(IGestureRecognizer gesture) { }

		class GestureRecognizerCollection : ObservableCollection<IGestureRecognizer>
		{
			protected override void ClearItems()
			{
				List<IGestureRecognizer> removed = new List<IGestureRecognizer>(this);
				base.ClearItems();
				base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
			}
		}
	}
}
