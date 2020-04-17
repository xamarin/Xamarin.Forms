namespace Xamarin.Forms.Maps
{
	public class Camera
	{
		public Camera(Position position, double zoom)
		{
			Position = position;
			Zoom = zoom;
		}

		public Position Position { get; }

		public double Zoom { get; }
	}
}
