using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 70000, "Platform Specifics - Android AdjustPan/AdjustResize")]
	public class PlatformSpecifics_AndroidSoftInputAdjust : TestContentPage
	{
		protected override void Init()
		{
			BackgroundColor = Color.Pink;

			var button1 = GetButton(WindowSoftInputModeAdjust.Pan);
			var button2 = GetButton(WindowSoftInputModeAdjust.Resize);

			var buttons = new StackLayout { Orientation = StackOrientation.Horizontal, Children = { button1, button2 }, VerticalOptions = LayoutOptions.Start };

			var entry = new Entry { Text = "1", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.StartAndExpand };
			var layout = new RelativeLayout
			{
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalOptions = LayoutOptions.Center,
			};

			layout.Children.Add(buttons, yConstraint: Xamarin.Forms.Constraint.RelativeToParent(parent => { return parent.Y; }));
			layout.Children.Add(entry, yConstraint: Xamarin.Forms.Constraint.RelativeToParent(parent => { return parent.Height - 100; }));

			Content = layout;
		}

		static Button GetButton(WindowSoftInputModeAdjust value)
		{
			var button = new Button { Text = value.ToString(), Margin = 20 };
			button.Clicked += (sender, args) =>
			{
				Application.Current.On<Android>().UseWindowSoftInputModeAdjust(value);
			};
			return button;
		}
	}
}