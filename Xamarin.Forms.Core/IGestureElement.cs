using System.Collections.Generic;

namespace Xamarin.Forms.Internals
{
	public interface IGestureElement
	{
		IList<IGestureChildElement> GetChildElements(Point point);
		
		IList<IGestureRecognizer> CompositeGestureRecognizers { get; }		
	}
}