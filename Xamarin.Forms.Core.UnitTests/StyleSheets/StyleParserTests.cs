using System.IO;
using NUnit.Framework;

namespace Xamarin.Forms.StyleSheets.UnitTests
{
	[TestFixture]
	public class StyleParserTests
	{
		[TestCase("font-size: 10", 1)]
		[TestCase("font-size: 10;", 1)]
		[TestCase("font-size: 10 background-color: blue", 0)]
		[TestCase("font-size: 10; background-color: blue", 2)]
		[TestCase("font-size: 10; background-color: blue;", 2)]
		public void TestCase(string css, int propertyCount)
		{
			using (var reader = new StringReader(css))
			{
				var parser = Style.Parse(new CssReader(reader));

				Assert.AreEqual(propertyCount, parser.Declarations.Count);
			}
		}
	}
}
