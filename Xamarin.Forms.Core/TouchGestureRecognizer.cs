using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public class TouchGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty PressedCommandProperty = BindableProperty.Create(nameof(PressedCommand), typeof(ICommand), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty ReleasedCommandProperty = BindableProperty.Create(nameof(ReleasedCommand), typeof(ICommand), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty MoveCommandProperty = BindableProperty.Create(nameof(MoveCommand), typeof(ICommand), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty CancelledCommandProperty = BindableProperty.Create(nameof(CancelledCommand), typeof(ICommand), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty FailedCommandProperty = BindableProperty.Create(nameof(FailedCommand), typeof(ICommand), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty ChangedCommandProperty = BindableProperty.Create(nameof(ChangedCommand), typeof(ICommand), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty EnteredCommandProperty = BindableProperty.Create(nameof(EnteredCommand), typeof(ICommand), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty ExitedCommandProperty = BindableProperty.Create(nameof(ExitedCommand), typeof(ICommand), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty HoverCommandProperty = BindableProperty.Create(nameof(HoverCommand), typeof(ICommand), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty PressedCommandParameterProperty = BindableProperty.Create(nameof(PressedCommandParameter), typeof(object), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty ReleasedCommandParameterProperty = BindableProperty.Create(nameof(ReleasedCommandParameter), typeof(object), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty MoveCommandParameterProperty = BindableProperty.Create(nameof(MoveCommandParameter), typeof(object), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty CancelledCommandParameterProperty = BindableProperty.Create(nameof(CancelledCommandParameter), typeof(object), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty FailedCommandParameterProperty = BindableProperty.Create(nameof(FailedCommandParameter), typeof(object), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty ChangedCommandParameterProperty = BindableProperty.Create(nameof(ChangedCommandParameter), typeof(object), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty EnteredCommandParameterProperty = BindableProperty.Create(nameof(EnteredCommandParameter), typeof(object), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty ExitedCommandParameterProperty = BindableProperty.Create(nameof(ExitedCommandParameter), typeof(object), typeof(TouchGestureRecognizer));
		public static readonly BindableProperty HoverCommandParameterProperty = BindableProperty.Create(nameof(HoverCommandParameter), typeof(object), typeof(TouchGestureRecognizer));

		public static readonly BindableProperty UseVisualStateManagerProperty = BindableProperty.Create(nameof(UseVisualStateManager), typeof(bool), typeof(TouchGestureRecognizer), true);

		public ICommand PressedCommand
		{
			get { return (ICommand)GetValue(PressedCommandProperty); }
			set { SetValue(PressedCommandProperty, value); }
		}

		public ICommand ReleasedCommand
		{
			get { return (ICommand)GetValue(ReleasedCommandProperty); }
			set { SetValue(ReleasedCommandProperty, value); }
		}

		public ICommand MoveCommand
		{
			get { return (ICommand)GetValue(MoveCommandProperty); }
			set { SetValue(MoveCommandProperty, value); }
		}

		public ICommand CancelledCommand
		{
			get { return (ICommand)GetValue(CancelledCommandProperty); }
			set { SetValue(CancelledCommandProperty, value); }
		}

		public ICommand FailedCommand
		{
			get { return (ICommand)GetValue(FailedCommandProperty); }
			set { SetValue(FailedCommandProperty, value); }
		}

		public ICommand ChangedCommand
		{
			get { return (ICommand)GetValue(ChangedCommandProperty); }
			set { SetValue(ChangedCommandProperty, value); }
		}

		public ICommand EnteredCommand
		{
			get { return (ICommand)GetValue(EnteredCommandProperty); }
			set { SetValue(EnteredCommandProperty, value); }
		}

		public ICommand ExitedCommand
		{
			get { return (ICommand)GetValue(ExitedCommandProperty); }
			set { SetValue(ExitedCommandProperty, value); }
		}

		public ICommand HoverCommand
		{
			get { return (ICommand)GetValue(HoverCommandProperty); }
			set { SetValue(HoverCommandProperty, value); }
		}

		public object PressedCommandParameter
		{
			get { return GetValue(PressedCommandParameterProperty); }
			set { SetValue(PressedCommandParameterProperty, value); }
		}

		public object ReleasedCommandParameter
		{
			get { return GetValue(ReleasedCommandParameterProperty); }
			set { SetValue(ReleasedCommandParameterProperty, value); }
		}

		public object MoveCommandParameter
		{
			get { return GetValue(MoveCommandParameterProperty); }
			set { SetValue(MoveCommandParameterProperty, value); }
		}

		public object CancelledCommandParameter
		{
			get { return GetValue(CancelledCommandParameterProperty); }
			set { SetValue(CancelledCommandParameterProperty, value); }
		}

		public object FailedCommandParameter
		{
			get { return GetValue(FailedCommandParameterProperty); }
			set { SetValue(FailedCommandParameterProperty, value); }
		}

		public object ChangedCommandParameter
		{
			get { return GetValue(ChangedCommandParameterProperty); }
			set { SetValue(ChangedCommandParameterProperty, value); }
		}

		public object EnteredCommandParameter
		{
			get { return GetValue(EnteredCommandParameterProperty); }
			set { SetValue(EnteredCommandParameterProperty, value); }
		}

		public object ExitedCommandParameter
		{
			get { return GetValue(ExitedCommandParameterProperty); }
			set { SetValue(ExitedCommandParameterProperty, value); }
		}

		public object HoverCommandParameter
		{
			get { return GetValue(HoverCommandParameterProperty); }
			set { SetValue(HoverCommandParameterProperty, value); }
		}

		public bool UseVisualStateManager
		{
			get { return (bool)GetValue(UseVisualStateManagerProperty); }
			set { SetValue(UseVisualStateManagerProperty, value); }
		}



		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SendTouch(View sender, TouchEventArgs eventArgs)
		{
			if (UseVisualStateManager)
			{
				VisualStateManager.GoToState(sender, eventArgs.TouchState.ToString());
			}
			
			TouchUpdated?.Invoke(sender, eventArgs);

			switch (eventArgs.TouchState)
			{
				case TouchState.Pressed:
					PressedCommand?.Run(HoverCommandParameter);
					break;
				case TouchState.Released:
					ReleasedCommand?.Run(HoverCommandParameter);
					break;
				case TouchState.Move:
					MoveCommand?.Run(HoverCommandParameter);
					break;
				case TouchState.Cancelled:
					CancelledCommand?.Run(HoverCommandParameter);
					break;
				case TouchState.Failed:
					FailedCommand?.Run(HoverCommandParameter);
					break;
				case TouchState.Changed:
					ChangedCommand?.Run(HoverCommandParameter);
					break;
				case TouchState.Entered:
					EnteredCommand?.Run(HoverCommandParameter);
					break;
				case TouchState.Exited:
					ExitedCommand?.Run(HoverCommandParameter);
					break;
				case TouchState.Hover:
					HoverCommand?.Run(HoverCommandParameter);
					break;
			}

		}

		public event EventHandler<TouchEventArgs> TouchUpdated;

		public IList<TouchPoint> TouchPoints { get; } = new List<TouchPoint>();
	}
}