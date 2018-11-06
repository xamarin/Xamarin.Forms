using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Xamarin.Forms
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IImageElement
	{
		Aspect Aspect { get; }
		ImageSource Source { get; }
		bool IsOpaque { get; }		
	}
}
