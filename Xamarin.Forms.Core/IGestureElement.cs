using System.Collections.Generic;

namespace Xamarin.Forms.Internals
{
	public interface IGestureElement
	{
		IGestureChildElement GetChildElement(Point point);
		
		IList<IGestureRecognizer> CompositeGestureRecognizers { get; }		
	}
}