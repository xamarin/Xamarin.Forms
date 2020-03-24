using System.Threading.Tasks;
using NUnit.Framework;
using UIKit;

namespace Xamarin.Forms.Platform.UnitTests.iOS
{
	[TestFixture]
	public class EmbeddingTests
	{
		[Test]
		public async Task CanCreateViewControllerFromContentPage() 
		{
			var contentPage = new ContentPage { Title = "Embedded Page" };
			await Device.InvokeOnMainThreadAsync(() => {
				UIViewController controller = contentPage.CreateViewController();
			});
		}
	}
}