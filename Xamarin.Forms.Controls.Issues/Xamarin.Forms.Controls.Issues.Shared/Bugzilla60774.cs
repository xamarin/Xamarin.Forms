using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	// Manual test to verify that ScrollOrientation.Both scrolls at the correct speed vertically
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 60774, "[Android] ScrollOrientation.Both doubles the distance of scrolling", 
		PlatformAffected.Android)]
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

	// Manual test to verify that ScrollOrientation.Both scrolls at the correct speed horizontally
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 60774, "[Android] ScrollOrientation.Both doubles the distance of scrolling", 
		PlatformAffected.Android, issueTestNumber: 1)]
	public class Bugzilla60774_1 : TestContentPage
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
			_currentOrientation = _currentOrientation == ScrollOrientation.Horizontal
				? ScrollOrientation.Both
				: ScrollOrientation.Horizontal;

			var al = new AbsoluteLayout();
			for (var i = 0; i < 100; i++)
			{
				var label = new Label { Text = $"{i} label", Margin = 10 };
				AbsoluteLayout.SetLayoutBounds(label, new Rectangle(i * 50, 50, 30, 200));
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

	// Manual test to make sure diagonal scrolling works at the correct speed 
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 60774, "[Android] ScrollOrientation.Both doubles the distance of scrolling", 
		PlatformAffected.Android, issueTestNumber: 2)]
	public class Bugzilla60774_2 : TestContentPage
	{
		Grid _host;

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

			_host = new Grid();
			Grid.SetRow(_host, 1);

			grid.Children.Add(layout);
			grid.Children.Add(_host);

			Content = grid;

			AddContent();
		}

		void AddContent()
		{
			_host.Children.Clear();

			var al = new AbsoluteLayout();
			
			var label = new Label { Text = "Make sure we can scroll this diagonally", FontSize = 72, Margin = 300 };
			AbsoluteLayout.SetLayoutBounds(label, new Rectangle(0, 0, 2000, 2000));
			al.Children.Add(label);

			var sv = new ScrollView
			{
				Orientation = ScrollOrientation.Both,
				Content = al
			};

			_host.Children.Add(sv);
		}
	}
}