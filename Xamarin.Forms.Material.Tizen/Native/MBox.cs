using System;
using ElmSharp;
using Xamarin.Forms.Platform.Tizen.Native;
using Tizen.NET.MaterialComponents;

namespace Xamarin.Forms.Material.Tizen.Native
{
	public class MBox : MCard
	{
		Rect _previousGeometry;

		public MBox(EvasObject parent) : base(parent)
		{
			SetLayoutCallback(() => { NotifyOnLayout(); });
		}

		public event EventHandler<LayoutEventArgs> LayoutUpdated;

		void NotifyOnLayout()
		{
			if (null != LayoutUpdated)
			{
				var g = Geometry;

				if (0 == g.Width || 0 == g.Height || g == _previousGeometry)
				{
					// ignore irrelevant dimensions
					return;
				}

				LayoutUpdated(this, new LayoutEventArgs()
				{
					Geometry = g,
				}
				);

				_previousGeometry = g;
			}
		}
	}
}