using System;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Controls.Issues;

namespace Xamarin.Forms.Controls
{
	public class ZoomableScrollView : ScrollView
	{
		public static readonly BindableProperty CurrentZoomProperty = 
			BindableProperty.Create(nameof(CurrentZoom), typeof(double), typeof(ZoomableScrollView), 1d);
		public static readonly BindableProperty MaximumZoomProperty = 
			BindableProperty.Create(nameof(MaxZoom), typeof(double), typeof(ZoomableScrollView), 1d);
		public static readonly BindableProperty IgnoreMinimumZoomProperty = 
			BindableProperty.Create(nameof(IgnoreMinimumZoom), typeof(bool), typeof(ZoomableScrollView), false);


		public double CurrentZoom
		{
			get { return (double)GetValue(CurrentZoomProperty); }
			set { SetValue(CurrentZoomProperty, value); }
		}

		public double MaxZoom
		{
			get { return (double)GetValue(MaximumZoomProperty); }
			set { SetValue(MaximumZoomProperty, value); }
		}

		public bool IgnoreMinimumZoom
		{
			get { return (bool)GetValue(IgnoreMinimumZoomProperty); }
			set { SetValue(IgnoreMinimumZoomProperty, value); }
		}
	}
}
