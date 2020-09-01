﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;
using System.Threading;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10182, "[Bug] Exception Ancestor must be provided for all pushes except first", PlatformAffected.Android, NavigationBehavior.SetApplicationRoot)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(UITestCategories.LifeCycle)]
#endif
	public class Issue10182 : TestContentPage
	{
		public Issue10182()
		{
			
		}

		protected override void Init()
		{
			Content = new StackLayout()
			{
				Children =
				{
					new Label()
					{
						Text = "Starting Activity to Test Changing Page on Resume. If success label shows up test has passed."
					}
				}
			};

#if !UITEST
			Device.BeginInvokeOnMainThread(() =>
			{
				DependencyService.Get<IMultiWindowService>().OpenWindow(this.GetType());
			});
#endif

		}

		public class Issue10182SuccessPage : ContentPage
		{ 
			public Issue10182SuccessPage()
			{
				Content = new StackLayout()
				{
					Children =
					{
						new Label()
						{
							Text = "Success.",
							AutomationId = "Success"
						}
					}
				};
			}
		}

#if UITEST && __ANDROID__
		[Test]
		public void AppDoesntCrashWhenResettingPage()
		{
			RunningApp.WaitForElement("Success");
		}
#endif
	}
}
