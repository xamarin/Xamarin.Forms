using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public sealed class Command<T> : Command 
	{
		public Command(Action<T> execute) 
			: base(o =>
			{
				if (o is T)
				{
					execute((T)o);
				}
			})
		{
			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}
		}

		public Command(Action<T> execute, Func<T, bool> canExecute) 
			: base(o =>
			{
				if (o is T)
				{
					execute((T)o);
				}
			}, o => o is T && canExecute((T)o))
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));
			if (canExecute == null)
				throw new ArgumentNullException(nameof(canExecute));
		}
	}

	public class Command : ICommand
	{
		readonly Func<object, bool> _canExecute;
		readonly Action<object> _execute;

		public Command(Action<object> execute)
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));

			_execute = execute;
		}

		public Command(Action execute) : this(o => execute())
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));
		}

		public Command(Action<object> execute, Func<object, bool> canExecute) : this(execute)
		{
			if (canExecute == null)
				throw new ArgumentNullException(nameof(canExecute));

			_canExecute = canExecute;
		}

		public Command(Action execute, Func<bool> canExecute) : this(o => execute(), o => canExecute())
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));
			if (canExecute == null)
				throw new ArgumentNullException(nameof(canExecute));
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute != null)
				return _canExecute(parameter);

			return true;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		public void ChangeCanExecute()
		{
			EventHandler changed = CanExecuteChanged;
			changed?.Invoke(this, EventArgs.Empty);
		}
	}
}