using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 9999, "Button FastRenderers", PlatformAffected.All)]
	public class ButtonFastRendererTest : TestContentPage
	{
		protected override void Init()
		{
			var img = new Image { Source = "cover1.jpg", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			var btn = new Button { AutomationId = "btnHello", Text = "hello", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			btn.Clicked += (sender, e) => { DisplayAlert("hello", "message", "ok"); };
			var grd = new Grid();
			grd.Children.Add(btn);
			grd.Children.Add(img);
			Content = grd;
		}

#if UITEST
		[Test]
		public void TestButtonUsingElevation ()
		{
			RunningApp.WaitForNoElement("btnHello");
		}
#endif
	}
}
