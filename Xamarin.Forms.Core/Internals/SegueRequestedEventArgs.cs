using System.ComponentModel;
using System.Threading.Tasks;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class SegueRequestedEventArgs
	{
		public ValueSegue Segue { get; }
		public SegueTarget Target { get; }
		public Task Task { get; set; }
		public bool Handled { get; set; }

		internal SegueRequestedEventArgs(ValueSegue segue, SegueTarget target)
		{
			Segue = segue;
			Target = target;
		}
	}
}