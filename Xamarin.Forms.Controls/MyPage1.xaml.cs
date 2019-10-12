using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyPage1 : ContentPage
	{
		public MyPage1()
		{
			InitializeComponent();

			lbl.Text = btn.LineBreakMode.ToString();
		}

		void Button_Clicked(object sender, EventArgs e)
		{
			btn.LineBreakMode  = SetLineBreak(); //6
			lblTxt.LineBreakMode = btn.LineBreakMode;
			lbl.Text = btn.LineBreakMode.ToString();
		}
		int count;
		LineBreakMode SetLineBreak()
		{
			count++;
			switch (count)
			{
				case 1:
					return LineBreakMode.WordWrap;

				case 2:
					return LineBreakMode.HeadTruncation;

				case 3:
					return LineBreakMode.MiddleTruncation;

				case 4:
					return LineBreakMode.NoWrap;

				case 5:
					return LineBreakMode.TailTruncation;

				default:
					count = 0;
					return LineBreakMode.CharacterWrap;
			}
		}
	}
}