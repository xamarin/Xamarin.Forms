using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
	public class DropShadow : BindableObject
    {
        static bool IsExperimentalFlagSet = false;

		public DropShadow()
		{
            VerifyExperimental(nameof(DropShadow));
        }

        public static readonly BindableProperty RadiusProperty =
			BindableProperty.Create(nameof(Radius), typeof(double), typeof(DropShadow), 10.0d);

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        public static readonly BindableProperty ColorProperty =
			BindableProperty.Create(nameof(Color), typeof(Color), typeof(DropShadow), Color.Black);

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly BindableProperty OffsetProperty =
			BindableProperty.Create(nameof(Offset), typeof(Point), typeof(DropShadow), default(Point));

        public Point Offset
        {
            get => (Point)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public static readonly BindableProperty OpacityProperty =
            BindableProperty.Create(nameof(Opacity), typeof(double), typeof(DropShadow), 0.5d);

		public double Opacity
        {
            get => (double)GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }

        internal static void VerifyExperimental([CallerMemberName] string memberName = "", string constructorHint = null)
        {
            if (IsExperimentalFlagSet)
                return;

            ExperimentalFlags.VerifyFlagEnabled(nameof(DropShadow), ExperimentalFlags.ShadowExperimental, constructorHint, memberName);

            IsExperimentalFlagSet = true;
        }
	}
}