namespace Xamarin.Platform.Handlers.Tests
{
	class MockViewHandler : AbstractViewHandler<IMockView, NativeMockView>
	{
		public static PropertyMapper<IMockView, MockViewHandler> MockViewMapper = new PropertyMapper<IMockView, MockViewHandler>(ViewHandler.ViewMapper)
		{

		};

		public MockViewHandler() : base(MockViewMapper)
		{

		}

		public MockViewHandler(PropertyMapper mapper) : base(mapper ?? MockViewMapper)
		{

		}

		protected override NativeMockView CreateNativeView() => new NativeMockView();
	}

}
