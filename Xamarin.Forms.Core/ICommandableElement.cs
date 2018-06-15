using System.Windows.Input;
using System.ComponentModel;

namespace Xamarin.Forms.Internals
{
	// implementing classes must be castable to Element
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICommandableElement
	{
		ICommand Command { get; set; }
		object CommandParameter { get; set; }
	}
}
