using System;
using ElmSharp;

namespace Xamarin.Platform.Tizen
{
	public class LayoutEventArgs : EventArgs
	{
		public Rect Geometry
		{
			get;
			internal set;
		}
	}
}
