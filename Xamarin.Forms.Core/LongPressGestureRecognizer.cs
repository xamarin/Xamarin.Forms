using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public class LongPressGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty IsLongPressingProperty =
			BindableProperty.Create(nameof(IsLongPressing), typeof(bool), typeof(LongPressGestureRecognizer));

		public static readonly BindableProperty StartedCommandProperty =
			BindableProperty.Create(nameof(StartedCommand), typeof(ICommand), typeof(LongPressGestureRecognizer));

		public static readonly BindableProperty FinishedCommandProperty =
			BindableProperty.Create(nameof(FinishedCommand), typeof(ICommand), typeof(LongPressGestureRecognizer));

		public static readonly BindableProperty CancelledCommandProperty =
			BindableProperty.Create(nameof(CancelledCommand), typeof(ICommand), typeof(LongPressGestureRecognizer));

		public static readonly BindableProperty StartedCommandParameterProperty =
			BindableProperty.Create(nameof(StartedCommandParameter), typeof(object), typeof(LongPressGestureRecognizer));

		public static readonly BindableProperty FinishedCommandParameterProperty =
			BindableProperty.Create(nameof(FinishedCommandParameter), typeof(object), typeof(LongPressGestureRecognizer));

		public static readonly BindableProperty CancelledCommandParameterProperty =
			BindableProperty.Create(nameof(CancelledCommandParameter), typeof(object), typeof(LongPressGestureRecognizer));

		public static readonly BindableProperty NumberOfTouchesRequiredProperty =
			BindableProperty.Create(nameof(NumberOfTouchesRequired), typeof(int), typeof(LongPressGestureRecognizer), 1);

		public static readonly BindableProperty PressDurationProperty = BindableProperty.Create(nameof(PressDuration), typeof(int),
			typeof(LongPressGestureRecognizer), 1000);

		public static readonly BindableProperty AllowMovementProperty =
			BindableProperty.Create(nameof(AllowMovement), typeof(bool), typeof(LongPressGestureRecognizer), false);

		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(LongPressGestureRecognizer));

		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(LongPressGestureRecognizer));

		CancellationTokenSource _cts;

		public bool AllowMovement
		{
			get => (bool)GetValue(AllowMovementProperty);
			set => SetValue(AllowMovementProperty, value);
		}

		public ICommand CancelledCommand
		{
			get => (ICommand)GetValue(CancelledCommandProperty);
			set => SetValue(CancelledCommandProperty, value);
		}

		public object CancelledCommandParameter
		{
			get => GetValue(CancelledCommandParameterProperty);
			set => SetValue(CancelledCommandParameterProperty, value);
		}

		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public object CommandParameter
		{
			get => GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}

		public ICommand FinishedCommand
		{
			get => (ICommand)GetValue(FinishedCommandProperty);
			set => SetValue(FinishedCommandProperty, value);
		}

		public object FinishedCommandParameter
		{
			get => GetValue(FinishedCommandParameterProperty);
			set => SetValue(FinishedCommandParameterProperty, value);
		}

		public bool IsLongPressing
		{
			get => (bool)GetValue(IsLongPressingProperty);
			set => SetValue(IsLongPressingProperty, value);
		}

		public int NumberOfTouchesRequired
		{
			get => (int)GetValue(NumberOfTouchesRequiredProperty);
			set => SetValue(NumberOfTouchesRequiredProperty, value);
		}

		public int PressDuration
		{
			get => (int)GetValue(PressDurationProperty);
			set => SetValue(PressDurationProperty, value);
		}

		public ICommand StartedCommand
		{
			get => (ICommand)GetValue(StartedCommandProperty);
			set => SetValue(StartedCommandProperty, value);
		}

		public object StartedCommandParameter
		{
			get => GetValue(StartedCommandParameterProperty);
			set => SetValue(StartedCommandParameterProperty, value);
		}

		public event EventHandler LongPressed;

		public override void OnTouch(View sender, TouchEventArgs eventArgs)
		{
			if (TouchCount != NumberOfTouchesRequired)
			{
				Cancel();
				return;
			}

			if (eventArgs.TouchState == TouchState.Move && !AllowMovement)
			{
				var gestureIsNone = true;
				foreach (Touch item in Touches)
				{
					if (item.Gesture != GestureDirection.None)
					{
						gestureIsNone = false;
						break;
					}
				}

				if (gestureIsNone)
				{
					Cancel();
				}
			}

			if (eventArgs.TouchState == TouchState.Press)
			{
				_cts?.Cancel();
				_cts = new CancellationTokenSource();
				IsLongPressing = true;
				StartedCommand.Run(StartedCommandParameter);
				Task.Delay(PressDuration, _cts.Token).ContinueWith(task =>
				{
					if (task.IsCanceled)
					{
						return;
					}

					Device.BeginInvokeOnMainThread(() =>
					{
						LongPressed?.Invoke(sender, new EventArgs());
						FinishedCommand.Run(FinishedCommandParameter);
						Command.Run(CommandParameter);
						IsLongPressing = false;
					});
				});
			}

			if (eventArgs.TouchState == TouchState.Release || eventArgs.TouchState == TouchState.Cancel || eventArgs.TouchState == TouchState.Fail ||
			    eventArgs.TouchState == TouchState.Move)
			{
				foreach (TouchPoint item in eventArgs.TouchPoints)
				{
					if (!item.IsInOriginalView)
					{
						Cancel();
						break;
					}
				}
			}
		}

		void Cancel()
		{
			_cts?.Cancel();
			if (IsLongPressing)
			{
				IsLongPressing = false;
				CancelledCommand.Run(CancelledCommandParameter);
			}
		}
	}
}