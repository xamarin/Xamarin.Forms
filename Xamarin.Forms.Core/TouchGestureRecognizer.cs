using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public sealed class TouchGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(TapGestureRecognizer), null);


		public TouchGestureRecognizer()
		{
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}



		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendTouch(View sender, TouchEventArgs eventArgs)
		{
			VisualStateManager.GoToState(sender, eventArgs.TouchState.ToString());
			TouchUpdated?.Invoke(this, eventArgs);
		}

		public event EventHandler<TouchEventArgs> TouchUpdated;
		public IList<TouchPoint> TouchPoints { get; set; }


	}
}