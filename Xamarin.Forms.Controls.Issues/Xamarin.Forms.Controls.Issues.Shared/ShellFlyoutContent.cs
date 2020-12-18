using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Shell Flyout Content",
		PlatformAffected.All)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class ShellFlyoutContent : TestShell
	{
		protected override void Init()
		{
			var page = new ContentPage();

			AddFlyoutItem(page, "Flyout Item");

			var layout = new StackLayout()
			{
				Children =
				{
					new Label()
					{
						Text = "Open the Flyout and Toggle the Content, Header and Footer. If it changes after each click test has passed",
						AutomationId = "PageLoaded"
					}
				}
			};

			page.Content = layout;

			layout.Children.Add(new Button()
			{
				Text = "Toggle Flyout Content Template",
				Command = new Command(() =>
				{
					if (FlyoutContentTemplate == null)
					{
						FlyoutContentTemplate = new DataTemplate(() =>
						{
							return new Label()
							{
								Background = SolidColorBrush.LightBlue,
								Text = "Flyout Content Template" 
							};
						});
					}
					else if (FlyoutContentTemplate != null)
					{
						FlyoutContentTemplate = null;
					}
				}),
				AutomationId = "ToggleFlyoutContentTemplate"
			});

			layout.Children.Add(new Button()
			{
				Text = "Toggle Flyout Content",
				Command = new Command(() =>
				{
					if (FlyoutContent != null)
					{
						FlyoutContent = null;
					}
					else
					{
						FlyoutContent = new StackLayout()
						{
							Background = SolidColorBrush.Green,
							Children = {
								new Label() { Text = "Content View" }
							},
							AutomationId = "ContentView"
						};
					}
				}),
				AutomationId = "ToggleContent"
			});

			layout.Children.Add(new Button()
			{
				Text = "Toggle Header/Footer View",
				Command = new Command(() =>
				{
					if (FlyoutHeader != null)
					{
						FlyoutHeader = null;
						FlyoutFooter = null;
					}
					else
					{
						FlyoutHeader = new StackLayout()
						{
							Children = {
								new Label() { Text = "Header" }
							},
							AutomationId = "Header View"
						};

						FlyoutFooter = new StackLayout()
						{
							Background = SolidColorBrush.Orange,
							Orientation = StackOrientation.Horizontal,
							Children = {
								new Label() { Text = "Footer" }
							},
							AutomationId = "Footer View"
						};
					}
				}),
				AutomationId = "ToggleHeaderFooter"
			});
		}


#if UITEST

		[Test]
		public void FlyoutTests()
		{
		}
#endif
	}
}
