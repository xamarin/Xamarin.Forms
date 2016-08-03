using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 90000, "Platform Specifics - iOS Blur Effect")]
	public class PlatformSpecifics_iOSBlurEffect : TestContentPage
	{
		protected override void Init()
		{
			var image = new Image { Source = ImageSource.FromFile("crimson.jpg") };
			var box = new BoxView { HeightRequest = 300, WidthRequest = 600 };
			box.On<iOS>().UseBlurEffect(BlurEffectStyle.Light);

			Button button1 = GetButton(box, BlurEffectStyle.None);
			Button button2 = GetButton(box, BlurEffectStyle.ExtraLight);
			Button button3 = GetButton(box, BlurEffectStyle.Light);
			Button button4 = GetButton(box, BlurEffectStyle.Dark);

			var buttons = new StackLayout { Orientation = StackOrientation.Horizontal, Children = { button1, button2, button3, button4 }};

			Content = new StackLayout { Children = { buttons, new AbsoluteLayout { Children = { image, box } } } };
		}

		Button GetButton(BoxView box, BlurEffectStyle value)
		{
			var button1 = new Button { Text = value.ToString(), Margin = 25 };
			button1.Clicked += (s, e) => { box.On<iOS>().UseBlurEffect(value); };
			return button1;
		}
	}
}