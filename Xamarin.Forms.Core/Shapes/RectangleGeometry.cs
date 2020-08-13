﻿namespace Xamarin.Forms.Shapes
{
    public class RectangleGeometry : Geometry
    {
        public static readonly BindableProperty RectProperty =
            BindableProperty.Create(nameof(Rect), typeof(Rect), typeof(RectangleGeometry), new Rect());

        public Rect Rect
        {
            set { SetValue(RectProperty, value); }
            get { return (Rect)GetValue(RectProperty); }
        }
    }
}