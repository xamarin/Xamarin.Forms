using System.Diagnostics;

namespace Xamarin.Forms
{
	[DebuggerDisplay ("TopLeft={TopLeft}, TopRight={TopRight}, BottomLeft={BottomLeft}, BottomRight={BottomRight}")]
	[TypeConverter (typeof (CornerRadiusTypeConverter))]
	public struct CornerRadius
	{
		public double TopLeft { get; set; }
		public double TopRight { get; set; }
		public double BottomLeft { get; set; }
		public double BottomRight { get; set; }

		internal bool IsDefault
		{
			get { return TopLeft == 0 && TopRight == 0 && BottomLeft == 0 && BottomRight == 0; }
		}

		public CornerRadius (double uniformRadius) : this (uniformRadius, uniformRadius, uniformRadius, uniformRadius)
		{
		}

		public CornerRadius (double topLeft, double topRight, double bottomLeft, double bottomRight)
		{
			TopLeft = topLeft;
			TopRight = topRight;
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
		}

		public static implicit operator CornerRadius (double uniformRadius)
		{
			return new CornerRadius (uniformRadius);
		}

		bool Equals (CornerRadius other)
		{
			return TopLeft == other.TopLeft && TopRight == other.TopRight && BottomLeft == other.BottomLeft && BottomRight == other.BottomRight;
		}

		public override bool Equals (object obj)
		{
			if (ReferenceEquals (null, obj))
				return false;

			return obj is CornerRadius cornerRadius && Equals (cornerRadius);
		}

		public override int GetHashCode ()
		{
			unchecked {
				int hashCode = TopLeft.GetHashCode();
				hashCode = (hashCode * 397) ^ TopRight.GetHashCode ();
				hashCode = (hashCode * 397) ^ BottomLeft.GetHashCode ();
				hashCode = (hashCode * 397) ^ BottomRight.GetHashCode ();
				return hashCode;
			}
		}

		public static bool operator == (CornerRadius left, CornerRadius right)
		{
			return left.Equals (right);
		}

		public static bool operator != (CornerRadius left, CornerRadius right)
		{
			return !left.Equals (right);
		}

		public void Deconstruct (out double topLeft, out double topRight, out double bottomLeft, out double bottomRight)
		{
			topLeft = TopLeft;
			topRight = TopRight;
			bottomLeft = BottomLeft;
			bottomRight = BottomRight;
		}
	}
}