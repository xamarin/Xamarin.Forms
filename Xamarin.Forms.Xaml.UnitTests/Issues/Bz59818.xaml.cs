using System;
using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class Bz59818 : ContentPage
	{
		public Bz59818()
		{
			InitializeComponent();
		}

		public Bz59818(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp]
			public void Setup()
			{
				Device.PlatformServices = new MockPlatformServices();
			}

			[TearDown]
			public void TearDown()
			{
				Device.PlatformServices = null;
			}

			[TestCase(true)]
			[TestCase(false)]
			public void Bz59818(bool useCompiledXaml)
			{
				((MockPlatformServices)Device.PlatformServices).RuntimePlatform = Device.iOS;
				if (useCompiledXaml)
					MockCompiler.Compile(typeof(Bz59818));
				var layout = new Bz59818(useCompiledXaml);
				Assert.That(layout.grid.ColumnDefinitions[0].Width, Is.EqualTo(new GridLength(100)));
			}
		}
	}
}
