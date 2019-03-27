using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests.Gh5704NS
{
	public class MyLabel : Label
	{
	}
}

namespace Xamarin.Forms.Xaml.UnitTests
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class Gh5704 : ContentPage
	{
		public Gh5704() => InitializeComponent();
		public Gh5704(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture] class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();
			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[Test]
			public void Throws([Values(false, true)]bool useCompiledXaml)
			{
				if (useCompiledXaml)
					Assert.Throws<XamlParseException>(() => MockCompiler.Compile(typeof(Gh5704)));
				else
					Assert.Throws<XamlParseException>(() => new Gh5704(useCompiledXaml));
			}
		}
	}
}