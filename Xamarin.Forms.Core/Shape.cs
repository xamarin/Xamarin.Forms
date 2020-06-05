namespace Xamarin.Forms
{
    public abstract class Shape : View
    {
		public Shape()
		{
            ExperimentalFlags.VerifyFlagEnabled(nameof(Shape), ExperimentalFlags.ShapesExperimental);
        }

        public static readonly BindableProperty FillProperty =
            BindableProperty.Create(nameof(Fill), typeof(Color), typeof(Shape), null);

        public static readonly BindableProperty StrokeProperty =
            BindableProperty.Create(nameof(Stroke), typeof(Color), typeof(Shape), null);

        public static readonly BindableProperty StrokeThicknessProperty =
            BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(Shape), 1.0);

        public static readonly BindableProperty StrokeDashArrayProperty =
            BindableProperty.Create(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(Shape), null,
                defaultValueCreator: bindable => new DoubleCollection());

        public static readonly BindableProperty StrokeDashOffsetProperty =
            BindableProperty.Create(nameof(StrokeDashOffset), typeof(double), typeof(Shape), 0.0);

        public static readonly BindableProperty AspectProperty =
            BindableProperty.Create(nameof(Aspect), typeof(Stretch), typeof(Shape), Stretch.None);

        public Color Fill
        {
            set { SetValue(FillProperty, value); }
            get { return (Color)GetValue(FillProperty); }
        }

        public Color Stroke
        {
            set { SetValue(StrokeProperty, value); }
            get { return (Color)GetValue(StrokeProperty); }
        }

        public double StrokeThickness
        {
            set { SetValue(StrokeThicknessProperty, value); }
            get { return (double)GetValue(StrokeThicknessProperty); }
        }

        public DoubleCollection StrokeDashArray
        {
            set { SetValue(StrokeDashArrayProperty, value); }
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
        }

        public double StrokeDashOffset
        {
            set { SetValue(StrokeDashOffsetProperty, value); }
            get { return (double)GetValue(StrokeDashOffsetProperty); }
        }

        public Stretch Aspect
        {
            set { SetValue(AspectProperty, value); }
            get { return (Stretch)GetValue(AspectProperty); }
        }
    }
}