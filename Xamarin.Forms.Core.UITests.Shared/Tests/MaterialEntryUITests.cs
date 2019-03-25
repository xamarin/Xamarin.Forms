using NUnit.Framework;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Core.UITests
{

#if __ANDROID__ || __IOS__
	[TestFixture]
	[Category(UITestCategories.Entry)]
	[Category(UITestCategories.Visual)]
	internal class MaterialEntryUITests : EntryUITests
	{
		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.EntryGallery, "Material");
		}

		[Test]
		[UiTest(typeof(Entry), "Focus")]
		public override void _Focus()
		{
			base._Focus();
		}

		[Test]
		[UiTest(typeof(Entry), "Rotation")]
		public override void _Rotation()
		{
			base._Rotation();
		}

		[Test]
		[UiTest(typeof(Entry), "RotationX")]
		public override void _RotationX()
		{
			base._RotationX();
		}

		[Test]
		[UiTest(typeof(Entry), "RotationY")]
		public override void _RotationY()
		{
			base._RotationY();
		}

		[Test]
		[UiTest(typeof(Entry), "Opacity")]
		public override void _Opacity()
		{
			base._Opacity();
		}

		[Test]
		[UiTest(typeof(Entry), "Scale")]
		public override void _Scale()
		{
			base._Scale();
		}
	}
#endif
}