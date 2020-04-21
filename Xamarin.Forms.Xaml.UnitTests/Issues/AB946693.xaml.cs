using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class AB946693 : ContentPage
	{
		public AB946693() => InitializeComponent();
		public AB946693(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();
			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[Test]
			public void KeylessResourceThrowsMeaningfulException([Values(false, true)]bool useCompiledXaml)
			{
				if (useCompiledXaml)
					Assert.Catch<XamlParseException>(() => MockCompiler.Compile(typeof(AB946693)));
				else
					Assert.Catch<XamlParseException>(() => new AB946693(useCompiledXaml));
			}
		}
	}
}
