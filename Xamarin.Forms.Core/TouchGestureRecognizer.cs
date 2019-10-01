using System.Windows.Input;
using Xamarin.Forms.Core.Internals;

namespace Xamarin.Forms
{
	public class TouchGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty PressCommandProperty =
			BindableProperty.Create(nameof(PressCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty PressCommandParameterProperty =
			BindableProperty.Create(nameof(PressCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty ReleaseCommandProperty =
			BindableProperty.Create(nameof(ReleaseCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty ReleaseCommandParameterProperty =
			BindableProperty.Create(nameof(ReleaseCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty MoveCommandProperty =
			BindableProperty.Create(nameof(MoveCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty MoveCommandParameterProperty =
			BindableProperty.Create(nameof(MoveCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty CancelCommandProperty =
			BindableProperty.Create(nameof(CancelCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty CancelCommandParameterProperty =
			BindableProperty.Create(nameof(CancelCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty FailCommandProperty =
			BindableProperty.Create(nameof(FailCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty FailCommandParameterProperty =
			BindableProperty.Create(nameof(FailCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty ChangeCommandProperty =
			BindableProperty.Create(nameof(ChangeCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty ChangeCommandParameterProperty =
			BindableProperty.Create(nameof(ChangeCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty EnterCommandProperty =
			BindableProperty.Create(nameof(EnterCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty EnterCommandParameterProperty =
			BindableProperty.Create(nameof(EnterCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty ExitCommandProperty =
			BindableProperty.Create(nameof(ExitCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty ExitCommandParameterProperty =
			BindableProperty.Create(nameof(ExitCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty HoverCommandProperty =
			BindableProperty.Create(nameof(HoverCommand), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty HoverCommandParameterProperty =
			BindableProperty.Create(nameof(HoverCommandParameter), typeof(object), typeof(RotateGestureRecognizer));

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

		public override void OnTouch(View sender, TouchEventArgs eventArgs)
		{
			if (sender.HasVisualStateGroups())
			{
				VisualStateManager.GoToState(sender, State.ToString());
			}

			switch (eventArgs.TouchState)
			{
				case TouchState.Press:
					PressCommand.Run(PressCommandProperty);
					break;
				case TouchState.Release:
					ReleaseCommand.Run(ReleaseCommandProperty);
					break;
				case TouchState.Move:
					MoveCommand.Run(MoveCommandProperty);
					break;
				case TouchState.Cancel:
					CancelCommand.Run(CancelCommandProperty);
					break;
				case TouchState.Fail:
					FailCommand.Run(FailCommandProperty);
					break;
				case TouchState.Change:
					ChangeCommand.Run(ChangeCommandProperty);
					break;
				case TouchState.Enter:
					EnterCommand.Run(EnterCommandProperty);
					break;
				case TouchState.Exit:
					ExitCommand.Run(ExitCommandProperty);
					break;
				case TouchState.Hover:
					HoverCommand.Run(HoverCommandProperty);
					break;
			}

			if (sender.HasVisualStateGroups() && TouchCount == 0)
			{
				VisualStateManager.GoToState(sender, TouchState.Default.ToString());
			}
		}
	}
}