﻿namespace Xamarin.Essentials
{
	public static partial class Magnetometer
	{
		internal static bool IsSupported =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		internal static void PlatformStart(SensorSpeed sensorSpeed) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		internal static void PlatformStop() =>
			throw ExceptionUtils.NotSupportedOrImplementedException;
	}
}
