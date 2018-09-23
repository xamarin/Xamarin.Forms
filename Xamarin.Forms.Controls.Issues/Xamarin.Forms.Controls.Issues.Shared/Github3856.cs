using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3856, "[Android] MaxLines on Label not working with FastRenderers 3.3.0-pre1", PlatformAffected.Android)]
	public class Github3856 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			Content = new Label
			{
				MaxLines = 4,
				LineBreakMode = LineBreakMode.TailTruncation,
				Text = "You should see 4 lines of text and truncation at the end. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam mattis quam non enim pellentesque, ut placerat purus finibus. Nulla quis tincidunt ante. Ut mauris lectus, aliquam a sagittis vitae, consequat eget elit. Interdum et malesuada fames ac ante ipsum primis in faucibus. Pellentesque convallis nunc nisi, a imperdiet elit efficitur et. Duis in lectus mollis, interdum ipsum et, tincidunt orci. Fusce ipsum metus, imperdiet non lacus vitae, facilisis feugiat magna. Nulla volutpat nisl tortor, a consectetur felis consectetur non. Curabitur in enim vulputate sem volutpat bibendum id nec lorem. Mauris laoreet lacus ac volutpat tempus."
			};
		}
	}
}