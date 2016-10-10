namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	public class TabSlide
	{
		public SlideConfig LeftSlideConfig { get; set; }
		public SlideConfig RightSlideConfig { get; set; }

		public TabSlide()
		{
			LeftSlideConfig = new SlideConfig();
			RightSlideConfig = new SlideConfig();
		}

		public TabSlide(SlideConfig leftSlideConfig, SlideConfig rightSlideConfig)
		{
			LeftSlideConfig = leftSlideConfig;
			RightSlideConfig = rightSlideConfig;
		}
	}

	public class SlideConfig
	{
		public bool IsEnabled { get; set; }

		public double Duration { get; set; }

		public double Velocity { get; set; }

		public SlideConfig()
		{
			IsEnabled = true;
			Duration = .3;
			Velocity = 1000;
		}

		public SlideConfig(bool isEnabled, double duration, double velocity)
		{
			IsEnabled = isEnabled;
			Duration = duration;
			Velocity = velocity;
		}
	}
}