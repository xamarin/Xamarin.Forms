using System;
using Tizen.UIExtensions.ElmSharp;

namespace Microsoft.Maui
{
	public class LayoutCanvas : Canvas
	{
		public LayoutCanvas(ElmSharp.EvasObject parent) : base(parent)
		{
		}

		internal Func<double, double, Size>? CrossPlatformMeasure { get; set; }
		internal Action<Rectangle>? CrossPlatformArrange { get; set; }
	}
}
