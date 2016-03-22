using Android.Content;

namespace Xamarin.Forms.Platform.Android
{
	public class AndroidAppIndexProvider : IAppIndexingProvider
	{
		public AndroidAppIndexProvider(Context context)
		{
			AppLinks = new AndroidAppLinks(context);
		}

		public IAppLinks AppLinks { get; }
	}
}