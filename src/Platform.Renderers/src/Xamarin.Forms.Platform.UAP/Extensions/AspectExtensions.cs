﻿using WStretch = Windows.UI.Xaml.Media.Stretch;

namespace Xamarin.Forms.Platform.UWP
{
	internal static class AspectExtensions
	{
		public static WStretch ToStretch(this Aspect aspect)
		{
			switch (aspect)
			{
				case Aspect.Fill:
					return WStretch.Fill;

				case Aspect.AspectFill:
					return WStretch.UniformToFill;

				case Aspect.AspectFit:
				default:
					return WStretch.Uniform;
			}
		}
	}
}