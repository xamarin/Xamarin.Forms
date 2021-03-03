﻿using Android.Content;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.ControlGallery.Android;
using Microsoft.Maui.Controls.ControlGallery.Issues;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Compatibility;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomHorizontalListview), typeof(HorizontalListviewRendererAndroid))]
namespace Microsoft.Maui.Controls.ControlGallery.Android
{
	public class HorizontalListviewRendererAndroid : ScrollViewRenderer
	{
		public HorizontalListviewRendererAndroid(Context context) : base(context)
		{
		}
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			var element = e.NewElement as CustomHorizontalListview;
			element?.Render();

			if (e.OldElement != null)
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;

			e.NewElement.PropertyChanged += OnElementPropertyChanged;

		}

		protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			
		}
	}
}