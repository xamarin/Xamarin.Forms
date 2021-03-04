namespace Microsoft.Maui.Handlers.Benchmarks
{
	class ApplicationStub : Application
	{
		public override IWindow CreateWindow(IActivationState state)
		{
			return new WindowStub();
		}
	}
}