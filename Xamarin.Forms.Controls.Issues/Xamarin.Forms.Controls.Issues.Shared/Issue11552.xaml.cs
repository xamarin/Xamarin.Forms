using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11552, "Make FontImageExtension Color property bindable", PlatformAffected.All)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.ManualReview)]
#endif
	public partial class Issue11552 : TestContentPage
	{
		private const string IconId = "icon";

		public static string FontFamily
		{
			get
			{
				var fontFamily = "";
				switch (Device.RuntimePlatform)
				{
					case Device.iOS:
						fontFamily = "Ionicons";
						break;
					case Device.UWP:
						fontFamily = "Assets/Fonts/ionicons.ttf#ionicons";
						break;
					case Device.Android:
					default:
						fontFamily = "fonts/ionicons.ttf#";
						break;
				}

				return fontFamily;
			}

		}
		

		protected override void Init()
		{
		}

#if APP
		public Issue11552()
		{
			InitializeComponent();
			
		}
#endif
#if UITEST
		[Test]
		public void AppThemeBindingFontImage()
		{
			RunningApp.WaitForElement(IconId);
			RunningApp.Screenshot("Icon color");
		}

		
#endif
	}

}