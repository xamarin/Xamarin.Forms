using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public class TouchGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(RotateGestureRecognizer));

		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(RotateGestureRecognizer));

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

		public override void OnTouch(View sender, TouchEventArgs eventArgs)
		{

			if (sender.HasVisualStateGroups())
			{
				VisualStateManager.GoToState(sender, State.ToString());
			}
		}

	}
}