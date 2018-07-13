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
		}

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);

			stackLayout.Children.Add(new FloatingActionButton
			{
				BackgroundColor = Color.Blue,
				Size = FloatingActionButtonSize.Mini,
				ImageSource = new FileImageSource { File = "bank.png" },
				Command = new Command(() => DisplayActionSheet("Command on Mini Floating Button", "Cancel", "Destroy"))
			});

			stackLayout.Children.Add(new FloatingActionButton
			{
				BackgroundColor = Color.Green,
				Size = FloatingActionButtonSize.Normal,
				ImageSource = new FileImageSource { File = "coffee.png" },
				Command = new Command(() => DisplayActionSheet("Command on Normal Floating Button", "Cancel", "Destroy"))
			});
		}
	}
}