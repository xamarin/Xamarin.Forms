using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Threading;
using System.ComponentModel;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Shell Modal Behavior Test",
		PlatformAffected.All)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class ShellModalBehavior : TestShell
	{
		protected override void Init()
		{
			Routing.RegisterRoute(nameof(ShellModalBehavior), typeof(ModalTestPage));
			AddContentPage(new ContentPage()
			{
				Content = new StackLayout()
				{
					Children =
					{
						new Button()
						{
							Text = "Push Modal",
							Command = new Command(() =>
							{
								GoToAsync(nameof(ShellModalBehavior));
							})
						},
						new Button()
						{
							Text = "Goto Different Tab and Push Modal",
							Command = new Command(() =>
							{
								GoToAsync("//OtherTab/ShellModalBehavior");
							})
						}
					}
				}
			}, title: "MainContent");

			AddTopTab("OtherTab");
		}

		[Preserve(AllMembers = true)]
		[QueryProperty("IsModal", "IsModal")]
		public class ModalTestPage : ContentPage
		{
			public string IsModal
			{
				set
				{
					Shell.GetModalBehavior(this).Modal = Convert.ToBoolean(value);
				}
			}

			public ModalTestPage()
			{
				Shell.SetModalBehavior(this, new ModalBehavior());

				Content = new StackLayout()
				{
					Children =
					{
						new Label()
						{
							Text = "Hello I am a modal page"
						},
						new Button()
						{
							Text = "Clicking me should go back to the MainContent Page",
							Command = new Command(async () =>
							{
								await Shell.Current.GoToAsync("//MainContent");
							})
						},
						new Button()
						{
							Text = "Push Another Modal Page",
							Command = new Command(async () =>
							{
								await Shell.Current.GoToAsync($"{nameof(ShellModalBehavior)}?IsModal=true");
							})
						},
						new Button()
						{
							Text = "Push a Content Page Onto Previous Modal Page",
							Command = new Command(async () =>
							{
								await Shell.Current.GoToAsync($"{nameof(ShellModalBehavior)}?IsModal=false");
							})
						}
					}
				};
			}
		}
	}
}
