using NUnit.Framework;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Core.UITests
{
	[TestFixture]
	[Category(UITestCategories.WebView)]
	internal class WebViewUITests : _ViewUITests
	{
		public WebViewUITests()
		{
			PlatformViewType = Views.WebView;
		}

		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.WebViewGallery);
		}

		[Category(UITestCategories.ManualReview)]
		public override void _IsEnabled()
		{
			Assert.Inconclusive("Does not make sense for WebView");
		}

		
		[Category(UITestCategories.ManualReview)]
		[Ignore("Keep empty test from failing in Test Cloud")]
		public override void _IsVisible()
		{
		}

		[UiTestExempt(ExemptReason.CannotTest, "Invalid interaction with Label")]
		public override void _Focus()
		{
		}

		// TODO
		public override void _GestureRecognizers()
		{
		}

		[UiTestExempt(ExemptReason.CannotTest, "Invalid interaction with Label")]
		public override void _IsFocused()
		{
		}

		
		[Category(UITestCategories.ManualReview)]
		[Ignore("Keep empty test from failing in Test Cloud")]
		public override void _Opacity()
		{
		}

		
		[Category(UITestCategories.ManualReview)]
		[Ignore("Keep empty test from failing in Test Cloud")]
		public override void _Rotation()
		{
		}

		
		[Category(UITestCategories.ManualReview)]
		[Ignore("Keep empty test from failing in Test Cloud")]
		public override void _RotationX()
		{
		}

		
		[Category(UITestCategories.ManualReview)]
		[Ignore("Keep empty test from failing in Test Cloud")]
		public override void _RotationY()
		{
		}

		
		[Category(UITestCategories.ManualReview)]
		[Ignore("Keep empty test from failing in Test Cloud")]
		public override void _TranslationX()
		{
		}

		
		[Category(UITestCategories.ManualReview)]
		[Ignore("Keep empty test from failing in Test Cloud")]
		public override void _TranslationY()
		{
		}

		
		[Category(UITestCategories.ManualReview)]
		[Ignore("Keep empty test from failing in Test Cloud")]
		public override void _Scale()
		{
		}

		[UiTestExempt(ExemptReason.CannotTest, "Invalid interaction with Label")]
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
	}
}