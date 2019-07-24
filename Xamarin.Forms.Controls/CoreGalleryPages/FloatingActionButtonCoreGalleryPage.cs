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
			element.HorizontalOptions = LayoutOptions.Center;
		}

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);
		}
	}
}