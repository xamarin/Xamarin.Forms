﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest.Queries;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7823, "[Bug] Frame corner radius.", PlatformAffected.Android)]
#if UITEST
	[Category(UITestCategories.Frame)]
#endif
	public class Issue7823 : TestContentPage
	{
		const string GetClipToOutline = "getClipToOutline";
		const string GetClipChildren = "getClipChildren";
		const string GetClipBounds = "getClipBounds";
		const string SetClipBounds = "SetClipBounds";
		const string SecondaryFrame = "Secondary Frame";
		const string RootFrame = "Root Frame";
		const string BoxView = "Box View";

		protected override void Init()
		{
			var frameClippedToBouds = new Frame
			{
				AutomationId = SecondaryFrame,
				CornerRadius = 10,
				BackgroundColor = Color.Blue,
				Padding = 0,
				Content = new BoxView
				{
					AutomationId = BoxView,
					BackgroundColor = Color.Green,
					WidthRequest = 100,
					HeightRequest = 100
				}
			};

			Content = new StackLayout()
			{
				Children =
				{
					new ApiLabel(),
					new Frame
					{
						AutomationId = RootFrame,
						CornerRadius = 5,
						BackgroundColor = Color.Red,
						Padding = 10,
						Content = frameClippedToBouds
					},
					new Button
					{
						AutomationId = SetClipBounds,
						Text = "Manually set Frame.IsClippedToBounds = false",
						Command = new Command(()=>
						{
							frameClippedToBouds.IsClippedToBounds = false;
							frameClippedToBouds.CornerRadius = 11;
						})
					}
				}
			};
		}

#if UITEST && __ANDROID__
		[Test]
		[UiTest(typeof(Frame))]
		public void Issue7823TestIsClippedIssue()
		{
			RunningApp.WaitForElement(RootFrame);
			AssertIsClipped(true);
			RunningApp.Tap(SetClipBounds);
			AssertIsClipped(false);
		}

		void AssertIsClipped(bool expected)
		{
			var clipChildrenValue = RunningApp.InvokeFromElement<bool>(SecondaryFrame, GetClipChildren)[0];
			if (RunningApp.IsApiHigherThan(17))
			{
				var clipBoundsRec = RunningApp.InvokeFromElement<object>(RootFrame, GetClipBounds)[0];
				Assert.AreEqual(expected, clipBoundsRec?.ToString()?.Contains("\"empty\": false"));
				Assert.AreNotEqual(expected, clipChildrenValue);
			}
			else
			{
				Assert.AreEqual(expected, clipChildrenValue);
			}
		}
#endif
	}
}
