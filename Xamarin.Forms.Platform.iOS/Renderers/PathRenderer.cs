using System.ComponentModel;
using CoreGraphics;

namespace Xamarin.Forms.Platform.iOS
{
	public class PathRenderer : ShapeRenderer<Path, PathView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Path> args)
        {
            if (Control == null)
            {
                SetNativeControl(new PathView());
            }

            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                UpdateData();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == Path.DataProperty.PropertyName)
                UpdateData();
        }

        void UpdateData()
        {
            Control.UpdateData(Element.Data.ToCGPath());
        }
    }

	public class PathData
    {
        public CGPath Data { get; set; }
        public bool IsNonzeroFillRule { get; set; }
    }

    public class PathView : ShapeView
    {
        public void UpdateData(PathData path)
        {
            ShapeLayer.UpdateShape(path.Data);
            ShapeLayer.UpdateFillMode(path == null ? false : path.IsNonzeroFillRule);
        }
    }
}
