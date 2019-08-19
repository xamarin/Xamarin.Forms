namespace Xamarin.Forms.Maps
{
	public class Camera
	{
		public Camera(Position position, double zoom, double bearing = 0, double tilt = 0)
		{
			Position = position;
			Bearing = bearing;
			Tilt = tilt;
			Zoom = zoom;
		}

		public Position Position { get; }

		public double Zoom { get; }

		public double Bearing { get; }

		public double Tilt { get; }
	}
}
