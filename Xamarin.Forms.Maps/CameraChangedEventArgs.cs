namespace Xamarin.Forms.Maps
{
	public class CameraChangedEventArgs
	{
		public Camera Camera { get; }

		public CameraChangedEventArgs(Camera camera)
		{
			Camera = camera;
		}
	}
}
