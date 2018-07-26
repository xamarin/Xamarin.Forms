using System;
using System.Linq.Expressions;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2875, "[macOS] ScrollViewRenderer.PackContent is not called when ScrollView.Content is set after same delay", PlatformAffected.macOS)]
	public class Issue2875 : TestContentPage
	{
		const string ButtonId = "ChangeButton";
		const string Label1Id = "Label1";
		const string Label2Id = "Label2";

		protected override void Init()
		{
			var sl = new StackLayout();
			var sv = new ScrollView
			{
				Content = new Label
				{
					Text = "Label 1",
					AutomationId = Label1Id
				}
			};
			sl.Children.Add(sv);
			sl.Children.Add(new Button
			{
				Text = "Change",
				AutomationId = ButtonId,
				Command = new Command(() =>
					sv.Content = new Label
					{
						Text = "Label 2",
						AutomationId = Label2Id
					}
				)
			});

			Content = sl;
		}

#if UITEST
#if __MACOS__
		[Test]
		public void Issue2875Test()
		{
			RunningApp.WaitForElement(q => q.Marked(Label1Id));
			RunningApp.WaitForElement(q => q.Marked(ButtonId));
			RunningApp.Tap(q => q.Marked(ButtonId));

			RunningApp.WaitForElement(q => q.Marked(Label2Id));
		}
#endif
#endif
	}
}
