using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14764, "[Bug] [Regression] UWP Button breaks the Layout", PlatformAffected.UWP)]
	public class Issue14764 : TestContentPage
	{
		bool _isVisible;

		public bool IsButtonVisible
		{
			get => _isVisible;
			set
			{
				if (_isVisible == value)
					return;

				_isVisible = value;
				OnPropertyChanged();
			}
		}

		protected override void Init()
		{
			var grid = new Grid
			{
				BackgroundColor = Color.AliceBlue
			};

			grid.AddRowDef(type: GridUnitType.Auto, count: 4);
			grid.AddRowDef(type: GridUnitType.Star, count: 1);

			var button1 = new Button
			{
				Text = "Toggle Visibility"
			};

			button1.SetBinding(Button.CommandProperty, new Binding("ToggleVisibilityCommand"));

			grid.Children.Add(button1, 0, 0);

			var button2 = new Button
			{
				Text = "Button 2",
				BackgroundColor = Color.Blue,
				TextColor = Color.White
			};
			button2.SetBinding(IsVisibleProperty, new Binding("IsButtonVisible"));

			grid.Children.Add(button2, 0, 1);

			var button3 = new Button
			{
				Text = "Button 3",
				BackgroundColor = Color.Yellow
			};
			button3.SetBinding(IsVisibleProperty, new Binding("IsButtonVisible"));

			grid.Children.Add(button3, 0, 2);

			var button4 = new Button
			{
				Text = "Button 4",
				BackgroundColor = Color.Red
			};
			button4.SetBinding(IsVisibleProperty, new Binding("IsButtonVisible"));

			grid.Children.Add(button4, 0, 4);

			var label = new Label
			{
				Text = "End alignment",
				VerticalOptions = LayoutOptions.End,
				HorizontalOptions = LayoutOptions.End,
				BackgroundColor = Color.Red
			};

			grid.Children.Add(label, 0, 5);

			Content = grid;
		}

		public ICommand ToggleVisibilityCommand { get; }

		public Issue14764()
		{
			ToggleVisibilityCommand = new Command(() => IsButtonVisible = !IsButtonVisible);
			BindingContext = this;
		}
	}
}
