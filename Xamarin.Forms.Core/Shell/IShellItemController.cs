using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public interface IShellItemController : IElementController
	{
		bool ProposeSection(ShellSection shellSection, bool setValue = true);

		IReadOnlyList<ShellSection> GetItems();
		int IndexOf(ShellSection section);
		event NotifyCollectionChangedEventHandler ItemsCollectionChanged;
	}
}