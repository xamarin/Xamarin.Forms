using System.Diagnostics;
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
	[Issue(IssueTracker.Github, 14805, "[Bug] Xamarin.Forms.Shell v5.0.0.2196 Backwards Navigation not working in iOS 15.0",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github5000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issue14805 : TestShell
	{
		static int PushCount = 1;
		static int PopCount = 1;

		protected override void Init()
		{
			AddFlyoutItem(CreateContentPage(), "Push Me");
		}

		ContentPage CreateContentPage()
		{
			StackLayout layout = new StackLayout();

			Label titleLabel = new Label()
			{
				Text = $"Page {PushCount - PopCount + 1}"
			};

			Button pushButton = new Button()
			{
				Text = "Push",
				AutomationId = $"Push{PushCount}",
				Command = new Command(async () =>
				{
					PushCount++;
					Debug.WriteLine($"Push {PushCount}");
					await Navigation.PushAsync(CreateContentPage());
				})
			};

			Button popButton = new Button()
			{
				Text = "Pop",
				AutomationId = $"Pop{PopCount}",
				Command = new Command(async () =>
				{
					PopCount++;
					Debug.WriteLine($"Pop {PopCount}");
					await Navigation.PopAsync();
				})
			};

			Label label = new Label()
			{
				Text = "Success",
				AutomationId = "Success"
			};

			layout.Children.Add(titleLabel);
			layout.Children.Add(pushButton);
			layout.Children.Add(popButton);

			if (PopCount == 3)
				layout.Children.Add(label);

			return new ContentPage()
			{
				Content = layout
			};
		}

#if UITEST
		[Test]
		public void PushingPagesAndThenPopNotWorking()
		{
			RunningApp.Tap("Push1");
			RunningApp.Tap("Push2");
			RunningApp.Tap("Pop1");
			RunningApp.Tap("Push3");
			RunningApp.Tap("Pop2");
			RunningApp.Tap("Push4");
			RunningApp.WaitForElement("Success");
		}
#endif
	}
}
