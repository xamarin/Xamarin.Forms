using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Xamarin.Forms.Controls.GalleryPages.PlatformTestsGallery
{
	public class TestNameContainsFilter : TestFilter
	{
		string _substring;

		public TestNameContainsFilter(string substring) => _substring = substring;

		public override TNode AddToXml(TNode parentNode, bool recursive)
		{
			TNode result = parentNode.AddElement("contains", _substring);
			return result;
		}

		public override bool Match(ITest test)
		{
			return test.Name.Contains(_substring);
		}
	}
}