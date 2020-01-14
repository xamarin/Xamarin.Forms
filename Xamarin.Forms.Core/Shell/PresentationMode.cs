using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms
{
	[Flags]
	public enum PresentationMode
	{
		None = 0,
		NotAnimated = 1,
		Modal = 1 << 1
	}
}
