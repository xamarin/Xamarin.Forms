namespace Xamarin.Forms
{
	public interface ISliderController : IViewController
	{
		void SendDragStarted();
		void SendDragCompleted();
	}
}