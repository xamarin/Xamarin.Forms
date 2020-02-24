using NUnit.Framework;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Core.UITests
{
	[TestFixture]
	[Category(UITestCategories.DatePicker)]
	internal class DatePickerUITests : _ViewUITests
	{
		public DatePickerUITests()
		{
			PlatformViewType = Views.DatePicker;
		}

		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.DatePickerGallery);
		}

		// View Tests
		// TODO
		public override void _Focus()
		{
		}

		[UiTestExempt(ExemptReason.CannotTest, "Invalid interaction")]
		public override void _GestureRecognizers()
		{
		}

		// TODO
		public override void _IsFocused()
		{
		}

		// TODO
		public override void _UnFocus()
		{
		}

		// TODO
		// Implement control specific ui tests

		protected override void FixtureTeardown()
		{
			App.NavigateBack();
			base.FixtureTeardown();
		}

#if __ANDROID__ || __IOS__
		[Ignore("This is covered by the platform tests")]
		public override void _Opacity() { }
#endif

#if __ANDROID__ || __IOS__ || __WINDOWS__
		[Ignore("This is covered by the platform tests")]
		public override void _IsEnabled() { }
#endif

#if __ANDROID__ || __IOS__ || __WINDOWS__
		[Ignore("This is covered by the platform tests")]
		public override void _Rotation() { }

		[Ignore("This is covered by the platform tests")]
		public override void _RotationX() { }

		[Ignore("This is covered by the platform tests")]
		public override void _RotationY() { }
#endif

#if __ANDROID__
		[Ignore("This is covered by the platform tests")]
		public override void _TranslationX() { }

		[Ignore("This is covered by the platform tests")]
		public override void _TranslationY() { }
#endif

#if __IOS__ || __WINDOWS__
		[Ignore("This is covered by the platform tests")]
		public override void _Scale() { }
#endif

#if __ANDROID__ || __IOS__
		[Ignore("This is covered by the platform tests")]
		public override void _IsVisible() { }
#endif
	}
}