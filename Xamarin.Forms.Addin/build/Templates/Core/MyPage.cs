using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ${Namespace}
{
	public class MyPage : ContentPage
	{
		public MyPage ()
		{
			var button = new Button {
				Text = "Click Me!",
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};

			int clicked = 0;
			button.Clicked += (s, e) => button.Text = "Clicked: " + clicked++;

			Content = button;
		}
	}
}
