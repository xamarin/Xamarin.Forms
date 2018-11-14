using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	[ContentProperty("Items")]
	public class ShellItem : ShellGroupItem, IShellItemController, IElementConfiguration<ShellItem>
	{
		#region PropertyKeys

		private static readonly BindablePropertyKey ItemsPropertyKey = BindableProperty.CreateReadOnly(nameof(Items), typeof(ShellSectionCollection), typeof(ShellItem), null,
				defaultValueCreator: bo => new ShellSectionCollection { Inner = new ElementCollection<ShellSection>(((ShellItem)bo)._children) });

		#endregion PropertyKeys

		#region IShellItemController

		Task IShellItemController.GoToPart(List<string> parts, Dictionary<string, string> queryData)
		{
			var shellSectionRoute = parts[0];

			var items = Items;
			for (int i = 0; i < items.Count; i++)
			{
				var shellSection = items[i];
				if (Routing.CompareRoutes(shellSection.Route, shellSectionRoute, out var isImplicit))
				{
					Shell.ApplyQueryAttributes(shellSection, queryData, parts.Count == 1);

					if (CurrentItem != shellSection)
						SetValueFromRenderer(CurrentItemProperty, shellSection);

					if (!isImplicit)
						parts.RemoveAt(0);
					if (parts.Count > 0)
					{
						return ((IShellSectionController)shellSection).GoToPart(parts, queryData);
					}
					break;
				}
			}
			return Task.FromResult(true);
		}

		bool IShellItemController.ProposeSection(ShellSection shellSection, bool setValue)
		{
			var controller = (IShellController)Parent;

			if (controller == null)
				return false;

			bool accept = controller.ProposeNavigation(ShellNavigationSource.ShellSectionChanged,
				this,
				shellSection,
				shellSection?.CurrentItem,
				shellSection.Stack,
				true
			);

			if (accept && setValue)
				SetValueFromRenderer(CurrentItemProperty, shellSection);

			return accept;
		}

		#endregion IShellItemController

		public static readonly BindableProperty CurrentItemProperty =
			BindableProperty.Create(nameof(CurrentItem), typeof(ShellSection), typeof(ShellItem), null, BindingMode.TwoWay,
				propertyChanged: OnCurrentItemChanged);


		public static readonly BindableProperty ItemsProperty = ItemsPropertyKey.BindableProperty;

		private readonly ObservableCollection<Element> _children = new ObservableCollection<Element>();
		private ReadOnlyCollection<Element> _logicalChildren;
		private Lazy<PlatformConfigurationRegistry<ShellItem>> _platformConfigurationRegistry;

		public ShellItem()
		{
			((INotifyCollectionChanged)Items).CollectionChanged += ItemsCollectionChanged;
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<ShellItem>>(() => new PlatformConfigurationRegistry<ShellItem>(this));
		}

		public ShellSection CurrentItem
		{
			get { return (ShellSection)GetValue(CurrentItemProperty); }
			set { SetValue(CurrentItemProperty, value); }
		}

		public ShellSectionCollection Items => (ShellSectionCollection)GetValue(ItemsProperty);

		internal override ReadOnlyCollection<Element> LogicalChildrenInternal => _logicalChildren ?? (_logicalChildren = new ReadOnlyCollection<Element>(_children));

		internal void SendStructureChanged()
		{
			if (Parent is Shell shell)
			{
				shell.SendStructureChanged();
			}
		}

#if DEBUG
		[Obsolete ("Please dont use this in core code... its SUPER hard to debug when this happens", true)]
#endif
		public static implicit operator ShellItem(ShellSection shellSection)
		{
			var result = new ShellItem();

			result.Route = Routing.GenerateImplicitRoute(shellSection.Route);

			result.Items.Add(shellSection);
			result.SetBinding(TitleProperty, new Binding(nameof(Title), BindingMode.OneWay, source: shellSection));
			result.SetBinding(IconProperty, new Binding(nameof(Icon), BindingMode.OneWay, source: shellSection));
			result.SetBinding(FlyoutDisplayOptionsProperty, new Binding(nameof(FlyoutDisplayOptions), BindingMode.OneTime, source: shellSection));
			return result;
		}

#if DEBUG
		[Obsolete("Please dont use this in core code... its SUPER hard to debug when this happens", true)]
#endif
		public static implicit operator ShellItem(ShellContent shellContent) => (ShellSection)shellContent;

#if DEBUG
		[Obsolete("Please dont use this in core code... its SUPER hard to debug when this happens", true)]
#endif
		public static implicit operator ShellItem(TemplatedPage page) => (ShellSection)(ShellContent)page;

#if DEBUG
		[Obsolete("Please dont use this in core code... its SUPER hard to debug when this happens", true)]
#endif
		public static implicit operator ShellItem(MenuItem menuItem) => new MenuShellItem(menuItem);

		protected override void OnChildAdded(Element child)
		{
			base.OnChildAdded(child);
			if (CurrentItem == null)
				SetValueFromRenderer(CurrentItemProperty, child);
		}

		protected override void OnChildRemoved(Element child)
		{
			base.OnChildRemoved(child);
			if (CurrentItem == child)
			{
				if (Items.Count == 0)
					ClearValue(CurrentItemProperty);
				else
					SetValueFromRenderer(CurrentItemProperty, Items[0]);
			}
		}

		private static void OnCurrentItemChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var shellItem = (ShellItem)bindable;

			if (shellItem.Parent is IShellController shell)
			{
				shell.UpdateCurrentState(ShellNavigationSource.ShellSectionChanged);
			}

			shellItem.SendStructureChanged();
			((IShellController)shellItem?.Parent)?.AppearanceChanged(shellItem, false);
		}

		private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (Element element in e.NewItems)
					OnChildAdded(element);
			}

			if (e.OldItems != null)
			{
				foreach (Element element in e.OldItems)
					OnChildRemoved(element);
			}

			SendStructureChanged();
		}

		public IPlatformElementConfiguration<T, ShellItem> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		public class MenuShellItem : ShellItem
		{
			internal MenuShellItem(MenuItem menuItem)
			{
				MenuItem = menuItem;

				SetBinding(TitleProperty, new Binding("Text", BindingMode.OneWay, source: menuItem));
				SetBinding(IconProperty, new Binding("Icon", BindingMode.OneWay, source: menuItem));
			}

			public MenuItem MenuItem { get; }
		}
	}
}