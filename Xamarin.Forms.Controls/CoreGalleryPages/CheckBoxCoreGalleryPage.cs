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
		}
	}
}