using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms
{
	public class RotateGestureUpdatedEventArgs 
	{

		public RotateGestureUpdatedEventArgs(double total, double delta, Point center)
		{
			Total = total;
			Delta = delta;
			Center = center;
		}

		public double Total { get;}
		public double Delta { get; }
		public Point Center { get; }
	}
}
