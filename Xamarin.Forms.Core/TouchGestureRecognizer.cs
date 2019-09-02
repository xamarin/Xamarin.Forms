using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public class TouchGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(TapGestureRecognizer), null);

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}



		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SendTouch(View sender, TouchEventArgs eventArgs)
		{
			VisualStateManager.GoToState(sender, eventArgs.TouchState.ToString());
			TouchUpdated?.Invoke(sender, eventArgs);
		}

		public event EventHandler<TouchEventArgs> TouchUpdated;
		public IList<TouchPoint> TouchPoints { get; set; }


	}
}