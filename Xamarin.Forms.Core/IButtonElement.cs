using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	interface IButtonElement
	{
		//note to implementor: implement this property publicly
		object CommandParameter { get; set; }
		ICommand Command { get; set; }
		bool IsEnabledCore { set; }
		bool IsPressed { get; }
	}
}
