using Xamarin.Platform;

namespace Xamarin.Forms
{
	public partial class Button : IButton
	{
		public TextAlignment HorizontalTextAlignment => throw new NotImplementedException();

		public TextAlignment VerticalTextAlignment => throw new NotImplementedException();

		void IButton.Clicked()
		{
			(this as IButtonController).SendClicked();
		}

		void IButton.Pressed()
		{
			(this as IButtonController).SendPressed();
		}

		void IButton.Released()
		{
			(this as IButtonController).SendReleased();
		}
	}
}
