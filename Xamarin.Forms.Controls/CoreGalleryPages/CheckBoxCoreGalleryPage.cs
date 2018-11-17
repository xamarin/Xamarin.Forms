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

			var isCheckedContainer = new ValueViewContainer<CheckBox>(Test.CheckBox.IsChecked, new CheckBox(), "IsChecked", value => value.ToString());
			Add(isCheckedContainer);

			var checkedColorContainer = new ValueViewContainer<CheckBox>(Test.CheckBox.CheckedColor, new CheckBox() { CheckedColor = Color.Orange }, "Color", value => value.ToString());
			Add(checkedColorContainer);

			var unCheckedColorContainer = new ValueViewContainer<CheckBox>(Test.CheckBox.UnCheckedColor, new CheckBox() { UnCheckedColor = Color.Pink }, "Color", value => value.ToString());
			Add(unCheckedColorContainer);
		}
	}
}