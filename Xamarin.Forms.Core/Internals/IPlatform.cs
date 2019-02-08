using System;
using System.ComponentModel;	

 namespace Xamarin.Forms.Internals	
{	
	[EditorBrowsable(EditorBrowsableState.Never)]	
	[Obsolete("This interface is obsolete as of 3.5.0. Do not use it.")]
	public interface IPlatform	
	{	
		SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint);	
	}	
}