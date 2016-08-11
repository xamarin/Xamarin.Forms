namespace Xamarin.Forms
{
	public class OnPlatform<T>
	{
		public T Android { get; set; }

		public T iOS { get; set; }

		public T WinPhone { get; set; }
		
		public T Windows { get; set; }

		public static implicit operator T(OnPlatform<T> onPlatform)
		{
			switch (Device.OS)
			{
				case TargetPlatform.iOS:
					return onPlatform.iOS;
				case TargetPlatform.Android:
					return onPlatform.Android;
				case TargetPlatform.WinPhone:
					return onPlatform.WinPhone;
				case TargetPlatform.Windows:
					return onPlatform.Windows;
			}

			return onPlatform.iOS;
		}
	}
}
