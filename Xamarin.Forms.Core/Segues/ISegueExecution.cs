using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISegueExecution
	{
		Task<bool> OnBeforeExecute(SegueTarget target);
	}
}
