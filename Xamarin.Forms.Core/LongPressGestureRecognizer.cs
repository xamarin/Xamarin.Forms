using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.Core.Internals;

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
				bool gestureIsNone = true;
				foreach (var item in Touches)
				{
					if (item.Gesture != GestureDirection.None)
					{
						gestureIsNone = false;
						break;
					}
				}

				if(gestureIsNone)
					Cancel();
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
						IsLongPressing = false;
					});
				});
			}

			if (eventArgs.TouchState == TouchState.Release || eventArgs.TouchState == TouchState.Cancel ||
				eventArgs.TouchState == TouchState.Fail ||
				eventArgs.TouchState == TouchState.Move)
			{
				foreach (var item in eventArgs.TouchPoints)
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