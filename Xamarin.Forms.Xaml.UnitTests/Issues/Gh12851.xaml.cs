using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class Gh12851 : ContentPage
	{
		public Gh12851() => InitializeComponent();
		public Gh12851(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();
			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[Test]
			public void FontImageWithDynamicResource([Values(false, true)] bool useCompiledXaml)
			{
				var layout = new Gh12851(useCompiledXaml);
				Assert.That((layout.staticImage.Source as FontImageSource).Color, Is.EqualTo(Color.HotPink));
				Assert.That((layout.dynamicWithExtension.Source as FontImageSource).Color, Is.EqualTo(Color.HotPink));
				Assert.That((layout.dynamicWithElement.Source as FontImageSource).Color, Is.EqualTo(Color.HotPink));
			}

			[Test]
			public void FontImageWithDynamicResourceChanged([Values(false, true)] bool useCompiledXaml)
			{
				var layout = new Gh12851(useCompiledXaml);
				var rd = layout.Resources;
				rd["PrimaryColor"] = Color.Chartreuse;
				Assert.That((layout.staticImage.Source as FontImageSource).Color, Is.EqualTo(Color.HotPink));
				Assert.That((layout.dynamicWithExtension.Source as FontImageSource).Color, Is.EqualTo(Color.Chartreuse));
				Assert.That((layout.dynamicWithElement.Source as FontImageSource).Color, Is.EqualTo(Color.Chartreuse));
			}
		}
	}
}
