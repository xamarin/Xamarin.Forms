using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using AColor = Android.Graphics.Color;
using AMatrix = Android.Graphics.Matrix;
using APath = Android.Graphics.Path;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
    public class ShapeRenderer<TShape, TNativeShape> : ViewRenderer<TShape, TNativeShape>
         where TShape : Shape
         where TNativeShape : ShapeView
    {
        double _height;
        double _width;

        public ShapeRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<TShape> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                UpdateSize();
                UpdateAspect();
                UpdateFill();
                UpdateStroke();
                UpdateStrokeThickness();
                UpdateStrokeDashArray();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == VisualElement.HeightProperty.PropertyName)
            {
                _height = (Element.HeightRequest > 0) ? Element.HeightRequest : Element.Height;
                UpdateSize();
            }
            else if (args.PropertyName == VisualElement.WidthProperty.PropertyName)
            {
                _width = (Element.WidthRequest > 0) ? Element.WidthRequest : Element.Width;
                UpdateSize();
            }
            else if (args.PropertyName == Shape.AspectProperty.PropertyName)
                UpdateAspect();
            else if (args.PropertyName == Shape.FillProperty.PropertyName)
                UpdateFill();
            else if (args.PropertyName == Shape.StrokeProperty.PropertyName)
                UpdateStroke();
            else if (args.PropertyName == Shape.StrokeThicknessProperty.PropertyName)
                UpdateStrokeThickness();
            else if (args.PropertyName == Shape.StrokeDashArrayProperty.PropertyName)
                UpdateStrokeDashArray();
        }

        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            if (Element != null)
            {
                return Control.GetDesiredSize();
            }

            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }

        void UpdateSize()
        {
            Control.UpdateSize(_width, _height);
        }

        void UpdateAspect()
        {
            Control.UpdateAspect(Element.Aspect);
        }

        void UpdateFill()
        {
            Control.UpdateFill(Element.Fill.ToAndroid());
        }

        void UpdateStroke()
        {
            Control.UpdateStroke(Element.Stroke.ToAndroid());
        }

        void UpdateStrokeThickness()
        {
            Control.UpdateStrokeThickness((float)Element.StrokeThickness);
        }

        void UpdateStrokeDashArray()
        {
            Control.UpdateStrokeDash(Element.StrokeDashArray.ToArray());
        }
    }

    public class ShapeView : AView
    {
        readonly ShapeDrawable _drawable;
        protected float _density;

        APath _path;

        readonly RectF _pathFillBounds;
        readonly RectF _pathStrokeBounds;

        AColor _stroke;
        AColor _fill;

        float _strokeWidth;
        float[] _strokeDash;

        double _height;
        double _width;

        Stretch _aspect;

        public ShapeView(Context context) : base(context)
        {
            _drawable = new ShapeDrawable(null);
            _density = Resources.DisplayMetrics.Density;

            _pathFillBounds = new RectF();
            _pathStrokeBounds = new RectF();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (_path == null)
                return;

            AMatrix matrix = CreateMatrix();
            _path.Transform(matrix);
            matrix.MapRect(_pathFillBounds);
            matrix.MapRect(_pathStrokeBounds);

            if (_fill != null)
            {
                _drawable.Paint.SetStyle(Paint.Style.Fill);
                _drawable.Paint.Color = _fill;
                _drawable.Draw(canvas);
                _drawable.Paint.SetShader(null);
            }

            if (_stroke != null)
            {
                _drawable.Paint.SetStyle(Paint.Style.Stroke);
                _drawable.Paint.Color = _stroke;
                _drawable.Draw(canvas);
                _drawable.Paint.SetShader(null);
            }

            AMatrix inverseMatrix = new AMatrix();
            matrix.Invert(inverseMatrix);
            _path.Transform(inverseMatrix);
            inverseMatrix.MapRect(_pathFillBounds);
            inverseMatrix.MapRect(_pathStrokeBounds);
        }

        public void UpdateShape(APath path)
        {
            _path = path;
            UpdatePathShape();
        }

        public SizeRequest GetDesiredSize()
        {
            if (_path != null)
            {
                return new SizeRequest(new Size(Math.Max(0, _pathStrokeBounds.Right),
                    Math.Max(0, _pathStrokeBounds.Bottom)));
            }

            return new SizeRequest();
        }

        public void UpdateAspect(Stretch aspect)
        {
            _aspect = aspect;
            Invalidate();
        }

        public void UpdateFill(AColor fill)
        {
            _fill = fill;
            Invalidate();
        }

        public void UpdateStroke(AColor stroke)
        {
            _stroke = stroke;
            Invalidate();
        }

        public void UpdateStrokeThickness(float strokeWidth)
        {
            _strokeWidth = _density * strokeWidth;
            _drawable.Paint.StrokeWidth = _strokeWidth;
        }

        public void UpdateStrokeDash(float[] dash)
        {
            _strokeDash = dash;

            if (_strokeDash != null && _strokeDash.Length > 1)
            {
                float[] strokeDash = new float[_strokeDash.Length];

                for (int i = 0; i < _strokeDash.Length; i++)
                    strokeDash[i] = _strokeDash[i] * _strokeWidth;

                _drawable.Paint.SetPathEffect(new DashPathEffect(strokeDash, 0));
            }
            else
                _drawable.Paint.SetPathEffect(null);
        }

        public void UpdateSize(double width, double height)
        {
            _width = width;
            _height = height;

            _drawable.SetBounds(0, 0, (int)(_width * _density), (int)(_height * _density));
            UpdatePathShape();
        }

        void UpdatePathShape()
        {
            if (_drawable.Bounds.IsEmpty)
                return;

            if (_path != null)
                _drawable.Shape = new PathShape(_path, _drawable.Bounds.Width(), _drawable.Bounds.Height());
            else
                _drawable.Shape = null;

            if (_path != null)
            {
                using (var fillPath = new APath())
                {
                    _drawable.Paint.StrokeWidth = 0.01f;
                    _drawable.Paint.SetStyle(Paint.Style.Stroke);
                    _drawable.Paint.GetFillPath(_path, fillPath);
                    fillPath.ComputeBounds(_pathFillBounds, false);
                    _drawable.Paint.StrokeWidth = _strokeWidth;
                }
			}
            else
                _pathFillBounds.SetEmpty();

            UpdatePathStrokeBounds();
        }

        AMatrix CreateMatrix()
        {
			var matrix = new AMatrix();

			var drawableBounds = new RectF(_drawable.Bounds);
            float halfStrokeWidth = _drawable.Paint.StrokeWidth / 2;

            drawableBounds.Left += halfStrokeWidth;
            drawableBounds.Top += halfStrokeWidth;
            drawableBounds.Right -= halfStrokeWidth;
            drawableBounds.Bottom -= halfStrokeWidth;

            switch (_aspect)
            {
                case Stretch.None:
                case Stretch.Fill:
                    matrix.SetRectToRect(_pathFillBounds, drawableBounds, AMatrix.ScaleToFit.Fill);
                    break;
                case Stretch.Uniform:
                    matrix.SetRectToRect(_pathFillBounds, drawableBounds, AMatrix.ScaleToFit.Center);
                    break;
                case Stretch.UniformToFill:
                    float widthScale = drawableBounds.Width() / _pathFillBounds.Width();
                    float heightScale = drawableBounds.Height() / _pathFillBounds.Height();
                    float maxScale = Math.Max(widthScale, heightScale);

                    matrix.SetScale(maxScale, maxScale);
                    matrix.PostTranslate(drawableBounds.Left - maxScale * _pathFillBounds.Left,
                                         drawableBounds.Top - maxScale * _pathFillBounds.Top);
                    break;
            }

            return matrix;
        }

        void UpdatePathStrokeBounds()
        {
            if (_path != null)
            {
                using (var strokePath = new APath())
                {
                    _drawable.Paint.SetStyle(Paint.Style.Stroke);
                    _drawable.Paint.GetFillPath(_path, strokePath);
                    strokePath.ComputeBounds(_pathStrokeBounds, true);
                }
            }
            else
                _pathStrokeBounds.SetEmpty();

            Invalidate();
        }
    }
}