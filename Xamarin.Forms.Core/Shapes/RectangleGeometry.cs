namespace Xamarin.Forms.Shapes
{
    public class RectangleGeometry : Geometry
    {
        public static readonly BindableProperty RectProperty =
            BindableProperty.Create(nameof(Rect), typeof(Rectangle), typeof(RectangleGeometry), new Rectangle());

        public Rectangle Rect
        {
            set { SetValue(RectProperty, value); }
            get { return (Rectangle)GetValue(RectProperty); }
        }
    }
}