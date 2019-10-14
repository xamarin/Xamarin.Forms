using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif


namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5518, "Frame Tap Gesture not working when using Visual=\"Material\" in iOS", PlatformAffected.iOS)]
	class Issue5518 : TestContentPage
	{

		protected override void Init()
		{
			var stack = new StackLayout();


			var frame = new Frame()
			{
				Visual = VisualMarker.Material,
				BackgroundColor = Color.White,
				AutomationId = "NoContentFrame"
			};

			var outputLabel1 = new Label() { Text = "", AutomationId = "Output1", HorizontalOptions = LayoutOptions.Center };

			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += (s, e) =>
			{
				outputLabel1.Text = "Success";
			};

			frame.GestureRecognizers.Add(tapGestureRecognizer);
			stack.Children.Add(frame);
			stack.Children.Add(outputLabel1);

			var frameWithContent = new Frame()
			{
				Visual = VisualMarker.Material,
				Content = new Label() { Text = "I'm label" },
				AutomationId = "ContentedFrame"
			};
			var outputLabel2 = new Label() { Text = "", AutomationId = "Output2", HorizontalOptions = LayoutOptions.Center };

			tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += (s, e) =>
			{
				outputLabel2.Text = "Success";
			};

			frameWithContent.GestureRecognizers.Add(tapGestureRecognizer);
			stack.Children.Add(frameWithContent);
			stack.Children.Add(outputLabel2);

			Content = stack;
		}

		private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			DisplayAlert("Frame Tap Gesture", "Work", "Ok");
		}

#if UITEST
		[Test]
		public void FrameTapGestureRecognizer()
		{
			RunningApp.WaitForElement("NoContentFrame");
			RunningApp.Tap("NoContentFrame");
			RunningApp.WaitForElement(q => q.Id("Output1").Text("Success"));

			RunningApp.WaitForElement("ContentedFrame");
			RunningApp.Tap("ContentedFrame");
			RunningApp.WaitForElement(q => q.Id("Output2").Text("Success"));
		}
#endif
	}
}
