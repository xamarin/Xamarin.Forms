using Xamarin.Forms;

namespace Xamarin.Platform
{
	public class SolidColorBrush2 : Brush2
	{
		public SolidColorBrush2()
		{

		}

		public SolidColorBrush2(Color color)
		{
			Color = color;
		}

		public override bool IsEmpty
		{
			get
			{
				var solidColorBrush = this;
				return solidColorBrush == null || solidColorBrush.Color.IsDefault;
			}
		}

		public Color Color { get; set; } = Color.Default;

		public override bool Equals(object obj)
		{
			if (!(obj is SolidColorBrush2 dest))
				return false;

			return Color == dest.Color;
		}

		public override int GetHashCode()
		{
			return -1234567890 + Color.GetHashCode();
		}
	}
}