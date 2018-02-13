using System.Collections.Generic;

namespace Xamarin.Forms.Internals
{
	public interface IGestureElement
	{
		IList<IGestureChildElement> ChildElementOverrides(Point point);
		
		IList<IGestureRecognizer> CompositeGestureRecognizers { get; }		
	}
}