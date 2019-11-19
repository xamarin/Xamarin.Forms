using System.IO;
using NUnit.Framework;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class EmbeddedResourceLoaderTests
	{
		[SetUp]
		public void Setup()
		{
			EmbeddedResourceLoader.SetExecutingAssembly(typeof(EmbeddedResourceLoaderTests).Assembly);
		}

		[Test]
		public void EmbeddedResourceLoaderBytesNotFoundTest()
		{
			byte[] test = EmbeddedResourceLoader.GetEmbeddedResourceBytes("embeddedcrimson_not_found.jpg");
			Assert.Null(test);
		}

		[Test]
		public void EmbeddedResourceLoaderBytesTest()
		{
			byte[] test = EmbeddedResourceLoader.GetEmbeddedResourceBytes("embeddedcrimson.jpg");
			Assert.NotNull(test);
		}

		[Test]
		public void EmbeddedResourceLoaderImageSourceNotFoundTest()
		{
			ImageSource test = EmbeddedResourceLoader.GetImageSource("embeddedcrimson_not_found.jpg");
			Assert.IsFalse(test.IsEmpty);
		}

		[Test]
		public void EmbeddedResourceLoaderImageSourceTest()
		{
			ImageSource test = EmbeddedResourceLoader.GetImageSource("embeddedcrimson.jpg");
			Assert.IsFalse(test.IsEmpty);
		}

		[Test]
		public void EmbeddedResourceLoaderPathNotFoundTest()
		{
			var test = EmbeddedResourceLoader.GetEmbeddedResourcePath("embeddedcrimson_not_found.jpg");
			Assert.IsTrue(string.IsNullOrEmpty(test));
		}

		[Test]
		public void EmbeddedResourceLoaderPathTest()
		{
			var test = EmbeddedResourceLoader.GetEmbeddedResourcePath("embeddedcrimson.jpg");
			Assert.IsFalse(string.IsNullOrEmpty(test));
		}

		[Test]
		public void EmbeddedResourceLoaderStreamNotFoundTest()
		{
			Stream test = EmbeddedResourceLoader.GetEmbeddedResourceStream("embeddedcrimson_not_found.jpg");
			Assert.Null(test);
		}

		[Test]
		public void EmbeddedResourceLoaderStreamTest()
		{
			Stream test = EmbeddedResourceLoader.GetEmbeddedResourceStream("embeddedcrimson.jpg");
			Assert.NotNull(test);
		}

		[Test]
		public void EmbeddedResourceLoaderStringNotFoundTest()
		{
			var test = EmbeddedResourceLoader.GetEmbeddedResourceString("embeddedcrimson_not_found.jpg");
			Assert.IsTrue(string.IsNullOrEmpty(test));
		}

		[Test]
		public void EmbeddedResourceLoaderStringTest()
		{
			var test = EmbeddedResourceLoader.GetEmbeddedResourceString("embeddedcrimson.jpg");
			Assert.IsFalse(string.IsNullOrEmpty(test));
		}
	}
}