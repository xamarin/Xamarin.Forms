﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

#if NETSTANDARD1_0
using System.Linq;
#endif

using System.Reflection;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	[ContentProperty("Content")]
	public class ShellContent : BaseShellItem, IShellContentController
	{
		static readonly BindablePropertyKey MenuItemsPropertyKey =
			BindableProperty.CreateReadOnly(nameof(MenuItems), typeof(MenuItemCollection), typeof(ShellContent), null,
				defaultValueCreator: bo => new MenuItemCollection());

		public static readonly BindableProperty MenuItemsProperty = MenuItemsPropertyKey.BindableProperty;

		public static readonly BindableProperty ContentProperty =
			BindableProperty.Create(nameof(Content), typeof(object), typeof(ShellContent), null, BindingMode.OneTime, propertyChanged: OnContentChanged);

		public static readonly BindableProperty ContentTemplateProperty =
			BindableProperty.Create(nameof(ContentTemplate), typeof(DataTemplate), typeof(ShellContent), null, BindingMode.OneTime);

		public MenuItemCollection MenuItems => (MenuItemCollection)GetValue(MenuItemsProperty);

		public object Content {
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		public DataTemplate ContentTemplate {
			get => (DataTemplate)GetValue(ContentTemplateProperty);
			set => SetValue(ContentTemplateProperty, value);
		}

		Page IShellContentController.Page => ContentCache;

		Page IShellContentController.GetOrCreateContent()
		{
			var template = ContentTemplate;
			var content = Content;

			Page result = null;
			if (template == null)
			{
				if (content is Page page)
					result = page;
			}
			else
			{
				result = ContentCache ?? (Page)template.CreateContent(content, this);
				ContentCache = result;
			}

			if (result != null && result.Parent != this)
				OnChildAdded(result);


			if (_delayedQueryParams != null && result  != null) {
				ApplyQueryAttributes(result, _delayedQueryParams);
				_delayedQueryParams = null;
			}

			return result;
		}

		void IShellContentController.RecyclePage(Page page)
		{
			if (ContentCache == page)
			{
				OnChildRemoved(page);
				ContentCache = null;
			}
		}

		Page _contentCache;
		IList<Element> _logicalChildren = new List<Element>();
		ReadOnlyCollection<Element> _logicalChildrenReadOnly;

		public ShellContent()
		{
			((INotifyCollectionChanged)MenuItems).CollectionChanged += MenuItemsCollectionChanged;
		}


		internal override ReadOnlyCollection<Element> LogicalChildrenInternal => _logicalChildrenReadOnly ?? (_logicalChildrenReadOnly = new ReadOnlyCollection<Element>(_logicalChildren));

		Page ContentCache {
			get { return _contentCache; }
			set
			{
				_contentCache = value;
				if (Parent != null)
					((ShellSection)Parent).UpdateDisplayedPage();
			}
		}

		public static implicit operator ShellContent(TemplatedPage page)
		{
			var shellContent = new ShellContent();

			var pageRoute = Routing.GetRoute(page);

			shellContent.Route = Routing.GenerateImplicitRoute(pageRoute);

			shellContent.Content = page;
			shellContent.SetBinding(TitleProperty, new Binding("Title", BindingMode.OneWay, source: page));
			shellContent.SetBinding(IconProperty, new Binding("Icon", BindingMode.OneWay, source: page));

			return shellContent;
		}

		static void OnContentChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var shellContent = (ShellContent)bindable;
			// This check is wrong but will work for testing
			if (shellContent.ContentTemplate == null)
			{
				// deparent old item
				if (oldValue is Page oldElement)
				{
					shellContent.OnChildRemoved(oldElement);
					shellContent.ContentCache = null;
				}

				// make sure LogicalChildren collection stays consisten
				shellContent._logicalChildren.Clear();
				if (newValue is Page newElement)
				{
					shellContent._logicalChildren.Add((Element)newValue);
					shellContent.ContentCache = newElement;
					// parent new item
					shellContent.OnChildAdded(newElement);
				}
			}

			if (shellContent.Parent?.Parent is ShellItem shellItem)
				shellItem?.SendStructureChanged();
		}

		void MenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (Element el in e.NewItems)
					OnChildAdded(el);

			if (e.OldItems != null)
				foreach (Element el in e.OldItems)
					OnChildRemoved(el);
		}

		IDictionary<string, string> _delayedQueryParams;
		internal override void ApplyQueryAttributes(IDictionary<string, string> query)
		{
			base.ApplyQueryAttributes(query);
			ApplyQueryAttributes(this, query);

			if (Content == null) {
				_delayedQueryParams = query;
				return;
			}
			ApplyQueryAttributes(Content as Page, query);
		}

		internal static void ApplyQueryAttributes(object content, IDictionary<string, string> query)
		{
			if (content is IQueryAttributable attributable)
				attributable.ApplyQueryAttributes(query);

			if (content is BindableObject bindable && bindable.BindingContext != null && content != bindable.BindingContext)
				ApplyQueryAttributes(bindable.BindingContext, query);

			var type = content.GetType();
			var typeInfo = type.GetTypeInfo();
#if NETSTANDARD1_0
			var queryPropertyAttributes = typeInfo.GetCustomAttributes(typeof(QueryPropertyAttribute), true).ToArray();
#else
			var queryPropertyAttributes = typeInfo.GetCustomAttributes(typeof(QueryPropertyAttribute), true);
#endif

			if (queryPropertyAttributes.Length == 0)
				return;

			foreach (QueryPropertyAttribute attrib in queryPropertyAttributes) {
				if (query.TryGetValue(attrib.QueryId, out var value)) {
					PropertyInfo prop = type.GetRuntimeProperty(attrib.Name);

					if (prop != null && prop.CanWrite && prop.SetMethod.IsPublic)
						prop.SetValue(content, value);
				}
			}
		}
	}
}