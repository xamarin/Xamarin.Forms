using System;
using System.Windows.Input;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	static class ButtonElement
	{
		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(IButtonElement), null, propertyChanging: OnCommandChanging, propertyChanged: OnCommandChanged);

		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(IButtonElement), null,
			propertyChanged: (bindable, oldvalue, newvalue) => CommandCanExecuteChanged(bindable, EventArgs.Empty));

		static void OnCommandChanged(BindableObject bo, object o, object n)
		{
			if (n is ICommand newCommand && bo is IButtonController button)
				newCommand.CanExecuteChanged += button.OnCommandCanExecuteChanged;

			CommandChanged((IButtonElement)bo);
		}

		static void OnCommandChanging(BindableObject bo, object o, object n)
		{
			if (o != null && bo is IButtonController button)
			{
				(o as ICommand).CanExecuteChanged -= button.OnCommandCanExecuteChanged;
			}
		}



		public const string PressedVisualState = "Pressed";

		public static void CommandChanged(IButtonElement sender)
		{
			if (sender.Command != null)
			{
				CommandCanExecuteChanged(sender, EventArgs.Empty);
			}
			else
			{
				sender.IsEnabledCore = true;
			}
		}

		public static void CommandCanExecuteChanged(object sender, EventArgs e)
		{
			IButtonElement ButtonElementManager = (IButtonElement)sender;
			ICommand cmd = ButtonElementManager.Command;
			if (cmd != null)
			{
				ButtonElementManager.IsEnabledCore = cmd.CanExecute(ButtonElementManager.CommandParameter);
			}
		}


		public static void ElementClicked(VisualElement visualElement, IButtonElement ButtonElementManager)
		{
			if (visualElement.IsEnabled == true)
			{
				IButtonController buttonController = ButtonElementManager as IButtonController;
				ButtonElementManager.Command?.Execute(ButtonElementManager.CommandParameter);
				buttonController?.PropagateUpClicked();
			}
		}

		public static void ElementPressed(VisualElement visualElement, IButtonElement ButtonElementManager)
		{
			if (visualElement.IsEnabled == true)
			{
				IButtonController buttonController = ButtonElementManager as IButtonController;
				buttonController?.SetIsPressed(true);
				visualElement.ChangeVisualStateInternal();
				buttonController?.PropagateUpPressed();
			}
		}

		public static void ElementReleased(VisualElement visualElement, IButtonElement ButtonElementManager)
		{
			if (visualElement.IsEnabled == true)
			{
				IButtonController buttonController = ButtonElementManager as IButtonController;
				buttonController?.SetIsPressed(false);
				visualElement.ChangeVisualStateInternal();
				buttonController?.PropagateUpReleased();
			}
		}
	}
}
