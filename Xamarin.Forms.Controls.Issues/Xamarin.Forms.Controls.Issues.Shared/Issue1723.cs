using System;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1723, "Picker's Items.Clear cause exception", PlatformAffected.WinPhone,
		NavigationBehavior.PushModalAsync)]
	public class Issue1723
		: ContentPage
	{
		Picker _picker = null;

		public Issue1723()
		{
			var layout = new StackLayout();
			Content = layout;

			_picker = new Picker();
			layout.Children.Add(_picker);

			var button = new Button();
			button.Clicked += button_Clicked;
			button.Text = "prepare magic";
			layout.Children.Add(button);
		}

		void button_Clicked(object sender, EventArgs e)
		{
			var r = new Random();

			_picker.Items.Clear();

			for (var j = 0; j < r.Next(10, 30); j++)
			{
				var sb = new StringBuilder();
				for (var k = 10; k < r.Next(15, 35); k++)
				{
					sb.Append((char)r.Next(65, 90));
				}
				_picker.Items.Add(sb.ToString());
			}
			_picker.SelectedIndex = r.Next(0, _picker.Items.Count);

			var button = (Button)sender;
			button.Text = "crash the magic";
		}
	}
}