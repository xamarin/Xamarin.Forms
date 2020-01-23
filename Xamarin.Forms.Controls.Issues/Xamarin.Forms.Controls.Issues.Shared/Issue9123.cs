using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif
namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9123, "[Bug] UWP Button BackgroundColor turns gray when hovering",
		PlatformAffected.UWP)]
	public class Issue9123 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Issue 9123";

			StackLayout layout = new StackLayout();

			Label instructions = new Label
			{
				Text = "Run this test in UWP with Target Platform Version above 17134.\n\n"
					 + "Check that the BackgroundColor of the Button below stays yellow when hovering the mouse pointer.\n\n"
					 + "Likewise, check that the BackgroundColor of the ImageButton with the coffee cup stays yellow-green, when hovering over it."
			};

			Button testButton = new Button
			{
				Text = "Yellow Button",
				BackgroundColor = Color.Yellow
			};

			ImageButton testImageButton = new ImageButton
			{
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.End,
				Source = "coffee.png",
				BackgroundColor = Color.YellowGreen
			};

			layout.Children.Add(instructions);
			layout.Children.Add(testButton);
			layout.Children.Add(testImageButton);

			Content = layout;
		}
	}
}
