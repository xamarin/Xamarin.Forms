﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public class ItemsView : View
	{
		List<Element> _logicalChildren = new List<Element>();

		protected internal ItemsView()
		{
			CollectionView.VerifyCollectionViewFlagEnabled(constructorHint: nameof(ItemsView));
		}

		public static readonly BindableProperty EmptyViewProperty =
			BindableProperty.Create(nameof(EmptyView), typeof(object), typeof(ItemsView), null);

		public object EmptyView
		{
			get => GetValue(EmptyViewProperty);
			set => SetValue(EmptyViewProperty, value);
		}

		public static readonly BindableProperty EmptyViewTemplateProperty =
			BindableProperty.Create(nameof(EmptyViewTemplate), typeof(DataTemplate), typeof(ItemsView), null);

		public DataTemplate EmptyViewTemplate
		{
			get => (DataTemplate)GetValue(EmptyViewTemplateProperty);
			set => SetValue(EmptyViewTemplateProperty, value);
		}

		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ItemsView), null);

		public IEnumerable ItemsSource 
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		public static readonly BindableProperty HorizontalScrollBarVisibilityProperty = BindableProperty.Create(
			nameof(HorizontalScrollBarVisibility),
			typeof(ScrollBarVisibility),
			typeof(ItemsView),
			ScrollBarVisibility.Default);

		public ScrollBarVisibility HorizontalScrollBarVisibility
		{
			get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
			set => SetValue(HorizontalScrollBarVisibilityProperty, value);
		}


		public static readonly BindableProperty VerticalScrollBarVisibilityProperty = BindableProperty.Create(
			nameof(VerticalScrollBarVisibility),
			typeof(ScrollBarVisibility),
			typeof(ItemsView),
			ScrollBarVisibility.Default);

		public ScrollBarVisibility VerticalScrollBarVisibility
		{
			get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
			set => SetValue(VerticalScrollBarVisibilityProperty, value);
		}

		public void AddLogicalChild(Element element)
		{
			_logicalChildren.Add(element);

			PropertyPropagationExtensions.PropagatePropertyChanged(null, element);

			element.Parent = this;
		}

		public void RemoveLogicalChild(Element element)
		{
			element.Parent = null;
			_logicalChildren.Remove(element);
		}

#if NETSTANDARD1_0
		ReadOnlyCollection<Element> _readOnlyLogicalChildren;
		internal override ReadOnlyCollection<Element> LogicalChildrenInternal => _readOnlyLogicalChildren ?? 
			(_readOnlyLogicalChildren = new ReadOnlyCollection<Element>(_logicalChildren));
#else
		internal override ReadOnlyCollection<Element> LogicalChildrenInternal => _logicalChildren.AsReadOnly();
#endif

		public static readonly BindableProperty ItemsLayoutProperty =
			BindableProperty.Create(nameof(ItemsLayout), typeof(IItemsLayout), typeof(ItemsView), 
				ListItemsLayout.Vertical);

		public IItemsLayout ItemsLayout
		{
			get => (IItemsLayout)GetValue(ItemsLayoutProperty);
			set => SetValue(ItemsLayoutProperty, value);
		}

		public static readonly BindableProperty ItemTemplateProperty =
			BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ItemsView));

		public DataTemplate ItemTemplate
		{
			get => (DataTemplate)GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		public static readonly BindableProperty ItemSizingStrategyProperty =
			BindableProperty.Create(nameof(ItemSizingStrategy), typeof(ItemSizingStrategy), typeof(ItemsView));

		public ItemSizingStrategy ItemSizingStrategy
		{
			get => (ItemSizingStrategy)GetValue(ItemSizingStrategyProperty);
			set => SetValue(ItemSizingStrategyProperty, value);
		}

		public static readonly BindableProperty ItemsUpdatingScrollModeProperty =
			BindableProperty.Create(nameof(ItemsUpdatingScrollMode), typeof(ItemsUpdatingScrollMode), typeof(ItemsView),
				default(ItemsUpdatingScrollMode));

		public ItemsUpdatingScrollMode ItemsUpdatingScrollMode
		{
			get => (ItemsUpdatingScrollMode)GetValue(ItemsUpdatingScrollModeProperty);
			set => SetValue(ItemsUpdatingScrollModeProperty, value);
		}

		public void ScrollTo(int index, int groupIndex = -1,
			ScrollToPosition position = ScrollToPosition.MakeVisible, bool animate = true)
		{
			OnScrollToRequested(new ScrollToRequestEventArgs(index, groupIndex, position, animate));
		}

		public void ScrollTo(object item, object group = null,
			ScrollToPosition position = ScrollToPosition.MakeVisible, bool animate = true)
		{
			OnScrollToRequested(new ScrollToRequestEventArgs(item, group, position, animate));
		}

		public event EventHandler<ScrollToRequestEventArgs> ScrollToRequested;

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			// TODO hartez 2018-05-22 05:04 PM This 40,40 is what LV1 does; can we come up with something less arbitrary?
			var minimumSize = new Size(40, 40);

			var maxWidth = Math.Min(Device.Info.ScaledScreenSize.Width, widthConstraint);
			var maxHeight = Math.Min(Device.Info.ScaledScreenSize.Height, heightConstraint);

			Size request = new Size(maxWidth, maxHeight);

			return new SizeRequest(request, minimumSize);
		}

		protected virtual void OnScrollToRequested(ScrollToRequestEventArgs e)
		{
			ScrollToRequested?.Invoke(this, e);
		}
	}
}