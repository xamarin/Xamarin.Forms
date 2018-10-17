using NUnit.Framework;
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
		public void ContentPageWithMissingClass()
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
		public void ContentPageWithMissingType()
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

		[Test]
		public void MissingTypeWithKnownProperty()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(Button);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly""
					xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
					<ContentPage.Content>
						<local:MyCustomButton BackgroundColor=""Red"" />
					</ContentPage.Content>
				</ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<Button>());
			Assert.That(page.Content.BackgroundColor, Is.EqualTo(new Color(1,0,0)));
		}

		[Test]
		public void MissingTypeWithUnknownProperty()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(Button);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly""
					xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
					<ContentPage.Content>
						<local:MyCustomButton MyColor=""Red"" />
					</ContentPage.Content>
				</ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<Button>());
		}

		[Test]
		public void ExplicitStyleAppliedToMissingType()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(Button);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly""
					xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml"">
					<ContentPage.Resources>
						<Style x:Key=""Test"" TargetType=""local:MyCustomButton"">
							<Setter Property=""BackgroundColor"" Value=""Red"" />
						</Style>
					</ContentPage.Resources>
					<local:MyCustomButton Style=""{StaticResource Test}"" />
				</ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<Button>());
			Assert.That(page.Content.BackgroundColor, Is.EqualTo(new Color(1, 0, 0)));
		}

		[Test]
		public void ImplicitStyleAppliedToMissingType()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(Button);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly"">
					<ContentPage.Resources>
						<Style TargetType=""local:MyCustomButton"">
							<Setter Property=""BackgroundColor"" Value=""Red"" />
						</Style>
					</ContentPage.Resources>
					<local:MyCustomButton />
				</ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<Button>());
			Assert.That(page.Content.BackgroundColor, Is.EqualTo(new Color(1, 0, 0)));
		}

		[Test]
		public void StyleTargetingRealTypeNotAppliedToUnknownType()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(Button);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly"">
					<ContentPage.Resources>
						<Style TargetType=""Button"">
							<Setter Property=""BackgroundColor"" Value=""Red"" />
						</Style>
					</ContentPage.Resources>
					<local:MyCustomButton />
				</ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<Button>());
			//Button Style shouldn't apply to MyButton
			Assert.That(page.Content.BackgroundColor, Is.Not.EqualTo(Color.Red));
		}

		[Test]
		public void StyleTargetingUnknownTypeNotAppliedToFallbackType()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(Button);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly"">
					<ContentPage.Resources>
						<Style TargetType=""local:MyCustomButton"">
							<Setter Property=""BackgroundColor"" Value=""Red"" />
						</Style>
					</ContentPage.Resources>
					<Button />
				</ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			//MyButton Style should be applied
			Assert.That(page.Content.BackgroundColor, Is.Not.EqualTo(Color.Red));
		}

		[Test]
		public void UnknownGenericType()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(MockView);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly""
					xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml"">
					<local:MyCustomButton x:TypeArguments=""local:MyCustomType"" />
				 </ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<MockView>());
		}

		[Test]
		public void UnknownMarkupExtensionOnUnknownType()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(MockView);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly"">
					<local:MyCustomButton Bar=""{local:Foo}"" />
				</ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<MockView>());
		}

		[Test]
		public void UnknownMarkupExtensionKnownType()
		{
			XamlLoader.FallbackTypeResolver = (p, type) => type ?? typeof(MockView);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
					xmlns:local=""clr-namespace:MissingNamespace;assembly=MissingAssembly"">
					<Button Text=""{local:Foo}"" />
				</ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<Button>());
		}

		[Test]
		public void StaticResourceKeyNotFound()
		{
			var app = @"
				<Application xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml"">
					<Application.Resources>
						<ResourceDictionary>
							<Style TargetType=""Button"" x:Key=""TestStyle"">
								<Setter Property=""BackgroundColor"" Value=""HotPink"" />
							</Style>
						</ResourceDictionary>
					</Application.Resources>
				</Application>
			";
			Application.Current = (Application)XamlLoader.Create(app, true);

			var xaml = @"
				<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms"">
					<Button Style=""{StaticResource TestStyle}"" />
				</ContentPage>";

			var page = (ContentPage)XamlLoader.Create(xaml, true);
			Assert.That(page.Content, Is.TypeOf<Button>());
		}
	}
}