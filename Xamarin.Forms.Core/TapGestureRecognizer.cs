using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public sealed class TapGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(TapGestureRecognizer), null);

		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(TapGestureRecognizer), null);

		public static readonly BindableProperty NumberOfTapsRequiredProperty = BindableProperty.Create("NumberOfTapsRequired", typeof(int), typeof(TapGestureRecognizer), 1);

		public TapGestureRecognizer()
		{
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public int NumberOfTapsRequired
		{
			get { return (int)GetValue(NumberOfTapsRequiredProperty); }
			set { SetValue(NumberOfTapsRequiredProperty, value); }
		}

		public event EventHandler Tapped;

		internal void SendTapped(View sender)
		{
			ICommand cmd = Command;
			if (cmd != null && cmd.CanExecute(CommandParameter))
				cmd.Execute(CommandParameter);

			EventHandler handler = Tapped;
			if (handler != null)
				handler(sender, new TappedEventArgs(CommandParameter));

#pragma warning disable 0618 // retain until TappedCallback removed
			Action<View, object> callback = TappedCallback;
			if (callback != null)
				callback(sender, TappedCallbackParameter);
#pragma warning restore
		}

		#region obsolete cruft

		// call empty constructor to hack around bug in mono where compiler generates invalid IL
		[Obsolete("Obsolete in 1.0.2. Use Command instead")]
		public TapGestureRecognizer(Action<View, object> tappedCallback) : this()
		{
			if (tappedCallback == null)
				throw new ArgumentNullException("tappedCallback");
			TappedCallback = tappedCallback;
		}

		// call empty constructor to hack around bug in mono where compiler generates invalid IL
		[Obsolete("Obsolete in 1.0.2. Use Command instead")]
		public TapGestureRecognizer(Action<View> tappedCallback) : this()
		{
			if (tappedCallback == null)
				throw new ArgumentNullException("tappedCallback");
			TappedCallback = (s, o) => tappedCallback(s);
		}

		[Obsolete("Obsolete in 1.0.2. Use Command instead")] public static readonly BindableProperty TappedCallbackProperty = BindableProperty.Create("TappedCallback", typeof(Action<View, object>),
			typeof(TapGestureRecognizer), null);

		[Obsolete("Obsolete in 1.0.2. Use Command instead")]
		public Action<View, object> TappedCallback
		{
			get { return (Action<View, object>)GetValue(TappedCallbackProperty); }
			set { SetValue(TappedCallbackProperty, value); }
		}

		[Obsolete("Obsolete in 1.0.2. Use Command instead")] public static readonly BindableProperty TappedCallbackParameterProperty = BindableProperty.Create("TappedCallbackParameter", typeof(object),
			typeof(TapGestureRecognizer), null);

		[Obsolete("Obsolete in 1.0.2. Use Command instead")]
		public object TappedCallbackParameter
		{
			get { return GetValue(TappedCallbackParameterProperty); }
			set { SetValue(TappedCallbackParameterProperty, value); }
		}

		#endregion
	}
}