using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	public class TestExportRendererAttribute : BaseExportRendererAttribute
	{
		public TestExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
		}

		public override int GetMajorVersion()
		{
			return 8;
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

		[Test]
		public void ShouldRegister2()
		{
			// Remember that MajorVersion is set to 8.
			var testExportRendererAttribute = new TestExportRendererAttribute(typeof(Entry), typeof(object))
			{
				MinimumSdkVersion = 6,
				MaximumSdkVersion = 8
			};

			Assert.True(testExportRendererAttribute.ShouldRegister());
		}

		[Test]
		public void ShouldRegister3()
		{
			var testExportRendererAttribute = new TestExportRendererAttribute(typeof(Entry), typeof(object))
			{
				MinimumSdkVersion = 8,
				MaximumSdkVersion = 10
			};

			Assert.True(testExportRendererAttribute.ShouldRegister());
		}

		[Test]
		public void ShouldNotRegister()
		{
			var testExportRendererAttribute = new TestExportRendererAttribute(typeof(Entry), typeof(object))
			{
				MinimumSdkVersion = 4,
				MaximumSdkVersion = 6
			};

			Assert.False(testExportRendererAttribute.ShouldRegister());
		}

		[Test]
		public void ShouldNotRegister2()
		{
			var testExportRendererAttribute = new TestExportRendererAttribute(typeof(Entry), typeof(object))
			{
				MinimumSdkVersion = 10,
				MaximumSdkVersion = 12
			};

			Assert.False(testExportRendererAttribute.ShouldRegister());
		}
	}
}