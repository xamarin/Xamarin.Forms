using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Shape)]
#endif
    [Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11050, "[Bug][iOS][Android] Shapes: clock drawing erro", PlatformAffected.Android | PlatformAffected.iOS)]
	public partial class Issue11050 : ContentPage
	{
        public Issue11050()
        {
#if APP
            Device.SetFlags(new List<string> { ExperimentalFlags.ShapesExperimental });

            InitializeComponent();

            BindingContext = this;

            Device.StartTimer(TimeSpan.FromMilliseconds(15), () =>
            {
                DateTime dateTime = DateTime.Now;
                SecondHandAngle = 6 * (dateTime.Second + dateTime.Millisecond / 1000.0);
                MinuteHandAngle = 6 * dateTime.Minute + SecondHandAngle / 60;
                HourHandAngle = 30 * (dateTime.Hour % 12) + MinuteHandAngle / 12;
                return true;
            });

            SizeChanged += (sender, args) =>
            {
                grid.AnchorX = 0;
                grid.AnchorY = 0;
                grid.Scale = Math.Min(Width, Height) / 200;
            };
#endif
        }

        public static readonly BindableProperty SecondHandAngleProperty = BindableProperty.Create(nameof(SecondHandAngle), typeof(double), typeof(Issue11050), default(double));
        public static readonly BindableProperty MinuteHandAngleProperty = BindableProperty.Create(nameof(MinuteHandAngle), typeof(double), typeof(Issue11050), default(double));
        public static readonly BindableProperty HourHandAngleProperty = BindableProperty.Create(nameof(HourHandAngle), typeof(double), typeof(Issue11050), default(double));

        public double SecondHandAngle
        {
            get { return (double)GetValue(SecondHandAngleProperty); }
            set { SetValue(SecondHandAngleProperty, value); }
        }

        public double MinuteHandAngle
        {
            get { return (double)GetValue(MinuteHandAngleProperty); }
            set { SetValue(MinuteHandAngleProperty, value); }
        }

        public double HourHandAngle
        {
            get { return (double)GetValue(HourHandAngleProperty); }
            set { SetValue(HourHandAngleProperty, value); }
        }
    }
}
