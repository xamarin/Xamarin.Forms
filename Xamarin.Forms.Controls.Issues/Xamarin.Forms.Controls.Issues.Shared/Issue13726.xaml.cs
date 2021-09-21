using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13726,
		"[Bug] Clicks on elements within a SwipeView are buggy",
		PlatformAffected.Android)]
	public partial class Issue13726 : TestContentPage
	{
#if APP
		int _buttonCounter = 0;
		int _swipeViewCounter = 0;
#endif

		public Issue13726()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
		}

#if APP
		void OnButtonClicked(object sender, EventArgs e)
		{
			_buttonCounter++;
			ButtonInfoLabel.Text = $"SwipeView Button tapped {_buttonCounter} times";
		}

		void OnSwipeViewButtonClicked(object sender, EventArgs e)
		{
			_swipeViewCounter++;
			SwipeViewInfoLabel.Text = $"SwipeView Button tapped {_swipeViewCounter} times";
		}
#endif

#if UITEST && __ANDROID__
		[Test]
		public void TouchSwipeViewContentTest()
		{
			string swipeViewButtonId = "SwipeViewButtonId";
			string swipeViewInfoLabelId = "SwipeViewInfoLabelId";

			RunningApp.WaitForElement(q => q.Marked(swipeViewButtonId));

			for (int i = 0; i < 10; i++)
				RunningApp.Tap(q => q.Marked(swipeViewButtonId));

			RunningApp.SwipeRightToLeft(q => q.Marked(swipeViewButtonId));

			for (int i = 0; i < 10; i++)
				RunningApp.Tap(q => q.Marked(swipeViewButtonId));

			var infoLabel = RunningApp.WaitForFirstElement(swipeViewInfoLabelId);

			Assert.AreEqual("SwipeView Button tapped 20 times", infoLabel.ReadText());
		}
#endif
	}
}