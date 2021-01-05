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
	[Issue(IssueTracker.None, 0, "Shell Flyout Background",
		   PlatformAffected.All)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class ShellFlyoutBackground : TestShell
	{
		public ShellFlyoutBackground()
		{
		}

		protected override void Init()
		{
			AddFlyoutItem(CreateContentPage(), "Hello");
			AddFlyoutItem(CreateContentPage(), "Hello2");
			FlyoutBackgroundImage = "photo.jpg";
			FlyoutBackgroundImageAspect = Aspect.AspectFill;
		}

		ContentPage CreateContentPage()
		{
			var layout = new StackLayout()
			{
				Children =
				{
					new Button()
					{
						Text = "Toggle Image",
						Command = new Command(() =>
						{
							if(FlyoutBackgroundImage == null)
								FlyoutBackgroundImage = "photo.jpg";
							else
								FlyoutBackgroundImage = null;
						})
					},
					new Button()
					{
						Text = "Toggle Color",
						Command = new Command(() =>
						{
							FlyoutBackground = null;
							if(FlyoutBackgroundColor == Color.Default)
								FlyoutBackgroundColor = Color.Red;
							else
								FlyoutBackgroundColor = Color.Default;
						})
					},
					new Button()
					{
						Text = "Toggle Color With Alpha (doesn't work correctly)",
						Command = new Command(() =>
						{
							FlyoutBackground = null;
							if(FlyoutBackgroundColor == Color.Default)
								FlyoutBackgroundColor = Color.Red.MultiplyAlpha(0.7);
							else
								FlyoutBackgroundColor = Color.Default;
						})
					},
					new Button()
					{
						Text = "Toggle Brush",
						Command = new Command(() =>
						{
							RadialGradientBrush radialGradientBrush = new RadialGradientBrush(
								new GradientStopCollection()
								{
									new GradientStop(Color.Red, 0.1f),
									new GradientStop(Color.DarkBlue, 1.0f),
								});

							FlyoutBackgroundColor = Color.Default;
							if(FlyoutBackground != null)
								FlyoutBackground = null;
							else
								FlyoutBackground = radialGradientBrush;
						})
					},
					new Button()
					{
						Text = "Toggle Flyout Content",
						Command = new Command(() =>
						{
							if(FlyoutContent == null)
								FlyoutContent = new Label() { Text = "hello" };
							else
								FlyoutContent = null;
						})
					}
				}
			};

			Button aspectBackgroundChange = null;
			aspectBackgroundChange = new Button()
			{
				Text = $"Change Flyout Background Image Aspect: {FlyoutBackgroundImageAspect}",
				Command = new Command(() =>
				{
					int inc = (int)FlyoutBackgroundImageAspect;
					inc++;

					if(inc >= Enum.GetNames(typeof(Aspect)).Length)
					{
						inc = 0;
					}

					FlyoutBackgroundImageAspect = (Aspect)inc;
					aspectBackgroundChange.Text = $"Change Flyout Background Image Aspect: {FlyoutBackgroundImageAspect}";
				})
			};

			layout.Children.Add(aspectBackgroundChange);

			return new ContentPage()
			{
				Content = layout
			};
		}


#if UITEST
		[Test]
		public void FlyoutBackgroundTest()
		{
			TapInFlyout("Hello2");
		}
#endif
	}
}
