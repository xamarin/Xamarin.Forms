using System;
using System.Collections;
using System.ComponentModel;

namespace Xamarin.Forms
{
	// TODO hartez 2018/06/23 13:42:22 Trying this out for a nicer read in OnElementPropertyChanged, not sure if I like it yet	
	public static class PropertyChangedEventArgsExtensions
	{
		public static bool Is(this PropertyChangedEventArgs args, BindableProperty property)
		{
			return args.PropertyName == property.PropertyName;
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

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			// TODO hartez 2018-05-22 12:42 PM Actually measure this

			// TODO hartez 2018-05-22 05:04 PM This 40,40 is what LV1 does; can we come up with something less arbitrary?
			var minimumSize = new Size(40, 40);

			// These are the rules if we don't have an ItemsTemplate; with a template, we may be able to 
			// work out better measurements

			// TODO hartez 2018-05-22 05:04 PM We need to check the itemslayout property here
			// If the layout is a vertical list, then we know our width should be the min of the screen width and the widthConstraint
			// if Horizontal, then height is the min of screenheight and heightConstraint
			// (assuming we don't allow overflow)

			// TODO hartez 2018-05-22 05:08 PM Whip up some unit tests to verify these rules

			var maxWidth = Math.Min(Device.Info.ScaledScreenSize.Width, widthConstraint);
			var maxHeight = Math.Min(Device.Info.ScaledScreenSize.Height, heightConstraint);
			

			Size request = new Size(maxWidth, maxHeight);

			return new SizeRequest(request, minimumSize);


			// TODO hartez 2018-05-22 05:07 PM Work out measurement heuristics if the ItemTemplate gives us sizes
		}
	}
}