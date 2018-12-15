using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls
{
	internal class CheckBoxCoreGalleryPage : CoreGalleryPage<CheckBox>
	{
		protected override bool SupportsFocus
		{
			get { return false; }
		}

		protected override bool SupportsTapGestureRecognizer
		{
			get { return false; }
		}

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);

			var isCheckedContainer = new ValueViewContainer<CheckBox>(Test.CheckBox.IsChecked, new CheckBox() { HorizontalOptions = LayoutOptions.Start }, "IsChecked", value => value.ToString());
			Add(isCheckedContainer);

			var checkedColorContainer = new ValueViewContainer<CheckBox>(Test.CheckBox.CheckedColor, new CheckBox() { CheckedColor = Color.Orange, HorizontalOptions = LayoutOptions.Start }, "Color", value => value.ToString());
			Add(checkedColorContainer);

			var unCheckedColorContainer = new ValueViewContainer<CheckBox>(Test.CheckBox.UncheckedColor, new CheckBox() { UncheckedColor = Color.Pink, HorizontalOptions = LayoutOptions.Start }, "Color", value => value.ToString());
			Add(unCheckedColorContainer);
		}
	}
}