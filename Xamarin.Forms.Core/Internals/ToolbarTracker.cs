﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ToolbarTracker
	{
		int _masterDetails;
		Page _target;
		ToolBarItemComparer _toolBarItemComparer;
		public ToolbarTracker()
		{
			_toolBarItemComparer = new ToolBarItemComparer();
		}

		public IEnumerable<Page> AdditionalTargets { get; set; }

		public bool HaveMasterDetail
		{
			get { return _masterDetails > 0; }
		}

		public bool SeparateMasterDetail { get; set; }

		public Page Target
		{
			get { return _target; }
			set
			{
				if (_target == value)
					return;

				UntrackTarget(_target);
				_target = value;

				if (_target != null)
					TrackTarget(_target);
				EmitCollectionChanged();
			}
		}

		public IEnumerable<ToolbarItem> ToolbarItems
		{
			get
			{
				if (Target == null)
					return new ToolbarItem[0];

				// I realize this is sorting on every single get but we don't have 
				// a mechanism in place currently to invalidate a stored version of this

				List<ToolbarItem> returnValue = GetCurrentToolbarItems(Target);

				if (AdditionalTargets != null)
					foreach(var item in AdditionalTargets)
						foreach(var toolbarItem in item.ToolbarItems)
							returnValue.Add(toolbarItem);

				returnValue.Sort(_toolBarItemComparer);
				return returnValue;
			}
		}

		public event EventHandler CollectionChanged;

		void EmitCollectionChanged()
			=> CollectionChanged?.Invoke(this, EventArgs.Empty);

		List<ToolbarItem> GetCurrentToolbarItems(Page page)
		{
			var result = new List<ToolbarItem>();
			result.AddRange(page.ToolbarItems);

			if (page is MasterDetailPage)
			{
				var masterDetail = (MasterDetailPage)page;
				if (SeparateMasterDetail)
				{
					if (masterDetail.IsPresented)
					{
						if (masterDetail.Master != null)
							result.AddRange(GetCurrentToolbarItems(masterDetail.Master));
					}
					else
					{
						if (masterDetail.Detail != null)
							result.AddRange(GetCurrentToolbarItems(masterDetail.Detail));
					}
				}
				else
				{
					if (masterDetail.Master != null)
						result.AddRange(GetCurrentToolbarItems(masterDetail.Master));
					if (masterDetail.Detail != null)
						result.AddRange(GetCurrentToolbarItems(masterDetail.Detail));
				}
			}
			else if (page is IPageContainer<Page>)
			{
				var container = (IPageContainer<Page>)page;
				if (container.CurrentPage != null)
					result.AddRange(GetCurrentToolbarItems(container.CurrentPage));
			}

			return result;
		}

		void OnChildAdded(object sender, ElementEventArgs eventArgs)
		{
			var page = eventArgs.Element as Page;
			if (page == null)
				return;

			RegisterChildPage(page);
		}

		void OnChildRemoved(object sender, ElementEventArgs eventArgs)
		{
			var page = eventArgs.Element as Page;
			if (page == null)
				return;

			UnregisterChildPage(page);
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			EmitCollectionChanged();
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == NavigationPage.CurrentPageProperty.PropertyName || propertyChangedEventArgs.PropertyName == MasterDetailPage.IsPresentedProperty.PropertyName ||
				propertyChangedEventArgs.PropertyName == "Detail" || propertyChangedEventArgs.PropertyName == "Master")
			{
				EmitCollectionChanged();
			}
		}

		void RegisterChildPage(Page page)
		{
			if (page is MasterDetailPage)
				_masterDetails++;

			((ObservableCollection<ToolbarItem>)page.ToolbarItems).CollectionChanged += OnCollectionChanged;
			page.PropertyChanged += OnPropertyChanged;
		}

		void TrackTarget(Page page)
		{
			if (page == null)
				return;

			if (page is MasterDetailPage)
				_masterDetails++;

			((ObservableCollection<ToolbarItem>)page.ToolbarItems).CollectionChanged += OnCollectionChanged;
			page.Descendants().OfType<Page>().ForEach(RegisterChildPage);

			page.DescendantAdded += OnChildAdded;
			page.DescendantRemoved += OnChildRemoved;
			page.PropertyChanged += OnPropertyChanged;
		}

		void UnregisterChildPage(Page page)
		{
			if (page is MasterDetailPage)
				_masterDetails--;

			((ObservableCollection<ToolbarItem>)page.ToolbarItems).CollectionChanged -= OnCollectionChanged;
			page.PropertyChanged -= OnPropertyChanged;
		}

		void UntrackTarget(Page page)
		{
			if (page == null)
				return;

			if (page is MasterDetailPage)
				_masterDetails--;

			((ObservableCollection<ToolbarItem>)page.ToolbarItems).CollectionChanged -= OnCollectionChanged;
			page.Descendants().OfType<Page>().ForEach(UnregisterChildPage);

			page.DescendantAdded -= OnChildAdded;
			page.DescendantRemoved -= OnChildRemoved;
			page.PropertyChanged -= OnPropertyChanged;
		}

		class ToolBarItemComparer : IComparer<ToolbarItem>
		{
			public int Compare(ToolbarItem x, ToolbarItem y) => x.Priority.CompareTo(y.Priority);
		}
	}
}