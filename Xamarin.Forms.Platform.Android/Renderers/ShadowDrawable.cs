using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using AColor = Android.Graphics.Color;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android.Renderers
{
    public class ShadowDrawable : Drawable
    {
		readonly Paint _paintShadow;
		readonly Path _path;

        RectF _rectF;
        AView _parent;

        float _radius;
        float _offsetX;
        float _offsetY;
        AColor _color;
        float _cornerRadius;

        public ShadowDrawable()
        {
            _paintShadow = new Paint
            {
                Color = AColor.White,
                AntiAlias = true
            };

            _path = new Path();
        }

		public void UpdateBackgroundColor(Color backgroundColor)
		{
            _paintShadow.Color = backgroundColor.ToAndroid();
        }

		public void UpdateCornerRadius(float cornerRadius)
		{
            _cornerRadius = cornerRadius;
            _paintShadow.SetShadowLayer(_radius, _offsetX, _offsetY, _color);
        }

		public void UpdateShadow(DropShadow shadow)
        {
            _radius = (float)shadow.Radius;
            _offsetX = (float)shadow.Offset.X;
            _offsetY = (float)shadow.Offset.Y;

            int opacity = (int)(shadow.Opacity * 255);
            var shadowColor = shadow.Color.ToAndroid();
            _color = AColor.Argb(opacity, shadowColor.R, shadowColor.G, shadowColor.B);

            _paintShadow.SetShadowLayer(_radius, _offsetX, _offsetY, _color);
        }

        public void Attach(AView view)
        {
            view.SetLayerType(LayerType.Software, null);
            _parent = view;

            if (_parent == null)
                throw new NotSupportedException("ShadowDrawable must have a Parent.");

            _parent.SetBackground(this);

            InvalidateSelf();
        }
				
        public override void Draw(Canvas canvas)
        {
            canvas.DrawPath(_path, _paintShadow);
        }
		
        public override void SetAlpha(int alpha)
        {
            _paintShadow.Alpha = alpha;
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {

        }

        protected override void OnBoundsChange(Rect bounds)
        {
            base.OnBoundsChange(bounds);

            _path.Reset();
            _rectF = new RectF(_radius, 0, bounds.Width() - _radius, bounds.Height() - _offsetY - _radius);
            _path.AddRoundRect(_rectF, new float[] { _cornerRadius, _cornerRadius, _cornerRadius, _cornerRadius, _cornerRadius, _cornerRadius, _cornerRadius, _cornerRadius }, Path.Direction.Cw);
        }

        public override int Opacity => 1;
    }
}