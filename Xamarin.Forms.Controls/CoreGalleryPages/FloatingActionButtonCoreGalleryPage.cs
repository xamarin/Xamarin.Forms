using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls
{
	internal class FloatActionButtonCoreGalleryPage : CoreGalleryPage<FloatingActionButton>
	{
		protected override bool SupportsTapGestureRecognizer
		{
			get { return false; }
		}

		protected override bool SupportsFocus
		{
			get { return false; }
		}

		protected override void InitializeElement(FloatingActionButton element)
		{
			element.BackgroundColor = Color.Red;
			element.Size = FloatingActionButtonSize.Normal;
			element.HorizontalOptions = LayoutOptions.Center;
		}

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);

			var imageSourceContainer = new StateViewContainer<FloatingActionButton>(Test.FloatingActionButton.ImageSource, new FloatingActionButton { HorizontalOptions = LayoutOptions.Center, BackgroundColor = Color.Red, ImageSource = "oasissmall.jpg" });
			var sizeContainer = new StateViewContainer<FloatingActionButton>(Test.FloatingActionButton.Size, new FloatingActionButton { HorizontalOptions = LayoutOptions.Center, BackgroundColor = Color.Green, Size = FloatingActionButtonSize.Mini });

			Add(imageSourceContainer);
			Add(sizeContainer);
		}
	}
}