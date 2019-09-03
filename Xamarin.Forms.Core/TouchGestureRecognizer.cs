using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xamarin.Forms
{
	public class TouchGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty UseVisualStateManagerProperty =
			BindableProperty.Create(nameof(UseVisualStateManager), typeof(bool), typeof(TouchGestureRecognizer), true);

		public static readonly BindableProperty StateProperty =
			BindableProperty.Create(nameof(TouchState), typeof(TouchState), typeof(TouchGestureRecognizer), TouchState.Default);

		public List<TouchPoint> TouchPoints { get; } = new List<TouchPoint>();

		public bool UseVisualStateManager
		{
			get => (bool)GetValue(UseVisualStateManagerProperty);
			set => SetValue(UseVisualStateManagerProperty, value);
		}

		public TouchState State
		{
			get => (TouchState)GetValue(StateProperty);
			set => SetValue(StateProperty, value);
		}

		protected TouchState PreviousState { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendTouch(View sender, TouchEventArgs eventArgs)
		{
			PreviousState = State;
			State = eventArgs.TouchState;
			TouchPoints.AddRange(eventArgs.TouchPoints);
			TouchUpdated?.Invoke(sender, eventArgs);
			OnTouch(sender, eventArgs);
			if (UseVisualStateManager)
			{
				VisualStateManager.GoToState(sender, State.ToString());
			}
		}

		public event EventHandler<TouchEventArgs> TouchUpdated;

		public virtual void OnTouch(View sender, TouchEventArgs eventArgs)
		{
			//Add your custom logic here.
		}
	}
}