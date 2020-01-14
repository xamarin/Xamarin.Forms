using System;
using System.ComponentModel;

namespace Xamarin.Forms
{
	[TypeConverter(typeof(BrushTypeConverter))]
	public abstract class Brush : BindableObject
	{
		public static Brush Default
		{
			get { return new SolidColorBrush(Color.Default); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsEmpty
		{
			get
			{
				Type type = GetType();

				if (type == typeof(SolidColorBrush))
				{
					var solidColorBrush = (SolidColorBrush)this;
					return solidColorBrush == null || solidColorBrush.Color.IsDefault;
				}

				if (type == typeof(LinearGradientBrush))
				{
					var linearGradientBrush = (LinearGradientBrush)this;
					return linearGradientBrush == null || linearGradientBrush.GradientStops.Count == 0;
				}

				if (type == typeof(RadialGradientBrush))
				{
					var radialGradientBrush = (RadialGradientBrush)this;
					return radialGradientBrush == null || radialGradientBrush.GradientStops.Count == 0;
				}

				return true;
			}
		}
	}
}