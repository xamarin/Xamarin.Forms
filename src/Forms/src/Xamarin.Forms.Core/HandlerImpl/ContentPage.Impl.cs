
using Xamarin.Platform;

namespace Xamarin.Forms
{
	public partial class ContentPage : IPage
	{
		public IView View { get => Content; set => Content = value; }
	}
}
