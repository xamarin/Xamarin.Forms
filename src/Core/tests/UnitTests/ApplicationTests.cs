using System;
using Microsoft.Maui.Tests;
using Xunit;

namespace Microsoft.Maui.UnitTests
{
	[Category(TestCategory.Core, TestCategory.Lifecycle)]
	public class ApplicationTests : IDisposable
	{
		[Fact]
		public void CanCreateApplication()
		{
			var application = new ApplicationStub();

			Assert.NotNull(Application.Current);
			Assert.Equal(Application.Current, application);
		}

		[Fact]
		public void ShouldntCreateMultipleApp()
		{
			var application = new ApplicationStub();

			Assert.Throws<InvalidOperationException>(() => new ApplicationStub());
		}

		public void Dispose()
		{
			(Application.Current as ApplicationStub)?.ClearApp();
		}
	}
}