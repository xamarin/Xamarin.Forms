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
	[Category(UITestCategories.Github5000)]
	[Category(UITestCategories.Editor)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 15368, "[iOS] Editor's TextChanged event is fired on Unfocus even when no text changed", PlatformAffected.iOS)]
	public class Issue15368 : TestContentPage
	{
		Label _label = new Label { Text = "Focus the Editor, then click the Label to unfocus the Editor. If this text changes, this test has failed." };

		protected override void Init()
		{
			var editor = new Editor { AutomationId = "editor" };
			editor.TextChanged += Editor_TextChanged;

			var layout = new StackLayout { Children = { _label, editor, new Label { Text = "Click me", AutomationId = "click" } } };

			Content = layout;
		}

		void Editor_TextChanged(object sender, TextChangedEventArgs e)
		{
			_label.Text = "FAIL";
		}

#if UITEST
		[Test]
		public void Issue15368Test()
		{
			RunningApp.WaitForElement("editor");
			RunningApp.Tap("editor");
			RunningApp.WaitForElement("click");
			RunningApp.Tap("click");
			RunningApp.WaitForNoElement("FAIL");
		}
#endif
	}
}