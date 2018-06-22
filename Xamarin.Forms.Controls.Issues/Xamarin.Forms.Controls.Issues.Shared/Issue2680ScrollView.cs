using System;
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
	[Issue(IssueTracker.Github, 2680, "[Enhancement] Add VerticalScrollMode/HorizontalScrollMode to ListView and ScrollView", PlatformAffected.All)]
	public class Issue2680ScrollView : TestContentPage // or TestMasterDetailPage, etc ... 
	{
		public const string BtnDisable = "Disable scroll";
		public const string BtnEnable = "Enable scroll";

		public bool IsEnabled { get; set; } = false;

		public void ToggleButtonText()
		{
			IsEnabled = !IsEnabled;
			toggleButton.Text = ButtonText;
		}

		public string ButtonText => IsEnabled ? BtnDisable : BtnEnable;

		protected override void Init()
		{
			toggleButton = new Button
			{
				Text = ButtonText
			};

			// Initialize ui here instead of ctor 
			var longStackLayout = new StackLayout();

			longStackLayout.Children.Add(toggleButton);
			Enumerable.Range(1, 50).Select(i => new Label() { Text = $"Test label {i}" })
				.ForEach(label => longStackLayout.Children.Add(label));

			scrollView = new ScrollView
			{
				Orientation = ScrollOrientation.Neither,
				Content = longStackLayout
			};

			Content = scrollView;

			toggleButton.Clicked += ToggleButtonOnClicked;
		}

		private void ToggleButtonOnClicked(object sender, EventArgs e)
		{
			ToggleButtonText();
			scrollView.Orientation = IsEnabled ? ScrollOrientation.Vertical : ScrollOrientation.Neither;
		}

		private ScrollView scrollView;
		private Button toggleButton;

#if UITEST
		[Test] 
		public void Issue2680Test_ScrollDisabled() 
		{ 
			RunningApp.Screenshot("I am at Issue 2680");
			for (var i = 0; i < 10; i++)
			{
				RunningApp.ScrollDown();
			}
			RunningApp.WaitForElement(q => q.Marked("Test label 1")); 
			RunningApp.Screenshot("I'm still seeing the first Label in non-scrollable ScrollView");
		}

		[Test]
		public void Issue2680Test_ScrollEnabled()
		{
			RunningApp.Screenshot("I am at Issue 2680");
			RunningApp.Tap(q => q.Button(BtnEnable));
			for (var i = 0; i < 10; i++)
			{
				RunningApp.ScrollDown();
			}
			RunningApp.WaitForNoElement(q => q.Marked("Test label 1"));
			RunningApp.Screenshot("Cannot see the first Label in ScrollView after few scrolls");
		}
#endif
	}
}