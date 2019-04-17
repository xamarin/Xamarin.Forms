using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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
	[Issue(IssueTracker.Github, 5650, "[Shell] TitleView Bindings does not work with Android",
		PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issue5650 : TestShell
	{
		const string PassStep1 = "Step 1 Success";
		const string Success = "If you can see this, the test has passed";
		protected override void Init()
		{
			var page = CreateContentPage();

			Button button = null;
			button = new Button()
			{
				Text = $"Verify Title View Says: Step 1 Passed then Click Me",
				Command = new Command(() =>
				{
					button.Text = "Verify Title View Changed to Success Text";
					page.BindingContext = new ViewModel() { Text = Success };
				}),
				AutomationId = "NextStep"
			};

			page.BindingContext = new ViewModel() { Text = PassStep1 };
			page.Content = new StackLayout()
			{
				Children =
				{
					button
				}
			};

			// setup title view
			StackLayout layout = new StackLayout() { BackgroundColor = Color.White };
			Label label = new Label();
			label.Text = "Test Text";
			label.SetBinding(Label.TextProperty, "Text");
			layout.Children.Add(label);
			Shell.SetTitleView(page, layout);

		}

		[Preserve(AllMembers = true)]
		public class ViewModel
		{
			public string Text { get; set; }
		}

#if UITEST && !__WINDOWS__
		[Test]
		public void ShellViewTitleViewBinding()
		{
			RunningApp.WaitForElement(PassStep1);
			RunningApp.Tap("NextStep");
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}
