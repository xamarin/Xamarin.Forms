using Xamarin.Forms;
using Xamarin.Platform;

namespace Sample
{
	public class ActivityIndicator : Xamarin.Forms.View, IActivityIndicator
	{
		public bool IsRunning { get; set; } = true;

		public Color Color { get; set; }
	}
}