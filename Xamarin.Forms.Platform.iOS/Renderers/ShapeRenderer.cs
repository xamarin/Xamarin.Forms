using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
    public class ShapeRenderer<TShape, TNativeShape> : ViewRenderer<TShape, TNativeShape>
        where TShape : Shape
        where TNativeShape : ShapeView
    {
        double _height;
        double _width;

        protected override void OnElementChanged(ElementChangedEventArgs<TShape> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                UpdateAspect();
                UpdateFill();
                UpdateStroke();
                UpdateStrokeThickness();
                UpdateStrokeDashArray();
                UpdateStrokeDashOffset();
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
            else if (args.PropertyName == Shape.StrokeDashOffsetProperty.PropertyName)
                UpdateStrokeDashOffset();
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            if (Control != null)
            {
                return Control.ShapeLayer.GetDesiredSize();
            }

            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }

        void UpdateAspect()
        {
            Control.ShapeLayer.UpdateAspect(Element.Aspect);
        }

        void UpdateSize()
        {
            Control.ShapeLayer.UpdateSize(new CGSize(new nfloat(_width), new nfloat(_height)));
        }

        void UpdateFill()
        {
            Control.ShapeLayer.UpdateFill(Element.Fill.ToCGColor());
        }

        void UpdateStroke()
        {
            Control.ShapeLayer.UpdateStroke(Element.Stroke.ToCGColor());
        }

        void UpdateStrokeThickness()
        {
            Control.ShapeLayer.UpdateStrokeThickness(Element.StrokeThickness);
        }

        void UpdateStrokeDashArray()
        {
            Control.ShapeLayer.UpdateStrokeDash(Element.StrokeDashArray.ToArray());
        }

        void UpdateStrokeDashOffset()
        {
            Control.ShapeLayer.UpdateStrokeDashOffset((nfloat)Element.StrokeDashOffset);
        }
    }

    public class ShapeView : UIView
    {
        public ShapeView()
        {
            BackgroundColor = UIColor.Clear;
            ShapeLayer = new ShapeLayer();
            Layer.AddSublayer(ShapeLayer);
            Layer.MasksToBounds = false;
        }

        public ShapeLayer ShapeLayer
        {
            private set;
            get;
        }
    }

    public class ShapeLayer : CALayer
    {
        CGPath _path;
        CGRect _pathBounds;

        CGPath _renderPath;

        CGColor _stroke;
        CGColor _fill;

        nfloat _strokeWidth;
        nfloat[] _strokeDash;
        nfloat _dashOffset;

        bool _fillMode;

        CGLineCap _strokeLineCap;
        CGLineJoin _strokeLineJoin;

        Stretch _aspect;

        public override void DrawInContext(CGContext ctx)
        {
            base.DrawInContext(ctx);
            RenderShape(ctx);
        }

        public void UpdateShape(CGPath path)
        {
            _path = path;

            if (_path != null)
                _pathBounds = _path.PathBoundingBox;
            else
                _pathBounds = new CGRect();

            BuildRenderPath();
        }

        public void UpdateFillMode(bool fillMode)
        {
            _fillMode = fillMode;
            SetNeedsDisplay();
        }

        public SizeRequest GetDesiredSize()
        {
            return new SizeRequest(new Size(Bounds.Width, Bounds.Height));
        }

        public void UpdateSize(CGSize size)
        {
            Bounds = new CGRect(new CGPoint(), size);
            BuildRenderPath();
        }

        public void UpdateAspect(Stretch aspect)
        {
            _aspect = aspect;
            BuildRenderPath();
        }

        public void UpdateFill(CGColor fill)
        {
            _fill = fill;
            SetNeedsDisplay();
        }

        public void UpdateStroke(CGColor stroke)
        {
            _stroke = stroke;
            SetNeedsDisplay();
        }

        public void UpdateStrokeThickness(double strokeWidth)
        {
            _strokeWidth = new nfloat(strokeWidth);
            BuildRenderPath();
        }

        public void UpdateStrokeDash(nfloat[] dash)
        {
            _strokeDash = dash;
            SetNeedsDisplay();
        }

        public void UpdateStrokeDashOffset(nfloat dashOffset)
        {
            _dashOffset = dashOffset;
            SetNeedsDisplay();
        }

        public void UpdateStrokeLineCap(CGLineCap strokeLineCap)
        {
            _strokeLineCap = strokeLineCap;
            SetNeedsDisplay();
        }

        public void UpdateStrokeLineJoin(CGLineJoin strokeLineJoin)
        {
            _strokeLineJoin = strokeLineJoin;
            SetNeedsDisplay();
        }

        void BuildRenderPath()
        {
            if (_path == null)
            {
                _renderPath = null;
                return;
            }

            CATransaction.Begin();
            CATransaction.DisableActions = true;

            CGRect viewBounds = Bounds;
            viewBounds.X += _strokeWidth / 2;
            viewBounds.Y += _strokeWidth / 2;
            viewBounds.Width -= _strokeWidth;
            viewBounds.Height -= _strokeWidth;

            nfloat widthScale = viewBounds.Width / _pathBounds.Width;
            nfloat heightScale = viewBounds.Height / _pathBounds.Height;
            CGAffineTransform stretchTransform = CGAffineTransform.MakeIdentity();

            if (_aspect == Stretch.None)
            {
                stretchTransform.Scale(widthScale, heightScale);
                stretchTransform.Translate(viewBounds.Left - widthScale * _pathBounds.Left, viewBounds.Top - heightScale * _pathBounds.Top);
            }
            else
            {
                switch (_aspect)
                {
                    case Stretch.Fill:
                        stretchTransform.Scale(widthScale, heightScale);

                        stretchTransform.Translate(
                            viewBounds.Left - widthScale * _pathBounds.Left,
                            viewBounds.Top - heightScale * _pathBounds.Top);
                        break;

                    case Stretch.Uniform:
                        nfloat minScale = NMath.Min(widthScale, heightScale);

                        stretchTransform.Scale(minScale, minScale);

                        stretchTransform.Translate(
                            viewBounds.Left - minScale * _pathBounds.Left +
                            (viewBounds.Width - minScale * _pathBounds.Width) / 2,
                            viewBounds.Top - minScale * _pathBounds.Top +
                            (viewBounds.Height - minScale * _pathBounds.Height) / 2);
                        break;

                    case Stretch.UniformToFill:
                        nfloat maxScale = NMath.Max(widthScale, heightScale);
                        stretchTransform.Scale(maxScale, maxScale);

                        stretchTransform.Translate(
                            viewBounds.Left - maxScale * _pathBounds.Left,
                            viewBounds.Top - maxScale * _pathBounds.Top);
                        break;
                }
            }


            Frame = Bounds;
            _renderPath = _path.CopyByTransformingPath(stretchTransform);

            CATransaction.Commit();

            SetNeedsDisplay();
        }

        void RenderShape(CGContext graphics)
        {
            if (_path == null)
                return;

            if (_stroke == null && _fill == null)
                return;

            CATransaction.Begin();
            CATransaction.DisableActions = true;

            graphics.SetLineWidth(_strokeWidth);

            var lengths = new nfloat[_strokeDash.Length];
            for (int i = 0; i < _strokeDash.Length; i++)
                lengths[i] = new nfloat(_dashOffset * _strokeDash[i]);

            graphics.SetLineDash(_dashOffset * _strokeWidth, lengths);
            graphics.AddPath(_renderPath);
            graphics.SetStrokeColor(_stroke);
            graphics.SetFillColor(_fill);
            graphics.SetLineCap(_strokeLineCap);
            graphics.SetLineJoin(_strokeLineJoin);

            graphics.DrawPath(_fillMode ? CGPathDrawingMode.FillStroke : CGPathDrawingMode.EOFillStroke);

            CATransaction.Commit();
        }
    }
}