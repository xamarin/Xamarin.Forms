using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.Core.Internals;

namespace Xamarin.Forms
{
	public class MultiTapPressGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MultiTapPressGestureRecognizer));

		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(MultiTapPressGestureRecognizer));

		public static readonly BindableProperty NumberOfTapsRequiredProperty =
			BindableProperty.Create(nameof(NumberOfTapsRequired), typeof(int), typeof(MultiTapPressGestureRecognizer), 1);

		public static readonly BindableProperty StartedCommandProperty =
			BindableProperty.Create(nameof(StartedCommand), typeof(ICommand), typeof(MultiTapPressGestureRecognizer));

		public static readonly BindableProperty StartedCommandParameterProperty =
			BindableProperty.Create(nameof(StartedCommandParameter), typeof(object), typeof(MultiTapPressGestureRecognizer));

		public static readonly BindableProperty CancelledCommandProperty =
			BindableProperty.Create(nameof(CancelledCommand), typeof(ICommand), typeof(MultiTapPressGestureRecognizer));

		public static readonly BindableProperty CancelledCommandParameterProperty =
			BindableProperty.Create(nameof(CancelledCommandParameter), typeof(object), typeof(MultiTapPressGestureRecognizer));

		public static readonly BindableProperty DelayBetweenTapsProperty = BindableProperty.Create(nameof(DelayBetweenTaps), typeof(int),
			typeof(LongPressGestureRecognizer), 500);

		CancellationTokenSource _cts;

		int _tapCount;

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

		public int DelayBetweenTaps
		{
			get => (int)GetValue(DelayBetweenTapsProperty);
			set => SetValue(DelayBetweenTapsProperty, value);
		}

		public bool IsTapping { get; private set; }

		public int NumberOfTapsRequired
		{
			get => (int)GetValue(NumberOfTapsRequiredProperty);
			set => SetValue(NumberOfTapsRequiredProperty, value);
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

		public event EventHandler Tapped;

		public override void OnTouch(View sender, TouchEventArgs eventArgs)
		{
			if (eventArgs.TouchState == TouchState.Cancel || eventArgs.TouchState == TouchState.Fail ||
				eventArgs.TouchState == TouchState.Move && eventArgs.TouchPoints.Any(a => !a.IsInOriginalView))
			{
				Cancel();
				return;
			}

			_cts?.Cancel();
			_cts = new CancellationTokenSource();

			if (State == TouchState.Release)
			{
				_tapCount++;
				if (!IsTapping)
				{
					IsTapping = true;
					StartedCommand.Run(StartedCommandParameter);
				}

				if (_tapCount == NumberOfTapsRequired)
				{
					_tapCount = 0;
					Tapped?.Invoke(this, new EventArgs());
					Command.Run(CommandParameter);
					IsTapping = false;
				}
				else
				{
					Task.Delay(DelayBetweenTaps, _cts.Token).ContinueWith(task =>
					{
						if (task.IsCanceled)
						{
							return;
						}

						Cancel();
					});
				}
			}
		}

		void Cancel()
		{
			_cts?.Cancel();
			_tapCount = 0;
			if (IsTapping)
			{
				IsTapping = false;
				CancelledCommand.Run(CancelledCommandParameter);
			}
		}
	}
}