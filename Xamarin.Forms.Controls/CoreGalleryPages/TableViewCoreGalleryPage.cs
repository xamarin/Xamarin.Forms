using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls
{
	internal class TableViewCoreGalleryPage : CoreGalleryPage<TableView>
	{
		// TODO
		protected override bool SupportsFocus
		{
			get { return false; }
		}

		protected override void Build (StackLayout stackLayout)
		{
			base.Build (stackLayout);

			var tableSectionContainer = new ViewContainer<TableView>(Test.TableView.TableSection, new TableView());
			var section = new TableSection("Test")
			{
				TextColor = Color.Red
			};

			section.Add(new TextCell { Text = "Worked!" });

			var section1 = new TableSection("Testing")
			{
				TextColor = Color.Green
			};

			section1.Add(new TextCell { Text = "Workeding!" });

			var section2 = new TableSection("Test old")
			{
				new TextCell { Text = "Worked old!" }
			};

			tableSectionContainer.View.Root.Add(section);
			tableSectionContainer.View.Root.Add(section1);
			tableSectionContainer.View.Root.Add(section2);

			var cellBackgroundColorContainer = new ViewContainer<TableView>(Test.TableView.CellBackgroundColor, new TableView());

			var section3 = new TableSection("Test")
			{
				TextColor = Color.Red
			};

			var textCell = new TextCell()
			{
				Text = "Red",
				BackgroundColor = Color.Red
			};

			textCell.Tapped += (s, a) =>
			{
				textCell.Text = "Hotpink";
				textCell.BackgroundColor = Color.HotPink;
			};

			section3.Add(textCell);

			var switchCell = new SwitchCell()
			{
				Text = "Switch me!",
				BackgroundColor = Color.Yellow,
				On = true
			};

			switchCell.OnChanged += (s, a) =>
			{
				if (a.Value)
				{
					switchCell.BackgroundColor = Color.Yellow;
				}
				else
				{
					switchCell.BackgroundColor = Color.HotPink;
				}
			};

			section3.Add(switchCell);

			var entryCell = new EntryCell()
			{
				Text = "Blue",
				BackgroundColor = Color.Blue
			};

			entryCell.Tapped += (s, a) =>
			{
				entryCell.Text = "Hotpink";
				entryCell.BackgroundColor = Color.HotPink;
			};

			section3.Add(entryCell);

			var imageCell = new ImageCell()
			{
				ImageSource = "xamarin_logo",
				Text = "Purple",
				BackgroundColor = Color.Purple
			};

			imageCell.Tapped += (s, a) =>
			{
				imageCell.Text = "Hotpink";
				imageCell.BackgroundColor = Color.HotPink;
			};

			section3.Add(imageCell);

			var viewCell = new ViewCell()
			{
				View = new Label { Text = "Black or Hotpink" },
				BackgroundColor = Color.Black
			};

			viewCell.Tapped += (s, a) =>
			{
				viewCell.BackgroundColor = Color.HotPink;
			};

			section3.Add(viewCell);

			cellBackgroundColorContainer.View.Root.Add(section3);

			Add(tableSectionContainer);
			Add(cellBackgroundColorContainer);
		}
	}
}