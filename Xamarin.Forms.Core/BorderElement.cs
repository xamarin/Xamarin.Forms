using System;

namespace Xamarin.Forms
{
	static class BorderElement
	{
		const int DefaultBorderRadius = 5;
		const int DefaultCornerRadius = -1;

		public static readonly BindableProperty BorderColorProperty =
			BindableProperty.Create("BorderColor", typeof(Color), typeof(IBorderElement), Color.Default,
									propertyChanged: OnBorderColorPropertyChanged);

		public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create("BorderWidth", typeof(double), typeof(IBorderElement), -1d);

		[Obsolete("BorderRadiusProperty is obsolete as of 2.5.0. Please use CornerRadius instead.")]
		public static readonly BindableProperty BorderRadiusProperty = BindableProperty.Create("BorderRadius", typeof(int), typeof(IBorderElement), defaultValue: DefaultBorderRadius,
			propertyChanged: BorderRadiusPropertyChanged);

		public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create("CornerRadius", typeof(int), typeof(IBorderElement), defaultValue: DefaultCornerRadius,
			propertyChanged: CornerRadiusPropertyChanged);

		static void OnBorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((IBorderElement)bindable).OnBorderColorPropertyChanged((Color)oldValue, (Color)newValue);
		}


		static void BorderRadiusPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (newvalue == oldvalue)
				return;

			var borderElement = (IBorderElement)bindable;
			var borderOrCornerRadius = (bindable as IBorderOrCornerRadius);

			var val = (int)newvalue;
			if (val == DefaultBorderRadius && (!borderOrCornerRadius?.cornerOrBorderRadiusSetting ?? true))
				val = DefaultCornerRadius;

			var oldVal = (int)bindable.GetValue(Button.CornerRadiusProperty);

			if (oldVal == val)
				return;

			if (borderOrCornerRadius != null)
				borderOrCornerRadius.cornerOrBorderRadiusSetting = true;

			bindable.SetValue(Button.CornerRadiusProperty, val);

			if (borderOrCornerRadius != null)
				borderOrCornerRadius.cornerOrBorderRadiusSetting = false;
		}

		static void CornerRadiusPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (newvalue == oldvalue)
				return;

			var borderElement = (IBorderElement)bindable;
			var borderOrCornerRadius = (bindable as IBorderOrCornerRadius);

			var val = (int)newvalue;

			if (val == DefaultCornerRadius && (!borderOrCornerRadius?.cornerOrBorderRadiusSetting ?? true))
				val = DefaultBorderRadius;

#pragma warning disable 0618 // retain until BorderRadiusProperty removed
			var oldVal = (int)bindable.GetValue(Button.BorderRadiusProperty);
#pragma warning restore

			if (oldVal == val)
				return;

#pragma warning disable 0618 // retain until BorderRadiusProperty removed
			if (borderOrCornerRadius != null)
				borderOrCornerRadius.cornerOrBorderRadiusSetting = true;

			bindable.SetValue(BorderElement.BorderRadiusProperty, val);

			if (borderOrCornerRadius != null)
				borderOrCornerRadius.cornerOrBorderRadiusSetting = false;
#pragma warning restore
		}
	}
}