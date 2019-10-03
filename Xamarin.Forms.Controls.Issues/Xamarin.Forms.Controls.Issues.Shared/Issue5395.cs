using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5395, "[macOs] Image Rotation issue", PlatformAffected.macOS)]
	public class Issue5395 : ContentPage
	{
		public Issue5395()
		{
			var sl = new StackLayout();
			sl.Children.Add(new Label() { Text = "Image should scale and rotate clockwise around its center" });
			sl.Children.Add(new Rotator());
			Content = sl; 
		}

		class Rotator : AbsoluteLayout
		{
			public Rotator()
			{
				var image = new Image { Aspect = Aspect.AspectFit, Source = "bank.png" };
				Children.Add(image, new Rectangle(.5, .5, .5, .5), AbsoluteLayoutFlags.All);
				VerticalOptions = HorizontalOptions = LayoutOptions.FillAndExpand;
				image.RotateTo(3600, 10000);
				image.ScaleTo(4, 10000);
			}
		}
	}
}

