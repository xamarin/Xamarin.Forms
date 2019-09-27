using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.Core.Internals;

namespace Xamarin.Forms
{
	public class RotateGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty NumberOfTapsRequiredProperty =
			BindableProperty.Create(nameof(NumberOfTapsRequired), typeof(int), typeof(RotateGestureRecognizer), 1);

		public static readonly BindableProperty StartedCommandProperty =
			BindableProperty.Create(nameof(StartedCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty StartedCommandParameterProperty =
			BindableProperty.Create(nameof(StartedCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty CancelledCommandProperty =
			BindableProperty.Create(nameof(CancelledCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty CancelledCommandParameterProperty =
			BindableProperty.Create(nameof(CancelledCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty IsRotatingPropertyProperty =
			BindableProperty.Create(nameof(IsRotating), typeof(bool), typeof(RotateGestureRecognizer), false);

		double _total;

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

		public bool IsRotating
		{
			get => (bool)GetValue(IsRotatingPropertyProperty);
			set => SetValue(IsRotatingPropertyProperty, value);
		}

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

		public event EventHandler<RotateGestureUpdatedEventArgs> Rotated;

		public override void OnTouch(View sender, TouchEventArgs eventArgs)
		{
			if (TouchCount != 2)
			{
				if(_total != 0)
				{
					_total = 0;
					IsRotating = false;
					Rotated?.Invoke(this, new RotateGestureUpdatedEventArgs(_total, 0, sender.Bounds.Center));
					CancelledCommand.Run(CancelledCommandParameter);
				}
				
				return;
			}


			if (Touches[0].TouchPoints.Count > 1 && Touches[1].TouchPoints.Count > 1 && _total == 0)
			{
				IsRotating = true;
				StartedCommand.Run(StartedCommandParameter);
			}

			TouchPoint a1 = Touches[0].TouchPoints.First();
			TouchPoint b1 = Touches[1].TouchPoints.First();

			TouchPoint a2 = Touches[0].TouchPoints.Last();
			TouchPoint b2 = Touches[1].TouchPoints.Last();

			var d1 = new Point(a1.Point.X - b1.Point.X, a1.Point.Y - b1.Point.Y);
			var d2 = new Point(a2.Point.X - b2.Point.X, a2.Point.Y - b2.Point.Y);

			var delta = Math.Atan2(d2.Y, d2.X) - Math.Atan2(d1.Y, d1.X);
			_total += delta;

			Rotated?.Invoke(this, new RotateGestureUpdatedEventArgs(_total, delta, new Point(d2.X - d1.X, d2.Y - d1.Y)));
		}

		
	}
}