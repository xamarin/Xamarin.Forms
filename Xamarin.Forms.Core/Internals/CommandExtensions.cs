using System.Windows.Input;

namespace Xamarin.Forms.Internals
{
	static class CommandExtensions
	{
		public static void Run(this ICommand command)
		{
			if (command != null && command.CanExecute(null))
			{
				command.Execute(null);
			}
		}

		public static void Run(this ICommand command, object parameter)
		{
			if (command != null && command.CanExecute(parameter))
			{
				command.Execute(parameter);
			}
		}
	}
}