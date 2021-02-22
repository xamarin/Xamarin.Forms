namespace Xamarin.Platform
{
	public interface IWindow
	{
		public IMauiContext HandlersContext { get; set; }
		public IPage Page { get; set; }
	}
}
