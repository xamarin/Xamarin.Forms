using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Xamarin.Forms
{
	// TODO hartez 2018/06/23 13:42:22 Trying this out for a nicer read in OnElementPropertyChanged, not sure if I like it yet	
	public static class PropertyChangedEventArgsExtensions
	{
		public static bool Is(this PropertyChangedEventArgs args, BindableProperty property)
		{
			return args.PropertyName == property.PropertyName;
		}

		public static bool IsOneOf(this PropertyChangedEventArgs args, params BindableProperty[] properties)
		{
			for (int n = 0; n < properties.Length; n++)
			{
				if (args.PropertyName == properties[n].PropertyName)
				{
					return true;
				}
			}

			return false;
		}
	}

	public class ScrollToRequestEventArgs : EventArgs
	{
		public ScrollToMode Mode { get; }

		public ScrollToPosition ScrollToPosition { get; }
		public bool Animate { get; }

		public int Index { get; }
		public int GroupIndex { get; }

		public object Item { get; }
		public object Group { get; }

		public ScrollToRequestEventArgs(int index, int groupIndex, 
			ScrollToPosition scrollToPosition, bool animate)
		{
			Mode = ScrollToMode.Position;

			Index = index;
			GroupIndex = groupIndex;
			ScrollToPosition = scrollToPosition;
			Animate = animate;
		}

		public ScrollToRequestEventArgs(object item, object group, 
			ScrollToPosition scrollToPosition, bool animate)
		{
			Mode = ScrollToMode.Element;

			Item = item;
			Group = group;
			ScrollToPosition = scrollToPosition;
			Animate = animate;
		}
	}

	public class ItemsView : View
	{
		// TODO hartez 2018/06/24 11:37:00 Give DisplayMemberPath some thought	

		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ItemsView), null);

		public IEnumerable ItemsSource 
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		public static readonly BindableProperty ItemsLayoutProperty =
			BindableProperty.Create(nameof(ItemsLayout), typeof(IItemsLayout), typeof(ItemsView), 
				ListItemsLayout.VerticalList);

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