using System.Windows.Input;
using System.ComponentModel;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICommandableElement
	{
		ICommand Command { get; set; }
		object CommandParameter { get; set; }
	}
}
