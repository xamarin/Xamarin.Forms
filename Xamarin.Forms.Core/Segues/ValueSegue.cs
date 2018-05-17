using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct ValueSegue
	{
		Segue segue; // may be null
		NavigationAction action;
		bool animated;

		public Segue Segue => segue;
		public NavigationAction Action => segue?.Action ?? action;
		public bool IsAnimated => segue?.IsAnimated ?? animated;
		public bool IsEnabled => segue?.IsEnabled ?? true;

		public ValueSegue(Segue segue) : this()
		{
			this.segue = segue;
		}

		public ValueSegue(NavigationAction action, bool animated)
		{
			this.segue = null;
			this.action = action;
			this.animated = animated;
		}

		public ICommand ToCommand(Element source)
		{
			if (segue != null)
				return segue.ToCommand(source);

			return Segue.CreateCommand(source, action, animated);
		}

		public bool CanExecuteCommand(Element source, object parameter)
		{
			if (segue != null)
				return segue.CanExecuteCommand(source, parameter);

			return Segue.DefaultCanExecuteCommand(this, source);
		}

		public Task ExecuteCommand(Element source, object parameter)
		{
			if (segue != null)
				return segue.ExecuteCommand(source, parameter);

			return Segue.DefaultExecuteCommand(this, source);
		}

		public static implicit operator ValueSegue(Segue seg) => new ValueSegue(seg);
	}
}
