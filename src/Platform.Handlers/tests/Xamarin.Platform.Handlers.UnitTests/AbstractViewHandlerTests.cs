﻿using Xunit;

namespace Xamarin.Platform.Handlers.UnitTests
{
	[Category(TestCategory.Core, TestCategory.Lifecycle)]
	public class AbstractViewHandlerTests
	{
		[Fact]
		public void ConnectAndDisconnectFireAppropriateNumberOfTimes()
		{
			HandlerStub handlerStub = new HandlerStub();
			handlerStub.SetVirtualView(new Forms.Button());

			Assert.Equal(1, handlerStub.ConnectHandlerCount);
			Assert.Equal(0, handlerStub.DisconnectHandlerCount);

			handlerStub.SetVirtualView(new Forms.Button());
			handlerStub.SetVirtualView(new Forms.Button());
			handlerStub.SetVirtualView(new Forms.Button());
			Assert.Equal(1, handlerStub.ConnectHandlerCount);
			Assert.Equal(0, handlerStub.DisconnectHandlerCount);

			(handlerStub as IViewHandler).DisconnectHandler();
			Assert.Equal(1, handlerStub.ConnectHandlerCount);
			Assert.Equal(1, handlerStub.DisconnectHandlerCount);

			(handlerStub as IViewHandler).DisconnectHandler();
			Assert.Equal(1, handlerStub.ConnectHandlerCount);
			Assert.Equal(1, handlerStub.DisconnectHandlerCount);


			handlerStub.SetVirtualView(new Forms.Button());
			Assert.Equal(2, handlerStub.ConnectHandlerCount);
			Assert.Equal(1, handlerStub.DisconnectHandlerCount);
			(handlerStub as IViewHandler).DisconnectHandler();
			Assert.Equal(2, handlerStub.ConnectHandlerCount);
			Assert.Equal(2, handlerStub.DisconnectHandlerCount);
		}
	}
}
