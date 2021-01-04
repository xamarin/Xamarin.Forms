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
	[Issue(IssueTracker.Github, 12245, "[iOS] RadioButtons not re-rendering correctly in Shell",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
	[NUnit.Framework.Category(UITestCategories.RadioButton)]
#endif
	public class Issue12245 : TestShell
	{
		protected override void Init()
		{
			AddFlyoutItem(CreatePage(), "Item 1");
			AddFlyoutItem(CreatePage(), "Item 2");
		}

		ContentPage CreatePage()
		{
			ContentPage contentPage = new ContentPage();

			contentPage.Content =
				new StackLayout()
				{
					Children =
					{
						new RadioButton()
						{
							ControlTemplate = RadioButton.DefaultTemplate,
							/*ControlTemplate = new ControlTemplate(() =>
							{
								return new StackLayout()
								{
									Orientation = StackOrientation.Horizontal,
									Children =
									{
										new Label(){ Text = "Hello" },
										new ContentPresenter()
									}
								};
							}),*/
							Content = "Content"
						}
					}
				};

			return contentPage;
		}


#if UITEST
		[Test]
		public void RadioButtonRenderer()
		{
			RunningApp.Tap(Go);
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}
