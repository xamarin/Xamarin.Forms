using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class Gh5095 : ContentPage
	{
		public Gh5095() => InitializeComponent();
		public Gh5095(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();
			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[Test]
			public void ThrowsOnInvalidXaml([Values(false, true)]bool useCompiledXaml)
			{
				if (useCompiledXaml)
					Assert.Catch<XamlParseException>(() => MockCompiler.Compile(typeof(Gh5095)));
				else
					Assert.Catch<XamlParseException>(() => new Gh5095(false));
			}
		}
	}
}
