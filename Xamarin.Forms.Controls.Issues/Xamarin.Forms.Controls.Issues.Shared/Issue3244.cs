
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3244, "[Android] ImageSource.FromFile does not pick up images if they are stored in a mipmap folder",
		PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Image)]
#endif
	public class Issue3244 : TestContentPage
	{
		string _imageId = "imageid";
		protected override void Init()
		{
			Content = new StackLayout()
			{
				Children =
				{
					new Label()
					{
						Text = "Android only test"
					},
					new Image()
					{
						Source = ImageSource.FromFile("mipmapicon"),
						AutomationId = _imageId
					}
				}
			};
		}

#if UITEST && __ANDROID__
		[Test]
		public void MipMapImageLoaded()
		{
			var element = RunningApp.WaitForElement(_imageId);
			Assert.IsTrue(element[0].Rect.Height > 0);
			Assert.IsTrue(element[0].Rect.Width > 0);
		}
#endif
	}
}