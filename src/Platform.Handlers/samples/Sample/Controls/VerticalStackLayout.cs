using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Platform.Layouts;

namespace Xamarin.Platform
{
	public class VerticalStackLayout : StackLayout
	{
		public override ILayoutManager CreateLayoutManager() => new VerticalStackLayoutManager(this);
	}
}
