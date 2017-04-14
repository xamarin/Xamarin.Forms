using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public sealed class SwipeGestureRecognizer : GestureRecognizer, ISwipeGestureController
	{
		private double _totalX, _totalY;

		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(SwipeGestureRecognizer), null);

		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(SwipeGestureRecognizer), null);

		public static readonly BindableProperty DirectionProperty = BindableProperty.Create("Direction", typeof(SwipeDirection), typeof(SwipeGestureRecognizer), default(SwipeDirection));

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

        public event EventHandler<SwipedEventArgs> Swiped;

		double ISwipeGestureController.TotalX => _totalX;
		double ISwipeGestureController.TotalY => _totalY;

		void ISwipeGestureController.SendSwipe(Element sender, double totalX, double totalY)
		{
			_totalX = totalX;
			_totalY = totalY;
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