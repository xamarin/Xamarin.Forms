using System;

namespace Xamarin.Forms.Maps
{
	public static class GeographyUtils
	{
		public const double EarthRadiusKm = 6371;

		public static double ToRadians(this double degrees)
		{
			return degrees * Math.PI / 180.0;
		}

		public static double ToDegrees(this double radians)
		{
			return radians / Math.PI * 180.0;
		}
	}
}
