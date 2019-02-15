using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public sealed class Gh5270VM : IGh5270VM
	{
		public bool CanEdit { get; set; }

		public string Text { get; set; }
	}

	public interface IGh5270VM
	{
		string Text { get; set; }
	}

	public partial class Gh5270 : ContentPage
	{
		public Gh5270() => InitializeComponent();
		public Gh5270(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();
			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[Test]
			public void CompiledBindingToInterface([Values(false, true)]bool useCompiledXaml)
			{
				var layout = new Gh5270(useCompiledXaml) { BindingContext = new Gh5270VM { CanEdit = true, Text = "foo" } };
				Assert.That(layout.entry.IsEnabled, Is.EqualTo(true));
				Assert.That(layout.label.Text, Is.EqualTo("foo"));
			}
		}
	}
}