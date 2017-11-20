using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 960774, "Some scrolling thing", PlatformAffected.Android)]
	public class Bugzilla60774 : TestContentPage
	{
		ScrollOrientation _currentOrientation;
		Grid _host;
		Label _labelOrientation;

		protected override void Init()
		{
			var grid = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition()
				}
			};


			var layout = new StackLayout();

			var button = new Button { Text = "Change Orientation" };
			button.Clicked += (sender, args) => ChangeOrientation();

			_labelOrientation = new Label { Text = "" };

			layout.Children.Add(button);
			layout.Children.Add(_labelOrientation);

			_host = new Grid();
			Grid.SetRow(_host, 1);

			grid.Children.Add(layout);
			grid.Children.Add(_host);

			Content = grid;

			_currentOrientation = ScrollOrientation.Both;
			ChangeOrientation();
		}

		void ChangeOrientation()
		{
			_host.Children.Clear();
			_currentOrientation = _currentOrientation == ScrollOrientation.Vertical
				? ScrollOrientation.Both
				: ScrollOrientation.Vertical;

			var al = new AbsoluteLayout();
			for (var i = 0; i < 100; i++)
			{
				var label = new Label { Text = "label " + i };
				AbsoluteLayout.SetLayoutBounds(label, new Rectangle(0, i * 50, 100, 30));
				al.Children.Add(label);
			}

			var sv = new ScrollView
			{
				Orientation = _currentOrientation,
				Content = al
			};

			_host.Children.Add(sv);
			_labelOrientation.Text = "Current orientation is: " + _currentOrientation;
		}
	}
}