using System.Windows.Input;

namespace Xamarin.Forms.Core.Internals
{
	static class CommandExtensions
	{
		public static void Run(this ICommand command)
		{
			if (command?.CanExecute(null) == true)
			{
				command.Execute(null);
			}
		}

		public static void Run(this ICommand command, object parameter)
		{
			if (command?.CanExecute(parameter) == true)
			{
				command.Execute(parameter);
			}
		}
	}
}