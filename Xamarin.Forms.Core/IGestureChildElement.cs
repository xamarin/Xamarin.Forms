using System.Collections.Generic;

namespace Xamarin.Forms.Internals
{
	public interface IGestureChildElement
    {
		IList<IGestureRecognizer> GestureRecognizers { get; }
	}
}
