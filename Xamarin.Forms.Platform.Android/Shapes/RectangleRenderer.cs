using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Rect = Xamarin.Forms.Shapes.Rectangle;
using APath = Android.Graphics.Path;

namespace Xamarin.Forms.Platform.Android
{
	public class RectangleRenderer : ShapeRenderer<Rect, RectView>
    {
        public RectangleRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Rect> args)
        {
            if (Control == null)
            {
                SetNativeControl(new RectView(Context));
            }

            base.OnElementChanged(args);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.IsOneOf(VisualElement.HeightProperty, VisualElement.WidthProperty))
                UpdateRadius();
            else if (args.PropertyName == Rect.RadiusXProperty.PropertyName)
                UpdateRadiusX();
            else if (args.PropertyName == Rect.RadiusYProperty.PropertyName)
                UpdateRadiusY();
        }

        void UpdateRadius()
		{
            UpdateRadiusX();
            UpdateRadiusY();
        }

        void UpdateRadiusX()
        {
            if (Element.Width > 0)
                Control.UpdateRadiusX(Element.RadiusX / Element.Width);
        }

        void UpdateRadiusY()
        {
            if (Element.Height > 0)
                Control.UpdateRadiusY(Element.RadiusY / Element.Height);
        }
    }

    public class RectView : ShapeView
    {
        public RectView(Context context) : base(context)
        {
            UpdateShape();
        }

        public float RadiusX { set; get; }

        public float RadiusY { set; get; }

        void UpdateShape()
        {
			var path = new APath();
            path.AddRoundRect(new RectF(0, 0, 1, 1), RadiusX, RadiusY, APath.Direction.Cw);
            UpdateShape(path);
        }

        public void UpdateRadiusX(double radiusX)
        {
            RadiusX = (float)radiusX;
            UpdateShape();
        }

        public void UpdateRadiusY(double radiusY)
        {
            RadiusY = (float)radiusY;
            UpdateShape();
        }
    }
}