using Microsoft.Maui.Controls.CustomAttributes;

namespace Microsoft.Maui.Controls.ControlGallery
{
	internal class ActivityIndicatorCoreGalleryPage : CoreGalleryPage<ActivityIndicator>
	{
		protected override bool SupportsTapGestureRecognizer
		{
			get { return true; }
		}

		protected override void InitializeElement(ActivityIndicator element)
		{
			element.IsRunning = true;
		}

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);

			var colorContainer = new ViewContainer<ActivityIndicator>(Test.ActivityIndicator.Color, new ActivityIndicator
			{
				Color = Color.Lime,
				IsRunning = true

			});

			var isRunningContainer = new StateViewContainer<ActivityIndicator>(Test.ActivityIndicator.IsRunning, new ActivityIndicator
			{
				IsRunning = true
			});

			isRunningContainer.StateChangeButton.Clicked += (sender, args) =>
			{
				isRunningContainer.View.IsRunning = !isRunningContainer.View.IsRunning;
			};

			Add(colorContainer);
			Add(isRunningContainer);
		}
	}
}