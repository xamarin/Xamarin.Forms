using System.ComponentModel;
using Android.Content;
using APath = Android.Graphics.Path;

namespace Xamarin.Forms.Platform.Android
{
    public class PathRenderer : ShapeRenderer<Path, PathView>
    {
        public PathRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Path> args)
        {
            if (Control == null)
            {
                SetNativeControl(new PathView(Context));
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
            Control.UpdateData(Element.Data.ToAPath(Context));
        }
    }

    public class PathView : ShapeView
    {
        public PathView(Context context) : base(context)
        {
        }

        public void UpdateData(APath path)
        {
            UpdateShape(path);
        }
    }
}
