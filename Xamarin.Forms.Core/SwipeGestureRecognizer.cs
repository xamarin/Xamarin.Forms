using System;
using System.Windows.Input;
using System.Linq;

namespace Xamarin.Forms
{
	public class SwipeGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(SwipeGestureRecognizer), null);

		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(SwipeGestureRecognizer), null);

		public SwipeGestureRecognizer()
		{

		}

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


		public event EventHandler SwipeLeft;

		public event EventHandler SwipeRight;

		public event EventHandler SwipeDown;

		public event EventHandler SwipeUp;

		//public event EventHandler SwipeCompleted;

		internal void HandleRightSwipe(View sender)
		{
			ICommand cmd = Command;
			if (cmd != null && cmd.CanExecute(CommandParameter))
				cmd.Execute(CommandParameter);

			EventHandler handler = SwipeRight;
			if (handler != null)
				handler(sender, new SwipeRightEventArgs(CommandParameter));
		}

		internal void HandleLeftSwipe(View sender)
		{
			ICommand cmd = Command;
			if (cmd != null && cmd.CanExecute(CommandParameter))
				cmd.Execute(CommandParameter);

			EventHandler handler = SwipeLeft;
			if (handler != null)
				handler(sender, new SwipeLeftEventArgs(CommandParameter));
		}

		internal void HandleDownSwipe(View sender)
		{
			ICommand cmd = Command;
			if (cmd != null && cmd.CanExecute(CommandParameter))
				cmd.Execute(CommandParameter);

			EventHandler handler = SwipeDown;
			if (handler != null)
				handler(sender, new SwipeDownEventArgs(CommandParameter));
		}

		internal void HandleUpSwipe(View sender)
		{
			ICommand cmd = Command;
			if (cmd != null && cmd.CanExecute(CommandParameter))
				cmd.Execute(CommandParameter);

			EventHandler handler = SwipeUp;
			if (handler != null)
				handler(sender, new SwipeDownEventArgs(CommandParameter));
		}

	}


}