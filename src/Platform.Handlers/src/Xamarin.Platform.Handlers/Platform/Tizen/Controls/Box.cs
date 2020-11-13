using System;
using ElmSharp;
using EBox = ElmSharp.Box;

namespace Xamarin.Platform.Tizen
{
	public class Box : EBox
	{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		public Box(EvasObject parent) : base(parent)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		{
			SetLayoutCallback(() => { NotifyOnLayout(); });
		}

		public event EventHandler<LayoutEventArgs> LayoutUpdated;

		void NotifyOnLayout()
		{
			LayoutUpdated?.Invoke(this, new LayoutEventArgs() { Geometry = Geometry });
		}
	}
}
