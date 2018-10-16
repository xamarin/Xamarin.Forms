using System;

using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[TestFixture]
	public class DesignTimeLoaderTests
	{
		[SetUp]
		public void Setup()
		{
			Device.PlatformServices = new MockPlatformServices();
		}

		[TearDown]
		public void TearDown()
		{
			XamlLoader.FallbackTypeResolver = null;
			Device.PlatformServices = null;
		}

		[Test]
		public void ContenPageWithMissingClass()
		{
			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
					x:Class=""Xamarin.Forms.Xaml.UnitTests.CustomView""
				/>";

			Assert.That(XamlLoader.Create(xaml, true), Is.TypeOf<ContentPage>());
		}

		[Test]
		public void ViewWithMissingClass()
		{
			var xaml = @"
				<ContentView xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
					x:Class=""Xamarin.Forms.Xaml.UnitTests.CustomView""
				/>";

			Assert.That(XamlLoader.Create(xaml, true), Is.TypeOf<ContentView>());
		}

		[Test]
		public void ContenPageWithMissingType()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(MockView);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly""
					xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
					<ContentPage.Content>
						<local:MyCustomButton />
					</ContentPage.Content>
				</ContentPage>";

			var page = (ContentPage) XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<MockView>());
		}
	}
}