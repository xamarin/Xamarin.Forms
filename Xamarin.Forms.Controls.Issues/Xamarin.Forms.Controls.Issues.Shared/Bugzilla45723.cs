using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 45723, "Entry / Editor and a Button. Tapping the button dismisses the keyboard", PlatformAffected.iOS)]
	public class Bugzilla45723 : TestNavigationPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			PushAsync(new EditorAndButtonReproPage());
		}
	}

	public class EditorAndButtonReproPage : ContentPage
	{
		public EditorAndButtonReproPage()
		{
			BackgroundColor = Color.Gray;
			Padding = 50;
			var editor = new Editor { HorizontalOptions = LayoutOptions.FillAndExpand };
			var editorButton = new Button { Text = "OK", HorizontalOptions = LayoutOptions.End };
			var editorLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Children = { editor, editorButton }, VerticalOptions = LayoutOptions.Start };
			var endtry = new Entry { Placeholder = "Entry", HorizontalOptions = LayoutOptions.FillAndExpand };
			var entryButton = new Button { Text = "OK", HorizontalOptions = LayoutOptions.End };
			var entryLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Children = { endtry, entryButton }, VerticalOptions = LayoutOptions.Start };
			Content = new StackLayout { Children = { editorLayout, entryLayout } };
		}
	}
}