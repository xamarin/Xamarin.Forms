using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls
{
	class RadioButtonCoreGalleryPage : CoreGalleryPage<RadioButton>
	{
		protected override bool SupportsFocus => false;
		protected override bool SupportsTapGestureRecognizer => true;
		protected override void InitializeElement(RadioButton element)
		{
			Device.SetFlags(new List<string> { ExperimentalFlags.RadioButtonExperimental });
			element.Content = "RadioButton";
		}

		protected override void OnDisappearing()
		{
			Device.SetFlags(new List<string>());
			base.OnDisappearing();
		}

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);

			// TODO ezhart IsCheckedChanged, maybe?
			//IsEnabledStateViewContainer.View.Clicked += (sender, args) => IsEnabledStateViewContainer.TitleLabel.Text += " (Tapped)";

			var borderButtonContainer = new ViewContainer<RadioButton>(Test.Button.BorderColor,
				new RadioButton
				{
					Content = "BorderColor",
					BackgroundColor = Color.Transparent,
					BorderColor = Color.Red,
					BorderWidth = 1,
				}
			);

			var borderRadiusContainer = new ViewContainer<RadioButton>(Test.Button.BorderRadius,
				new RadioButton
				{
					Content = "BorderRadius",
					BackgroundColor = Color.Transparent,
					BorderColor = Color.Red,
					BorderWidth = 1,
				}
			);

			var borderWidthContainer = new ViewContainer<RadioButton>(Test.Button.BorderWidth,
				new RadioButton
				{
					Content = "BorderWidth",
					BackgroundColor = Color.Transparent,
					BorderColor = Color.Red,
					BorderWidth = 15,
				}
			);

			var fontContainer = new ViewContainer<RadioButton>(Test.Button.Font,
				new RadioButton
				{
					Content = "Font",
					FontSize = Device.GetNamedSize(NamedSize.Large, typeof(RadioButton)),
					FontAttributes = FontAttributes.Bold
				}
			);

			var textContainer = new ViewContainer<RadioButton>(Test.Button.Text,
				new RadioButton
				{
					Content = "Text"
				}
			);

			var textColorContainer = new ViewContainer<RadioButton>(Test.Button.TextColor,
				new RadioButton
				{
					Content = "TextColor",
					TextColor = Color.Pink
				}
			);

			var paddingContainer = new ViewContainer<RadioButton>(Test.Button.Padding,
				new RadioButton
				{
					Content = "Padding",
					BackgroundColor = Color.Red,
					Padding = new Thickness(20, 30, 60, 15)
				}
			);

			var isCheckedContainer = new ValueViewContainer<RadioButton>(Test.RadioButton.IsChecked, new RadioButton() { IsChecked = true, HorizontalOptions = LayoutOptions.Start }, "IsChecked", value => value.ToString());

			//var checkedVisualState = new VisualState { Name = "IsChecked" };
			//checkedVisualState.Setters.Add(new Setter { Property = RadioButton.ButtonSourceProperty, Value = "rb_checked" });

			//var group = new VisualStateGroup();
			//group.States.Add(checkedVisualState);

			//var normalVisualState = new VisualState{  Name = "Normal" };
			//normalVisualState.Setters.Add(new Setter { Property = RadioButton.ButtonSourceProperty, Value = "rb_unchecked" });
			//group.States.Add(normalVisualState);

			//var groupList = new VisualStateGroupList();
			//groupList.Add(group);

			//var rbStateManaged = new RadioButton() { HorizontalOptions = LayoutOptions.Start };
			//VisualStateManager.SetVisualStateGroups(rbStateManaged, groupList);

			//var stateManagedContainer = new ValueViewContainer<RadioButton>(Test.RadioButton.ButtonSource, rbStateManaged, "IsChecked", value => value.ToString());

			Add(borderButtonContainer);
			Add(borderRadiusContainer);
			Add(borderWidthContainer);
			Add(fontContainer);
			Add(textContainer);
			Add(textColorContainer);
			Add(paddingContainer);
			Add(isCheckedContainer);
			//Add(stateManagedContainer);
		}
	}
}
