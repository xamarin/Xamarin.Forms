using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public class Gh4524VM {
		public Uri[] Images { get; } = { new Uri("file://foo.jpg", UriKind.RelativeOrAbsolute)};
	}

	public partial class Gh4524 : ContentPage
	{
		public Gh4524() => InitializeComponent();

		public Gh4524(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();
			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[TestCase(true), TestCase(false)]
			public void BindingCompilerIndexers(bool useCompiledXaml)
			{
				if (useCompiledXaml)
					MockCompiler.Compile(typeof(Gh4524));
				var layout = new Gh4524(useCompiledXaml) { BindingContext = new Gh4524VM() };
				Assert.That((layout.image.Source as UriImageSource).Uri.ToString(), Is.EqualTo("file://foo.jpg/"));
			}
		}
	}
}
