namespace Xamarin.Forms
{
	public class OnPlatform<T>
	{
		public T Android { get; set; }

		public T iOS { get; set; }

		public T WinPhone { get; set; }
		
		public T Windows { get; set; }

#warning This method no longer returns TargetPlatform.WinPhone for Windows 8.1 or UWP applications. Please use Windows type for Windows 8.1 and UWP applications.
		public static implicit operator T(OnPlatform<T> onPlatform)
		{
			switch (Device.OS)
			{
				case TargetPlatform.iOS:
					return onPlatform.iOS ?? onPlatform.Default;
					break;
				case TargetPlatform.Android:
					return onPlatform.Android ?? onPlatform.Default;
					break;
				case TargetPlatform.WinPhone:
					return onPlatform.WinPhone ?? onPlatform.Default;
					break;
				case TargetPlatform.Windows:
					return ( onPlatform.Windows ?? onPlatform.Default ) ?? onPlatform.WinPhone;
					break;
				default:
					onPlatform.Default;
					break;
			}

			return onPlatform.iOS;
		}
	}
}
