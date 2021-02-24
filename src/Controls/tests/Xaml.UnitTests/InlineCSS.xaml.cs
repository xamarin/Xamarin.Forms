using System;
using NUnit.Framework;
using Microsoft.Maui.Controls.Core.UnitTests;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	public partial class InlineCSS : ContentPage
	{
		public InlineCSS()
		{
			InitializeComponent();
		}

		public InlineCSS(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			[SetUp]
			public void SetUp()
			{
				Device.PlatformServices = new MockPlatformServices();
				Microsoft.Maui.Controls.Internals.Registrar.RegisterAll(new Type[0]);
			}

			[TearDown]
			public void TearDown()
			{
				Device.PlatformServices = null;
			}

			[TestCase(false), TestCase(true)]
			public void InlineCSSParsed(bool useCompiledXaml)
			{
				var layout = new InlineCSS(useCompiledXaml);
				Assert.That(layout.label.TextColor, Is.EqualTo(Color.Pink));
			}

			[TestCase(false), TestCase(true)]
			public void InitialValue(bool useCompiledXaml)
			{
				var layout = new InlineCSS(useCompiledXaml);
				Assert.That(layout.BackgroundColor, Is.EqualTo(Color.Green));
				Assert.That(layout.stack.BackgroundColor, Is.EqualTo(Color.Green));
				Assert.That(layout.button.BackgroundColor, Is.EqualTo(Color.Green));
				Assert.That(layout.label.BackgroundColor, Is.EqualTo(VisualElement.BackgroundColorProperty.DefaultValue));
				Assert.That(layout.label.TextTransform, Is.EqualTo(TextTransform.Uppercase));
			}
		}
	}
}
