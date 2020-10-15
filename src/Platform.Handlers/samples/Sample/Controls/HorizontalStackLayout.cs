using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Platform.Layouts;

namespace Xamarin.Platform
{
	public class HorizontalStackLayout : StackLayout
	{
		public override ILayoutManager CreateLayoutManager() => new HorizontalStackLayoutManager(this);
	}
}
