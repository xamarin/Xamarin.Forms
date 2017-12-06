using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
    public sealed class SwipeGestureRecognizer : GestureRecognizer, ISwipeGestureController
    {
        // Default threshold in pixels before a swipe is detected.
        const double DefaultSwipeThreshold = 100;

        private double _totalX, _totalY;

        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(SwipeGestureRecognizer), null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(SwipeGestureRecognizer), null);

        public static readonly BindableProperty DirectionProperty = BindableProperty.Create("Direction", typeof(SwipeDirection), typeof(SwipeGestureRecognizer), default(SwipeDirection));

        public static readonly BindableProperty ThresholdProperty = BindableProperty.Create("Threshold", typeof(double), typeof(SwipeGestureRecognizer), DefaultSwipeThreshold);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public SwipeDirection Direction
        {
            get { return (SwipeDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public double Threshold
        {
            get { return (double)GetValue(ThresholdProperty); }
            set { SetValue(ThresholdProperty, value); }
        }

        public event EventHandler<SwipedEventArgs> Swiped;

        void ISwipeGestureController.SendSwipe(Element sender, double totalX, double totalY)
        {
            _totalX = totalX;
            _totalY = totalY;
        }

        bool ISwipeGestureController.DetectSwipe(View sender, SwipeDirection direction)
        {
            var detected = false;
            var threshold = Threshold;

            switch (direction)
            {
                case SwipeDirection.Left:
                    detected = _totalX < -threshold;
                    break;
                case SwipeDirection.Right:
                    detected = _totalX > threshold;
                    break;
                case SwipeDirection.Down:
                    detected = _totalY > threshold;
                    break;
                case SwipeDirection.Up:
                    detected = _totalY < -threshold;
                    break;
            }

            if (detected)
            {
                SendSwiped(sender, direction);
            }

            return detected;
        }

        public void SendSwiped(View sender, SwipeDirection direction)
        {
            ICommand cmd = Command;
            if (cmd != null && cmd.CanExecute(CommandParameter))
                cmd.Execute(CommandParameter);

            Swiped?.Invoke(sender, new SwipedEventArgs(CommandParameter, direction));
        }
    }
}