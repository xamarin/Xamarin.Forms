using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 12489, "Unable to get positon when dropping an item", PlatformAffected.All)]
	public class Issue12489 : ContentPage
	{
		public Issue12489()
		{
			var dragGestureRecognizers = new DragGestureRecognizer();
			var dropGestureRecognizers = new DropGestureRecognizer();
			var dropContainer = new DropGestureRecognizer();

			dragGestureRecognizers.DragStarting += DragGestureRecognizers_DragStarting;
			dropContainer.Drop += DropGestureRecognizers_Drop;
			dropContainer.AllowDrop = true;

			AbsoluteLayout absoluteLayout = new AbsoluteLayout
			{
				BackgroundColor = Color.Blue.WithLuminosity(0.9),
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			absoluteLayout.GestureRecognizers.Add(dropContainer);

			Label header = new Label
			{
				Text = "Drag the Box and drop anywhere on the screen to see it's drop location...",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center
			};

			var frame = new Frame
			{
				HeightRequest = 100,
				WidthRequest = 50,
				BackgroundColor = Color.Red
			};

			frame.GestureRecognizers.Add(dragGestureRecognizers);
			AbsoluteLayout.SetLayoutFlags(frame, AbsoluteLayoutFlags.PositionProportional);
			AbsoluteLayout.SetLayoutBounds(frame,
				new Rectangle(0.5f,
								0.5f, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));


			absoluteLayout.Children.Add(header);
			absoluteLayout.Children.Add(frame);

			Content = absoluteLayout;
		}

		private void DropGestureRecognizers_Drop(object sender, DropEventArgs e)
		{
			var X = e.DropX;
			var Y = e.DropY;
			DisplayAlert("Dropped!", $"Was dropped at X:{X}  Y:{Y}", "OK");
		}

		private void DragGestureRecognizers_DragStarting(object sender, DragStartingEventArgs e)
		{
		}
	}
}