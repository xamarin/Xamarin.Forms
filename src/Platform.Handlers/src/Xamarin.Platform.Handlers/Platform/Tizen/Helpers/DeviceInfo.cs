using System;
using ElmSharp;
using TSystemInfo = Tizen.System.Information;

namespace Xamarin.Platform.Tizen
{
	public static class DeviceInfo
	{
		static Lazy<string> s_profile = new Lazy<string>(() =>
		{
			//TODO : Fix me if elm_config_profile_get() unavailable
			return Elementary.GetProfile();
		});

		static Lazy<int> s_dpi = new Lazy<int>(() =>
		{
			int dpi = 0;
			if (IsTV)
			{
				// Use fixed DPI value (72) if TV profile
				return 72;
			}
			TSystemInfo.TryGetValue<int>("http://tizen.org/feature/screen.dpi", out dpi);
			return dpi;
		});

		static Lazy<int> s_screenWidth = new Lazy<int>(() =>
		{
			int width = 0;
			TSystemInfo.TryGetValue("http://tizen.org/feature/screen.width", out width);
			return width;
		});

		static Lazy<int> s_screenHeight = new Lazy<int>(() =>
		{
			int height = 0;
			TSystemInfo.TryGetValue("http://tizen.org/feature/screen.width", out height);
			return height;
		});

		static Lazy<double> s_elmScale = new Lazy<double>(() =>
		{
			return s_deviceScale.Value / Elementary.GetScale();
		});

		static Lazy<DeviceType> s_deviceType = new Lazy<DeviceType>(() =>
		{
			var type = DeviceType.Unknown;
			if (TSystemInfo.TryGetValue("http://tizen.org/system/device_type", out string deviceType))
			{
				if (deviceType.StartsWith("Mobile"))
				{
					type = DeviceType.Mobile;
				}
				else if (deviceType.StartsWith("TV"))
				{
					type = DeviceType.TV;
				}
				else if (deviceType.StartsWith("Wearable"))
				{
					type = DeviceType.Watch;
				}
				else if (deviceType.StartsWith("Refrigerator"))
				{
					type = DeviceType.Refrigerator;
				}
				else if (deviceType.StartsWith("TizenIOT"))
				{
					type = DeviceType.IoT;
				}
				return type;
			}
			else
			{
				// Since, above key("http://tizen.org/system/device_type") is not available on Tizen 4.0, we uses profile to decide the type of device on 4.0.
				var profile = GetProfile();
				if (profile == "mobile")
				{
					type = DeviceType.Mobile;
				}
				else if (profile == "tv")
				{
					type = DeviceType.TV;
				}
				else if (profile == "wearable")
				{
					type = DeviceType.Watch;
				}
				else
				{
					type = DeviceType.Unknown;
				}
				return type;
			}
		});

		static Lazy<double> s_deviceScale = new Lazy<double>(() =>
		{
			// This is the base scale value and varies from profile
			return GetBaseScale(s_deviceType.Value);
		});

		public static int DPI => s_dpi.Value;

		public static double ElmScale => s_elmScale.Value;

		public static double PhysicalScale => DPI / 160.0;

		public static Size PixelScreenSize => new Size(s_screenWidth.Value, s_screenHeight.Value);

		public static Size DPScreenSize => new Size((int)(s_screenWidth.Value / PhysicalScale), (int)(s_screenHeight.Value / PhysicalScale));

		public static Size ScaledDPScreenSize => new Size((int)(s_screenWidth.Value / ScalingFactor), (int)(s_screenHeight.Value / ScalingFactor));

		public static double ScalingFactor
		{
			get
			{
				var scalingFactor = 1.0;  // scaling is disabled, we're using pixels as Xamarin's geometry units
				if (DisplayResolutionUnit == DisplayResolutionUnit.VP && ViewPortWidth > 0)
				{
					scalingFactor = s_screenWidth.Value / ViewPortWidth;
				}
				else
				{
					if (DisplayResolutionUnit == DisplayResolutionUnit.DP || DisplayResolutionUnit == DisplayResolutionUnit.DeviceScaledDP)
					{
						scalingFactor = DPI / 160.0;
					}

					if (DisplayResolutionUnit == DisplayResolutionUnit.DeviceScaledPixel || DisplayResolutionUnit == DisplayResolutionUnit.DeviceScaledDP)
					{
						var portraitSize = Math.Min(DPScreenSize.Width, DPScreenSize.Height);
						if (portraitSize > 2000)
						{
							scalingFactor *= 4;
						}
						else if (portraitSize > 1000)
						{
							scalingFactor *= 2.5;
						}
					}
				}
				return scalingFactor;
			}
		}

		public static double GetPhysicalPortraitSizeInDP()
		{
			var screenSize = DPScreenSize;
			return Math.Min(screenSize.Width, screenSize.Height);
		}

		public static double CalculateDoubleScaledSizeInLargeScreen(double size)
		{
			if (DisplayResolutionUnit == DisplayResolutionUnit.VP)
				return size;

			if ((DisplayResolutionUnit == DisplayResolutionUnit.Pixel || DisplayResolutionUnit == DisplayResolutionUnit.DP) && GetPhysicalPortraitSizeInDP() > 1000)
			{
				size *= 2.5;
			}

			if (DisplayResolutionUnit == DisplayResolutionUnit.Pixel || DisplayResolutionUnit == DisplayResolutionUnit.DeviceScaledPixel)
			{
				size = DPExtensions.ConvertToPixel(size);
			}
			return size;
		}

		public static double GetBaseScale(DeviceType deviceType)
		{
			if (deviceType == DeviceType.Mobile)
			{
				return ThemeConstants.Common.Resource.Mobile.BaseScale;
			}
			else if (deviceType == DeviceType.TV)
			{
				return ThemeConstants.Common.Resource.TV.BaseScale;
			}
			else if (deviceType == DeviceType.Watch)
			{
				return ThemeConstants.Common.Resource.Watch.BaseScale;
			}
			else if (deviceType == DeviceType.Refrigerator)
			{
				return ThemeConstants.Common.Resource.Refrigerator.BaseScale;
			}
			else if (deviceType == DeviceType.IoT)
			{
				return ThemeConstants.Common.Resource.Iot.BaseScale;
			}
			return 1.0;
		}

		public static string GetProfile()
		{
			return s_profile.Value;
		}

		public static DeviceType GetDeviceType()
		{
			return s_deviceType.Value;
		}

		public static bool IsMobile => GetDeviceType() == DeviceType.Mobile;

		public static bool IsTV => GetDeviceType() == DeviceType.TV;

		public static bool IsWatch => GetDeviceType() == DeviceType.Watch;

		public static bool IsRefrigerator => GetDeviceType() == DeviceType.Refrigerator;

		public static bool IsIoT => GetDeviceType() == DeviceType.IoT;

		public static DisplayResolutionUnit DisplayResolutionUnit { get; internal set; }

		public static double ViewPortWidth { get; internal set; }

		//public static bool UseDP { get; set; } = true;

		//public static bool UseDeviceAutoScaling { get; set; } = false;

		//public static bool UseVP { get; set; } = false;

		//public static double ViewPortWidth { get; set; } = -1d;
	}

	public enum DeviceType
	{
		Mobile,
		TV,
		Watch,
		Refrigerator,
		IoT,
		Unknown
	}

	public enum DisplayResolutionUnit
	{
		Pixel,
		DeviceScaledPixel,
		DP,
		DeviceScaledDP,
		VP
	}
}