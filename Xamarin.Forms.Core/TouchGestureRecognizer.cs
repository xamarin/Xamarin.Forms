using System;
using System.Windows.Input;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public class TouchGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty PressCommandProperty =
			BindableProperty.Create(nameof(PressCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty PressCommandParameterProperty =
			BindableProperty.Create(nameof(PressCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty ReleaseCommandProperty =
			BindableProperty.Create(nameof(ReleaseCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty ReleaseCommandParameterProperty =
			BindableProperty.Create(nameof(ReleaseCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty MoveCommandProperty =
			BindableProperty.Create(nameof(MoveCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty MoveCommandParameterProperty =
			BindableProperty.Create(nameof(MoveCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty CancelCommandProperty =
			BindableProperty.Create(nameof(CancelCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty CancelCommandParameterProperty =
			BindableProperty.Create(nameof(CancelCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty FailCommandProperty =
			BindableProperty.Create(nameof(FailCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty FailCommandParameterProperty =
			BindableProperty.Create(nameof(FailCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty ChangeCommandProperty =
			BindableProperty.Create(nameof(ChangeCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty ChangeCommandParameterProperty =
			BindableProperty.Create(nameof(ChangeCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty EnterCommandProperty =
			BindableProperty.Create(nameof(EnterCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty EnterCommandParameterProperty =
			BindableProperty.Create(nameof(EnterCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty ExitCommandProperty =
			BindableProperty.Create(nameof(ExitCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty ExitCommandParameterProperty =
			BindableProperty.Create(nameof(ExitCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty HoverCommandProperty =
			BindableProperty.Create(nameof(HoverCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty HoverCommandParameterProperty =
			BindableProperty.Create(nameof(HoverCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public event EventHandler<TouchEventArgs> Press;
		public event EventHandler<TouchEventArgs> Release;
		public event EventHandler<TouchEventArgs> Move;
		public event EventHandler<TouchEventArgs> Cancel;
		public event EventHandler<TouchEventArgs> Fail;
		public event EventHandler<TouchEventArgs> Change;
		public event EventHandler<TouchEventArgs> Enter;
		public event EventHandler<TouchEventArgs> Exit;
		public event EventHandler<TouchEventArgs> Hover;

		public ICommand CancelCommand
		{
			get => (ICommand)GetValue(CancelCommandProperty);
			set => SetValue(CancelCommandProperty, value);
		}

		public object CancelCommandParameter
		{
			get => GetValue(CancelCommandParameterProperty);
			set => SetValue(CancelCommandParameterProperty, value);
		}

		public ICommand ChangeCommand
		{
			get => (ICommand)GetValue(ChangeCommandProperty);
			set => SetValue(ChangeCommandProperty, value);
		}

		public object ChangeCommandParameter
		{
			get => GetValue(ChangeCommandParameterProperty);
			set => SetValue(ChangeCommandParameterProperty, value);
		}

		public ICommand EnterCommand
		{
			get => (ICommand)GetValue(EnterCommandProperty);
			set => SetValue(EnterCommandProperty, value);
		}

		public object EnterCommandParameter
		{
			get => GetValue(EnterCommandParameterProperty);
			set => SetValue(EnterCommandParameterProperty, value);
		}

		public ICommand ExitCommand
		{
			get => (ICommand)GetValue(ExitCommandProperty);
			set => SetValue(ExitCommandProperty, value);
		}

		public object ExitCommandParameter
		{
			get => GetValue(ExitCommandParameterProperty);
			set => SetValue(ExitCommandParameterProperty, value);
		}

		public ICommand FailCommand
		{
			get => (ICommand)GetValue(FailCommandProperty);
			set => SetValue(FailCommandProperty, value);
		}

		public object FailCommandParameter
		{
			get => GetValue(FailCommandParameterProperty);
			set => SetValue(FailCommandParameterProperty, value);
		}

		public ICommand HoverCommand
		{
			get => (ICommand)GetValue(HoverCommandProperty);
			set => SetValue(HoverCommandProperty, value);
		}

		public object HoverCommandParameter
		{
			get => GetValue(HoverCommandParameterProperty);
			set => SetValue(HoverCommandParameterProperty, value);
		}

		public ICommand MoveCommand
		{
			get => (ICommand)GetValue(MoveCommandProperty);
			set => SetValue(MoveCommandProperty, value);
		}

		public object MoveCommandParameter
		{
			get => GetValue(MoveCommandParameterProperty);
			set => SetValue(MoveCommandParameterProperty, value);
		}

		public ICommand PressCommand
		{
			get => (ICommand)GetValue(PressCommandProperty);
			set => SetValue(PressCommandProperty, value);
		}

		public object PressCommandParameter
		{
			get => GetValue(PressCommandParameterProperty);
			set => SetValue(PressCommandParameterProperty, value);
		}

		public ICommand ReleaseCommand
		{
			get => (ICommand)GetValue(ReleaseCommandProperty);
			set => SetValue(ReleaseCommandProperty, value);
		}

		public object ReleaseCommandParameter
		{
			get => GetValue(ReleaseCommandParameterProperty);
			set => SetValue(ReleaseCommandParameterProperty, value);
		}

		bool _previousTouchIsInOriginalView = true;

		public override void OnTouch(View sender, TouchEventArgs eventArgs)
		{
			if (sender.HasVisualStateGroups())
			{
				VisualStateManager.GoToState(sender, TouchState.ToString());
			}

			switch (eventArgs.TouchState)
			{
				case TouchState.Press:
					Press?.Invoke(this, eventArgs);
					PressCommand.Run(PressCommandProperty);
					break;
				case TouchState.Release:
					Release?.Invoke(this, eventArgs);
					ReleaseCommand.Run(ReleaseCommandProperty);
					break;
				case TouchState.Move:
					Move?.Invoke(this, eventArgs);
					MoveCommand.Run(MoveCommandProperty);
					break;
				case TouchState.Cancel:
					Cancel?.Invoke(this, eventArgs);
					CancelCommand.Run(CancelCommandProperty);
					break;
				case TouchState.Fail:
					Fail?.Invoke(this, eventArgs);
					FailCommand.Run(FailCommandProperty);
					break;
				case TouchState.Change:
					Change?.Invoke(this, eventArgs);
					ChangeCommand.Run(ChangeCommandProperty);
					break;
				case TouchState.Enter:
					Enter?.Invoke(this, eventArgs);
					EnterCommand.Run(EnterCommandProperty);
					break;
				case TouchState.Exit:
					Exit?.Invoke(this, eventArgs);
					ExitCommand.Run(ExitCommandProperty);
					break;
				case TouchState.Hover:
					Hover?.Invoke(this, eventArgs);
					HoverCommand.Run(HoverCommandProperty);
					break;
			}


			if (TouchCount > 0 && eventArgs.TouchState == TouchState.Move)
			{
				if (_previousTouchIsInOriginalView != eventArgs.IsInOriginalView)
				{
					if (!_previousTouchIsInOriginalView && eventArgs.IsInOriginalView)
					{
						if (sender.HasVisualStateGroups())
						{
							VisualStateManager.GoToState(sender, TouchState.Enter.ToString());
						}
						Enter?.Invoke(this, eventArgs);
						EnterCommand.Run(EnterCommandProperty);
					}
					else
					{
						if (sender.HasVisualStateGroups())
						{
							VisualStateManager.GoToState(sender, TouchState.Exit.ToString());
						}
						Exit?.Invoke(this, eventArgs);
						ExitCommand.Run(ExitCommandProperty);
					}
				}
			}

			_previousTouchIsInOriginalView = eventArgs.IsInOriginalView;

			if (sender.HasVisualStateGroups() && TouchCount == 0 && eventArgs.TouchState.IsFinishedTouch())
			{
				VisualStateManager.GoToState(sender, TouchState.Default.ToString());
			}
		}
	}
}