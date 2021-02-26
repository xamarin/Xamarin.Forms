namespace Microsoft.Maui
{
	public interface IWindow
	{
		public IMauiContext MauiContext { get; set; }
		public IPage Page { get; set; }

		void Create();
		void Resume();
		void Pause();
		void Stop();
	}
}