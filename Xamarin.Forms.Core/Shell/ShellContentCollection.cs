using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Xamarin.Forms
{
	internal sealed class ShellContentCollection : ShellAbstractCollection<ShellContent>
	{
		public event NotifyCollectionChangedEventHandler VisibleItemsChangedInternal;

		readonly List<NotifyCollectionChangedEventArgs> _notifyCollectionChangedEventArgs;
		bool _pauseCollectionChanged;

		public ShellContentCollection() : base()
		{
			_notifyCollectionChangedEventArgs = new List<NotifyCollectionChangedEventArgs>();
		}

		void PauseCollectionChanged() => _pauseCollectionChanged = true;

		void ResumeCollectionChanged()
		{
			_pauseCollectionChanged = false;

			var pendingEvents = _notifyCollectionChangedEventArgs.ToList();
			_notifyCollectionChangedEventArgs.Clear();

			foreach (var args in pendingEvents)
				OnVisibleItemsChanged(this, args);
		}

		void OnIsPageVisibleChanged(object sender, EventArgs e)
		{
			CheckVisibility((ShellContent)sender);
		}

		protected override void OnVisibleItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (_pauseCollectionChanged)
			{
				_notifyCollectionChangedEventArgs.Add(args);
				return;
			}

			base.OnVisibleItemsChanged(sender, args);
			VisibleItemsChangedInternal?.Invoke(VisibleItems, args);
		}

		protected override void CheckVisibility(ShellContent shellContent)
		{
			if (shellContent is IShellContentController controller)
			{
				// Assume incoming page will be visible
				if (controller.Page == null || controller.Page.IsVisible)
				{
					if (_visibleContents.Contains(shellContent))
						return;

					int visibleIndex = 0;
					for (var i = 0; i < Inner.Count; i++)
					{
						var item = Inner[i];

						if (item == shellContent)
						{
							_visibleContents.Insert(visibleIndex, shellContent);
							break;
						}

						visibleIndex++;
					}
				}
				else
				{
					_visibleContents.Remove(shellContent);
				}
			}
			else if (_visibleContents.Contains(shellContent))
			{
				_visibleContents.Remove(shellContent);
			}
		}

		protected override void OnElementControllerInserting(IElementController element)
		{
			if (element is IShellContentController controller)
				controller.IsPageVisibleChanged += OnIsPageVisibleChanged;
		}

		protected override void OnElementControllerRemoving(IElementController element)
		{
			if (element is IShellContentController controller)
				controller.IsPageVisibleChanged -= OnIsPageVisibleChanged;
		}

		protected override void RemoveInnerCollection()
		{
			try
			{
				PauseCollectionChanged();
				base.RemoveInnerCollection();
			}
			finally
			{
				ResumeCollectionChanged();
			}
		}
	}
}