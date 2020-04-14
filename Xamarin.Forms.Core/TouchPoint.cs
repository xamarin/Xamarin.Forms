using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public class TouchPoint : BindableObject
	{
		static readonly BindablePropertyKey TouchIndexPropertyKey =
			BindableProperty.CreateReadOnly(nameof(TouchIndex), typeof(int), typeof(TouchPoint), 0);

		public static readonly BindableProperty TouchIndexProperty = TouchIndexPropertyKey.BindableProperty;

		static readonly BindablePropertyKey IsTouchingPropertyKey =
			BindableProperty.CreateReadOnly(nameof(IsTouching), typeof(bool), typeof(TouchPoint), false);

		public static readonly BindableProperty IsTouchingProperty = IsTouchingPropertyKey.BindableProperty;

		public static readonly BindableProperty StartedCommandProperty =
			BindableProperty.Create(nameof(StartedCommand), typeof(ICommand), typeof(TouchPoint));

		public static readonly BindableProperty StartedCommandParameterProperty =
			BindableProperty.Create(nameof(StartedCommand), typeof(object), typeof(TouchPoint));

		public static readonly BindableProperty CancelledCommandProperty =
			BindableProperty.Create(nameof(CancelledCommand), typeof(ICommand), typeof(TouchPoint));

		public static readonly BindableProperty CancelledCommandParameterProperty =
			BindableProperty.Create(nameof(CancelledCommand), typeof(object), typeof(TouchPoint));

		public static readonly BindableProperty CompletedCommandCommandProperty =
			BindableProperty.Create(nameof(CompletedCommandCommand), typeof(ICommand), typeof(TouchPoint));

		public static readonly BindableProperty CompletedCommandCommandParameterProperty =
			BindableProperty.Create(nameof(CompletedCommandCommand), typeof(object), typeof(TouchPoint));

		public ICommand CancelledCommand
		{
			get { return (ICommand)GetValue(CancelledCommandProperty); }
			set { SetValue(CancelledCommandProperty, value); }
		}

		public object CancelledCommandParameter
		{
			get { return GetValue(CancelledCommandParameterProperty); }
			set { SetValue(CancelledCommandParameterProperty, value); }
		}

		public ICommand CompletedCommandCommand
		{
			get { return (ICommand)GetValue(CompletedCommandCommandProperty); }
			set { SetValue(CompletedCommandCommandProperty, value); }
		}

		public object CompletedCommandCommandParameter
		{
			get { return GetValue(CompletedCommandCommandParameterProperty); }
			set { SetValue(CompletedCommandCommandParameterProperty, value); }
		}

		public bool IsTouching
		{
			get { return (bool)GetValue(TouchIndexProperty); }
			internal set { SetValue(TouchIndexPropertyKey, value); }
		}

		public ICommand StartedCommand
		{
			get { return (ICommand)GetValue(StartedCommandProperty); }
			set { SetValue(StartedCommandProperty, value); }
		}

		public object StartedCommandParameter
		{
			get { return GetValue(StartedCommandParameterProperty); }
			set { SetValue(StartedCommandParameterProperty, value); }
		}

		public int TouchIndex
		{
			get { return (int)GetValue(TouchIndexProperty); }
			internal set { SetValue(TouchIndexPropertyKey, value); }
		}

		public event EventHandler<TouchPointEventArgs> TouchPointUpdated { get; }
	}
}