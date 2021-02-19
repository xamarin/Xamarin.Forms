using Xamarin.Forms;

namespace Xamarin.Platform
{
	public class GradientStop2
	{
		public Color Color { get; set; }

		public float Offset { get; set; }

		public GradientStop2() { }

		public GradientStop2(Color color, float offset)
		{
			Color = color;
			Offset = offset;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is GradientStop2 dest))
				return false;

			return Color == dest.Color && System.Math.Abs(Offset - dest.Offset) < 0.00001;
		}

		public override int GetHashCode()
		{
			return -1234567890 + Color.GetHashCode();
		}
	}
}