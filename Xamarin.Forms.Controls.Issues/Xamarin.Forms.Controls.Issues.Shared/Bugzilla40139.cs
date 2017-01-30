using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40139, "Changing the Windows 10 System Theme Color causes ListView text to disappear.", PlatformAffected.WinRT)]
	public class Bugzilla40139 : TestContentPage 
	{
		protected override void Init()
		{
			var lv = new ListView
			{
				ItemsSource = new List<Color>
				{
					Color.Aqua,
					Color.Black,
					Color.Blue,
					Color.Fuchsia,
					Color.Gray,
					Color.Green,
					Color.Lime,
					Color.Maroon,
					Color.Navy,
					Color.Olive,
					Color.Pink,
					Color.Purple,
					Color.Red,
					Color.Silver,
					Color.Teal,
					Color.White,
					Color.Yellow
				}
			};

			lv.BackgroundColor = Color.Black;
			lv.ItemTemplate = new DataTemplate(typeof(_40139ViewCell));

			Content = lv;
		}

		[Preserve(AllMembers = true)]
		public class _40139ViewCell : ViewCell
		{
			public _40139ViewCell()
			{
				var label = new Label();
				label.Text = "abc";
				label.VerticalOptions = LayoutOptions.Center;
				label.TextColor = Color.White;
				View = label;
			}
		}
	}
}