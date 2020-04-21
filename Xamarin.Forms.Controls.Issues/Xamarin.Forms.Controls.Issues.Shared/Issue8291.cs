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
	[Issue(IssueTracker.Github, 8291, "[Android] Editor - Text selection menu does not appear when selecting text on an editor placed within a ScrollView",
		PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Editor)]
#endif
	public class Issue8291 : TestContentPage
	{
		protected override void Init()
		{
			Content = new StackLayout()
			{
				Children =
				{
					new Label()
					{
						Text = "Only Relevant on Android"
					},
					new ScrollView()
					{
						Content = new Editor()
						{
							Text = "Press and hold this text. Text should become selected and copy context menu should open",
							AutomationId = "PressEditor"
						}
					},
					new ScrollView()
					{
						Content = new Entry()
						{
							Text = "Press and hold this text. Text should become selected and copy context menu should open",
							AutomationId = "PressEntry"
						}
					}
				}
			};
		}

#if UITEST && __ANDROID__
		[Test]
		public void ContextMenuShowsUpWhenPressAndHoldTextOnEditorAndEntryField()
		{
			RunningApp.TouchAndHold("PressEditor");
			RunningApp.WaitForElement("Share");
			RunningApp.TouchAndHold("PressEntry");
			RunningApp.WaitForElement("Share");
		}
#endif
	}
}
