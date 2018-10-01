using System;

namespace Xamarin.Forms
{
	public class ToolbarItem : MenuItem
	{
		static readonly BindableProperty OrderProperty = BindableProperty.Create("Order", typeof(ToolbarItemOrder), typeof(ToolbarItem), ToolbarItemOrder.Default, validateValue: (bo, o) =>
		{
			var order = (ToolbarItemOrder)o;
			return order == ToolbarItemOrder.Default || order == ToolbarItemOrder.Primary || order == ToolbarItemOrder.Secondary;
		});

		static readonly BindableProperty PriorityProperty = BindableProperty.Create("Priority", typeof(int), typeof(ToolbarItem), 0);

		public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create("IsVisible", typeof(bool), typeof(ToolbarItem), true, BindingMode.TwoWay, propertyChanged: (bindable, oldvalue, newvalue) =>
		{
			var item = bindable as ToolbarItem;

			if (item != null && item.Parent == null)
				return;

			if (item != null)
			{
				var items = ((Page)item.Parent).ToolbarItems;

				if ((bool)newvalue && !items.Contains(item))
				{
					items.Add(item);
				}
				else if (!(bool)newvalue && items.Contains(item))
				{
					items.Remove(item);
				}
			}
		});

		public ToolbarItem()
		{
	
		}

		public ToolbarItem(string name, string icon, Action activated, ToolbarItemOrder order = ToolbarItemOrder.Default, int priority = 0, bool isVisible = true)
		{
			if (activated == null)
				throw new ArgumentNullException("activated");

			Text = name;
			Icon = icon;
			Clicked += (s, e) => activated();
			Order = order;
			Priority = priority;
			IsVisible = isVisible;
		}

		[Obsolete("Name is obsolete as of version 1.3.0. Please use Text instead.")]
		public string Name
		{
			get { return Text; }
			set { Text = value; }
		}

		public ToolbarItemOrder Order
		{
			get { return (ToolbarItemOrder)GetValue(OrderProperty); }
			set { SetValue(OrderProperty, value); }
		}

		public int Priority
		{
			get { return (int)GetValue(PriorityProperty); }
			set { SetValue(PriorityProperty, value); }
		}

		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		[Obsolete("Activated is obsolete as of version 1.3.0. Please use Clicked instead.")]
		public event EventHandler Activated
		{
			add { Clicked += value; }
			remove { Clicked -= value; }
		}
	}
}