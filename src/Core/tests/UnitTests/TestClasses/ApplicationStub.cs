using Microsoft.Maui.UnitTests.TestClasses;

namespace Microsoft.Maui.Tests
{
	class ApplicationStub : Application
	{
		public override IWindow CreateWindow(IActivationState state)
		{
			return new WindowStub();
		}

		internal void ClearApp()
		{
			Current = null;
		}
	}
}