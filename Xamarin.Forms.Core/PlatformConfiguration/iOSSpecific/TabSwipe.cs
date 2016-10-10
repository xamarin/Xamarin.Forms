namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	public class TabSwipe
	{
		public SwipeConfig LeftSwipeConfig { get; set; }
		public SwipeConfig RightSwipeConfig { get; set; }

		public TabSwipe()
		{
			LeftSwipeConfig = new SwipeConfig();
			RightSwipeConfig = new SwipeConfig();
		}

		public TabSwipe(SwipeConfig leftSwipeConfig, SwipeConfig rightSwipeConfig)
		{
			LeftSwipeConfig = leftSwipeConfig;
			RightSwipeConfig = rightSwipeConfig;
		}
	}

	public class SwipeConfig
	{
		public bool IsEnabled { get; set; }

		public double Duration { get; set; }

		public double Velocity { get; set; }

		public SwipeConfig()
		{
			IsEnabled = true;
			Duration = .3;
			Velocity = 1000;
		}

		public SwipeConfig(bool isEnabled, double duration, double velocity)
		{
			IsEnabled = isEnabled;
			Duration = duration;
			Velocity = velocity;
		}
	}
}