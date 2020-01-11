using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public interface IShellItemController : IElementController
	{
		event EventHandler<ShellSection> Reselected;

		bool ProposeSection(ShellSection shellSection, bool setValue = true);

		void SendReselected();
	}
}