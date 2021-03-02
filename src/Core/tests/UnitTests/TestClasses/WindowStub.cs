namespace Microsoft.Maui.UnitTests.TestClasses
{
	class WindowStub : IWindow
	{
		public IPage Page { get; set; }
		public IMauiContext MauiContext { get; set; }
		public IPage Content { get; set; }
		public bool IsActive { get; set; }

		public void Activate() { }
		public void Close() { }
		public void Hide() { }
		public void OnCreated() { }
		public void OnPaused() { }
		public void OnResumed() { }
		public void OnStopped() { }
		public void Show() { }
	}
}