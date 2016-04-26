using System;

using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 27729, "Image does not scale with Aspect when inside a StackLayout with a WidthRequest")]
	public class Bugzilla27729 : TestContentPage
	{
		const string image1 = "xamagon.png";

		protected override void Init()
		{
			var image = new Image {
				Aspect = Aspect.AspectFit,
			};

			var label = new Label { BackgroundColor = Color.Pink };

			var layout = new StackLayout {
				Padding = 0,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				WidthRequest = 400,
				BackgroundColor = Color.Green
			};

			layout.Children.Add(image);
			layout.Children.Add(label);

			Content = layout;

			image.Source = image1;
			label.Text = image1;
		}

#if UITEST
		[Test]
		public void Bugzilla27729Test()
		{
			RunningApp.WaitForElement(q => q.Marked(image1));
			RunningApp.Screenshot("I see the Xamagon fit the width of the stacklayout with no Green background on the sides");
		}
#endif
	}
}
