namespace Xamarin.Forms.Platform.UWP
{
	public interface IRecreateImageSource
	{
		Windows.UI.Xaml.Media.ImageSource InitialSource { get; }
		Windows.UI.Xaml.Media.ImageSource CreateImageSource();
	}
}
