using System.Collections.Generic;
using System.Diagnostics;
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
					Color.Aqua
				}
			};

			lv.BackgroundColor = Color.Gray;
			lv.ItemTemplate = new DataTemplate(typeof(_40139ViewCell));

			lv.ItemSelected += (sender, args) =>
			{
				Debug.WriteLine($">>>>> Bugzilla40139 Init 41: {lv.SelectedItem}");
				lv.InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);
			};

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
				label.FontFamily = "Consolas";
				label.FontSize = 24;
				//label.LineBreakMode = LineBreakMode.MiddleTruncation;
				label.BackgroundColor = Color.Chartreuse;

				var entry = new Entry();
				entry.Placeholder = "Placeholder";
				entry.TextColor = Color.Coral;

				var button = new Button();
				button.Text = "Button";
				button.TextColor = Color.Coral;

				var layout = new StackLayout();
				layout.Children.Add(label);

				layout.Children.Add(entry);
				layout.Children.Add(button);

				var image = new Image();
				image.Source = "coffee.png";
				layout.Children.Add(image);

				View = layout;
			}
		}
	}
}