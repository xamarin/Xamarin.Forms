namespace Xamarin.Forms.Controls.GalleryPages.GradientGalleries
{
	public partial class LinearGradientBrushFlowDirectionGallery : ContentPage
	{
		public LinearGradientBrushFlowDirectionGallery()
		{
			InitializeComponent();
			BackgroundPicker.SelectedIndex = 1;

			Button.Clicked += (sender, args) =>
			{
				DisplayAlert("Events", "Button Clicked", "Ok");
			};
		}

		void OnFlowDirectionSelectedIndexChanged(object sender, System.EventArgs e)
		{
			FlowDirection flowDirection = FlowDirection.MatchParent;

			var selectedIndex = ((Picker)sender).SelectedIndex;

			switch (selectedIndex)
			{
				case 0:
					flowDirection = FlowDirection.MatchParent;
					break;
				case 1:
					flowDirection = FlowDirection.LeftToRight;
					break;
				case 2:
					flowDirection = FlowDirection.RightToLeft;
					break;
			}

			UpdateFlowDirection(flowDirection);
		}

		void UpdateFlowDirection(FlowDirection flowDirection)
		{
			Button.FlowDirection = flowDirection;
			BoxView.FlowDirection = flowDirection;
			CornerRadiusBoxView.FlowDirection = flowDirection;
			CheckBox.FlowDirection = flowDirection;
			CarouselView.FlowDirection = flowDirection;
			CollectionView.FlowDirection = flowDirection;
			DatePicker.FlowDirection = flowDirection;
			Editor.FlowDirection = flowDirection;
			Entry.FlowDirection = flowDirection;
			Frame.FlowDirection = flowDirection;
			Grid.FlowDirection = flowDirection;
			Label.FlowDirection = flowDirection;
			ListView.FlowDirection = flowDirection;
			Picker.FlowDirection = flowDirection;
			ScrollView.FlowDirection = flowDirection;
			SearchBar.FlowDirection = flowDirection;
			Slider.FlowDirection = flowDirection;
			Stepper.FlowDirection = flowDirection;
			TableView.FlowDirection = flowDirection;
			TimePicker.FlowDirection = flowDirection;
		}
	}
}