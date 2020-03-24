using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.Platform.UnitTests.Android
{
	[TestFixture]
	public class EmbeddingTests : PlatformTestFixture
	{
		[Test]
		public async Task CanCreateFragmentFromContentPage()
		{
			var contentPage = new ContentPage { Title = "Embedded Page" };
			contentPage.Parent = Application.Current;
			await Device.InvokeOnMainThreadAsync(() => {
				var fragment = contentPage.CreateSupportFragment(Context);
			});
			contentPage.Parent = null;
		}
	}
}