namespace Xamarin.Platform
{
	public interface IWindow
	{
		public IHandlersContext HandlersContext { get; set; }
		public IPage Page { get; set; }
	}
}
