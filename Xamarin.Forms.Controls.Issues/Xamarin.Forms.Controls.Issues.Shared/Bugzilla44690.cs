using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading;
using System.Windows.Input;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 44690, "Item inside a ListView does not keep TranslationX after scroll", PlatformAffected.iOS)]
	public class Bugzilla1023 : TestContentPage
	{
		public List<Item44690> Items { get; set; }

		protected override void Init()
		{
			var sl = new StackLayout { Margin = new Thickness(0, 20, 0, 0) };
			var l = new Label
			{
				FontSize = 9,
				Text = "Click on the black button to move the ViewCell.Grid.TranslationX to -50 Scroll the listview so the moved cell is out of the screen Scroll back The cell should be at - 50 but it is at 0.The TranslationX is still at - 50"
			};
			sl.Children.Add(l);

			Items = new List<Item44690>();
			for (var i = 0; i < 50; i++)
				Items.Add(new Item44690 { Text = $"Item {i}" });

			var listView = new ListView
			{
				ItemsSource = Items,
				HasUnevenRows = true,
				RowHeight = 80,
				SeparatorVisibility = SeparatorVisibility.None,
				BackgroundColor = Color.Silver,
				ItemTemplate = new DataTemplate(() =>
				{
					var viewCell = new ViewCell();

					var grid = new Grid
					{
						BackgroundColor = Color.White,
						Margin = 5
					};
					grid.SetBinding(Grid.TranslationXProperty, new Binding("PosX"));

					var stackLayout = new StackLayout();

					var label = new Label();
					label.SetBinding(Label.TextProperty, new Binding("Text"));
					stackLayout.Children.Add(label);

					var button = new Button
					{
						BackgroundColor = Color.Black,
						HorizontalOptions = LayoutOptions.Center,
						TextColor = Color.Red
					};
					button.SetBinding(Button.CommandProperty, new Binding("MoveCommand"));
					button.SetBinding(Button.TextProperty, new Binding("PosX"));
					stackLayout.Children.Add(button);

					grid.Children.Add(stackLayout);
					viewCell.View = grid;
					return viewCell;
				})
			};
			sl.Children.Add(listView);

			Content = sl;
		}
	}

	[Preserve(AllMembers = true)]
	public class Item44690 : INotifyPropertyChanged
	{
		public string Text { get; set; }
		public double PosX { get; set; }

		public ICommand MoveCommand => new Command(Move);

		void Move()
		{
			PosX = PosX < 0 ? 0 : -50;
			OnPropertyChanged("PosX");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}