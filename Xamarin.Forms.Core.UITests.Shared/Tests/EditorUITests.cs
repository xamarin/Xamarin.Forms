using NUnit.Framework;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Core.UITests
{
	[TestFixture]
	[Category(UITestCategories.Editor)]
	internal class EditorUITests : _ViewUITests
	{
		public EditorUITests()
		{
			PlatformViewType = Views.Editor;
		}

		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.EditorGallery);
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

		[Test]
		[UiTest(typeof(Editor))]
		public void CursorPositionSelectionLength()
		{
			var remote = new ViewContainerRemote(App, Test.Editor.CursorPositionSelectionLength, PlatformViewType);
			remote.GoTo();

			App.Tap("Set CursorPosition");
			App.EnterText("5");
			App.PressEnter();

			App.Tap("Set SelectionLength");
			App.EnterText("10");
			App.PressEnter();

			App.Tap("Set Values");

			Assert.AreEqual(5, CursorPosition());
			Assert.AreEqual(10, SelectionLength());
		}

		[Test]
		[UiTest(typeof(Editor))]
		public void CursorPositionSelectionLengthInit()
		{
			var remote = new ViewContainerRemote(App, Test.Editor.CursorPositionSelectionLengthInit, PlatformViewType);
			remote.GoTo();

			Assert.AreEqual(17, CursorPosition());
			Assert.AreEqual(14, SelectionLength());
		}

		[Test]
		[UiTest(typeof(Editor), nameof(Editor.CursorPosition))]
		public void CursorPositionInvalidInit()
		{
			var remote = new ViewContainerRemote(App, Test.Editor.CursorPositionInvalidInit, PlatformViewType);
			remote.GoTo();

			int textLength = remote.GetView().Text.Length;

			Assert.AreEqual(textLength, CursorPosition());
			Assert.AreEqual(0, SelectionLength());
		}

		[Test]
		[UiTest(typeof(Editor), nameof(Editor.SelectionLength))]
		public void SelectionLengthInvalidInit()
		{
			var remote = new ViewContainerRemote(App, Test.Editor.SelectionLengthInvalidInit, PlatformViewType);
			remote.GoTo();

			int textLength = remote.GetView().Text.Length;

			Assert.AreEqual(textLength, CursorPosition());
			Assert.AreEqual(0, SelectionLength());
		}

		protected override void FixtureTeardown()
		{
			App.NavigateBack();
			base.FixtureTeardown();
		}

		int CursorPosition()
		{
			string cursorPosition = App.Query("CursorPositionLabel")[0].ReadText();
			return int.Parse(cursorPosition);
		}

		int SelectionLength()
		{
			string selectionLength = App.Query("SelectionLengthLabel")[0].ReadText();
			return int.Parse(selectionLength);
		}
	}
}