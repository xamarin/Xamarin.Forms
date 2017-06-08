using System;

namespace Xamarin.Forms.Platform.Tizen.Native
{
	/// <summary>
	/// Enumeration for the orientation of a rectangular screen.
	/// </summary>
	[Flags]
	public enum DisplayOrientations
	{
		/// <summary>
		/// No display orientation is specified.
		/// </summary>
		None = 0,

		/// <summary>
		/// The display is oriented in a natural position.
		/// </summary>
		Portrait = 1,

		/// <summary>
		/// The display's left side is at the top.
		/// </summary>
		Landscape = 2,

		/// <summary>
		/// The display is upside down.
		/// </summary>
		PortraitFlipped = 4,

		/// <summary>
		/// The display's right side is at the top.
		/// </summary>
		LandscapeFlipped = 8
	}
}
