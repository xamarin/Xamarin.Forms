using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	public class TestExportRendererAttribute : BaseExportRendererAttribute
	{
		public TestExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
			MajorVersion = 8;
		}
	}

	[TestFixture]
	public class ExportRendererAttributeTests : BaseTestFixture
	{
		[Test]
		public void InvalidMinimumSdkVersion()
		{
			var testExportRendererAttribute = new TestExportRendererAttribute(typeof(Entry), typeof(object));

			Assert.Throws<ArgumentException>(() =>
			{
				testExportRendererAttribute.MaximumSdkVersion = 10;
				testExportRendererAttribute.MinimumSdkVersion = 20;
			});
		}

		[Test]
		public void InvalidMaximumSdkVersion()
		{
			var testExportRendererAttribute = new TestExportRendererAttribute(typeof(Entry), typeof(object));

			Assert.Throws<ArgumentException>(() =>
			{
				testExportRendererAttribute.MinimumSdkVersion = 20;
				testExportRendererAttribute.MaximumSdkVersion = 10;
			});
		}

		[Test]
		public void ValidEquality()
		{
			var testExportRendererAttribute = new TestExportRendererAttribute(typeof(Entry), typeof(object));

			Assert.DoesNotThrow(() =>
			{
				testExportRendererAttribute.MinimumSdkVersion = 10;
				testExportRendererAttribute.MaximumSdkVersion = 10;
			});
		}

		[Test]
		public void ShouldRegister()
		{
			var testExportRendererAttribute = new TestExportRendererAttribute(typeof(Entry), typeof(object))
			{
				MinimumSdkVersion = 6,
				MaximumSdkVersion = 10
			};

			Assert.True(testExportRendererAttribute.ShouldRegister());
		}
	}
}