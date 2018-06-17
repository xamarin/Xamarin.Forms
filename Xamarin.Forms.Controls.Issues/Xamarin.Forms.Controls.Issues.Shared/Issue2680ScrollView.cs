using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2680, "[Enhancement] Add VerticalScrollMode/HorizontalScrollMode to ListView and ScrollView", PlatformAffected.Default)]
	public class Issue2680ScrollView : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			var longStackLayout = new StackLayout();
			Enumerable.Range(1, 50).Select(i => new Label() {Text = $"Test label {i}"})
				.ForEach(label => longStackLayout.Children.Add(label));

			Content = new ScrollView()
			{
				Orientation = ScrollOrientation.Neither,
				Content = longStackLayout
			};
		}

#if UITEST
		[Test]
		public void Issue2680Test()
		{
			RunningApp.Screenshot("I am at Issue 2680");
			RunningApp.ScrollDown();
			RunningApp.ScrollDown();
			RunningApp.ScrollDown();
			RunningApp.WaitForElement(q => q.Marked("Test label 1"));
			RunningApp.Screenshot("I'm still seeing the first Label in non-scrolable ScrollView");
		}
#endif
	}
}