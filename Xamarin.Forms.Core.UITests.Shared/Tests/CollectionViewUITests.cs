﻿using NUnit.Framework;

namespace Xamarin.Forms.Core.UITests
{
	[Category(UITestCategories.CollectionView)]
	internal class CollectionViewUITests : BaseTestFixture
	{
		string _collectionViewId = "collectionview";
		string _enableCollectionView = "Enable CollectionView";
		string _btnUpdate = "Update";
		string _entryUpdate = "entryUpdate";
		string _entryInsert = "entryInsert";
		string _entryRemove = "entryRemove";
		string _entryReplace = "entryReplace";
		string _entryScrollTo = "entryScrollTo";
		string _btnInsert = "btnInsert";
		string _btnRemove = "btnRemove";
		string _btnReplace = "btnReplace";
		string _btnGo = "btnGo";
		string _inserted = "Inserted";
		string _replaced = "Replacement";
		string _picker = "pickerSelectItem";
#if __ANDROID__
		string _dialogAndroidFrame = "select_dialog_listview";
#else
		string _pickeriOSFrame = "UIPickerTableView";
#endif

		public CollectionViewUITests()
		{
		}

		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.CollectionViewGallery);
		}

		protected override void TestTearDown()
		{
			base.TestTearDown();
			ResetApp();
			NavigateToGallery();
		}
		//#if __ANDROID__
		//		[TestCase("CarouselView", new string[] { "CarouselViewCode,Horizontal", "CarouselViewCode,Vertical" }, 19, 6)]
		//#endif
		//[TestCase("ScrollTo", new string[] {
		//	"ScrollToIndexCode,HorizontalList", "ScrollToIndexCode,VerticalList", "ScrollToIndexCode,HorizontalGrid", "ScrollToIndexCode,VerticalGrid",
		//	"ScrollToItemCode,HorizontalList", "ScrollToItemCode,VerticalList", "ScrollToItemCode,HorizontalGrid", "ScrollToItemCode,VerticalGrid",
		//  }, 19, 3)]
		//[TestCase("Snap Points", new string[] { "SnapPointsCode,HorizontalList", "SnapPointsCode,VerticalList", "SnapPointsCode,HorizontalGrid", "SnapPointsCode,VerticalGrid" }, 19, 2)]
		[TestCase("Observable Collection", new string[] { "Add/RemoveItemsList", "Add/RemoveItemsGrid" }, 19, 6)]
		[TestCase("Default Text", new string[] { "VerticalListCode", "HorizontalListCode", "VerticalGridCode", "HorizontalGridCode" }, 101, 11)]
		[TestCase("DataTemplate", new string[] { "VerticalListCode", "HorizontalListCode", "VerticalGridCode", "HorizontalGridCode" }, 19, 6)]
		public void VisitAndUpdateItemsSource(string collectionTestName, string[] subGalleries, int firstItem, int lastItem)
		{
			VisitInitialGallery(collectionTestName);

			foreach (var gallery in subGalleries)
			{
				if (gallery == "FilterItems")
					continue;

				VisitSubGallery(gallery, !gallery.Contains("Horizontal"), $"Item: {firstItem}", $"Item: {lastItem}", lastItem - 1, true, false);
				App.NavigateBack();
			}
		}

		//[TestCase("ScrollTo", new string[] {
		//	"ScrollToIndexCode,HorizontalList", "ScrollToIndexCode,VerticalList", "ScrollToIndexCode,HorizontalGrid", "ScrollToIndexCode,VerticalGrid",
		//	"ScrollToItemCode,HorizontalList", "ScrollToItemCode,VerticalList", "ScrollToItemCode,HorizontalGrid", "ScrollToItemCode,VerticalGrid",
		//  }, 1, 20)]
		//public void ScrollTo(string collectionTestName, string[] subGalleries, int firstItem, int goToItem)
		//{
		//	VisitInitialGallery(collectionTestName);

		//	foreach (var galleryName in subGalleries)
		//	{
		//		if (galleryName == "FilterItems")
		//			continue;

		//		var isVertical = !galleryName.Contains("Horizontal");
		//		var isList = !galleryName.Contains("Grid");
		//		var isItem = !galleryName.Contains("Index");
		//		if (isItem)
		//		{
		//			TestScrollToItem(firstItem, goToItem, galleryName, isList);
		//		}
		//		else
		//		{
		//			TestScrollToIndex(firstItem, goToItem, galleryName, isList);
		//		}
		//		App.Back();
		//	}
		//}

		void TestScrollToItem(int firstItem, int goToItem, string galleryName, bool isList)
		{
			App.WaitForElement(t => t.Marked(galleryName));
			App.Tap(t => t.Marked(galleryName));
			App.WaitForElement(t => t.Marked(_picker));
			App.Tap(t => t.Marked(_picker));

			var firstItemMarked = $"Item: {firstItem}";
			var goToItemMarked = isList ? $"Item: {goToItem}" : $"Item: {goToItem - 1}";
			App.WaitForElement(firstItemMarked);
#if __ANDROID__
			var pickerDialogFrame = App.Query(q => q.Marked(_dialogAndroidFrame))[0].Rect;
#else
			var pickerDialogFrame = App.Query(q => q.Class(_pickeriOSFrame))[0].Rect;
#endif
			App.ScrollForElement($"* marked:'{goToItemMarked}'", new Drag(pickerDialogFrame, Drag.Direction.BottomToTop, Drag.DragLength.Short));
			App.Tap(goToItemMarked);
			App.DismissKeyboard();
			App.Tap(_btnGo);
			App.WaitForNoElement(c => c.Marked(firstItemMarked));
			App.WaitForElement(c => c.Marked(goToItemMarked));
		}

		void TestScrollToIndex(int firstItem, int goToItem, string galleryName, bool isList)
		{
			App.WaitForElement(t => t.Marked(galleryName));
			App.Tap(t => t.Marked(galleryName));
			App.WaitForElement(t => t.Marked(_entryScrollTo));
			App.ClearText(_entryScrollTo);
			App.EnterText(_entryScrollTo, goToItem.ToString());
			App.DismissKeyboard();
			App.Tap(_btnGo);
			App.WaitForNoElement(c => c.Marked($"Item: {firstItem}"));
			var itemToCheck = isList ? $"Item: {goToItem}" : $"Item: {goToItem - 1}";
			App.WaitForElement(c => c.Marked(itemToCheck));
		}

		[TestCase("Observable Collection", new string[] { "Add/RemoveItemsList", "Add/RemoveItemsGrid" }, 1, 6)]
		public void AddRemoveItems(string collectionTestName, string[] subGalleries, int firstItem, int lastItem)
		{
		VisitInitialGallery(collectionTestName);

		foreach (var gallery in subGalleries)
		{
			if (gallery == "FilterItems")
				continue;

			VisitSubGallery(gallery, !gallery.Contains("Horizontal"), $"Item: {firstItem}", $"Item: {lastItem}", lastItem - 1, false, true);
			App.NavigateBack();
		}
		}

		[TestCase("Observable Collection", new string[] { "Add/RemoveItemsList", "Add/RemoveItemsGrid" }, 19, 6)]
		[TestCase("Default Text", new string[] { "VerticalListCode", "HorizontalListCode", "VerticalGridCode" }, 101, 11)] //HorizontalGridCode
		[TestCase("DataTemplate", new string[] { "VerticalListCode", "HorizontalListCode", "VerticalGridCode", "HorizontalGridCode" }, 19, 6)]
		public void VisitAndTestItemsPosition(string collectionTestName, string[] subGalleries, int firstItem, int lastItem)
		{
			VisitInitialGallery(collectionTestName);

			foreach (var gallery in subGalleries)
			{
				if (gallery == "FilterItems")
					continue;
				App.WaitForElement(t => t.Marked(gallery));
				App.Tap(t => t.Marked(gallery));
				TesItemsPosition(gallery);
				App.NavigateBack();
			}
		}

		void VisitInitialGallery(string collectionTestName)
		{
			var galeryName = $"{collectionTestName} Galleries";
			App.WaitForElement(t => t.Marked(_enableCollectionView));
			App.Tap(t => t.Marked(_enableCollectionView));

			App.WaitForElement(t => t.Marked(galeryName));
			App.Tap(t => t.Marked(galeryName));
		}

		void VisitSubGallery(string galleryName, bool scrollDown, string lastItem, string firstPageItem, int updateItemsCount, bool testItemSource, bool testAddRemove)
		{
			App.WaitForElement(t => t.Marked(galleryName));
			App.Tap(t => t.Marked(galleryName));

			//let's test the update
			if (testItemSource)
			{
				UITest.Queries.AppRect collectionViewFrame = TestItemsExist(scrollDown, lastItem);
				TestUpdateItemsWorks(scrollDown, firstPageItem, updateItemsCount.ToString(), collectionViewFrame);
			}

			if (testAddRemove)
			{
				TestAddRemoveReplaceWorks(lastItem);
			}
		}

		void TestAddRemoveReplaceWorks(string lastItem)
		{
			App.WaitForElement(t => t.Marked(_entryRemove));
			App.ClearText(_entryRemove);
			App.EnterText(_entryRemove, "1");
			App.DismissKeyboard();
			App.Tap(_btnRemove);
			App.WaitForNoElement(lastItem);
			App.ClearText(_entryInsert);
			App.EnterText(_entryInsert, "1");
			App.DismissKeyboard();
			App.Tap(_btnInsert);
			App.WaitForElement(_inserted);
			//TODO: enable replace
			App.ClearText(_entryReplace);
			App.EnterText(_entryReplace, "1");
			App.DismissKeyboard();
			App.Tap(_btnReplace);
			App.WaitForElement(_replaced);
		}

		void TestUpdateItemsWorks(bool scrollDown, string itemMarked, string updateItemsCount, UITest.Queries.AppRect collectionViewFrame)
		{
			App.WaitForElement(t => t.Marked(_entryUpdate));
			App.ScrollForElement($"* marked:'{itemMarked}'", new Drag(collectionViewFrame, scrollDown ? Drag.Direction.TopToBottom : Drag.Direction.LeftToRight, Drag.DragLength.Long), 50);

			App.ClearText(_entryUpdate);
			App.EnterText(_entryUpdate, updateItemsCount);
			App.DismissKeyboard();
			App.Tap(_btnUpdate);
			App.WaitForNoElement(t => t.Marked(itemMarked));
		}

		UITest.Queries.AppRect TestItemsExist(bool scrollDown, string itemMarked)
		{
			App.WaitForElement(t => t.Marked(_btnUpdate));

			var collectionViewFrame = App.Query(q => q.Marked(_collectionViewId))[0].Rect;
			App.ScrollForElement($"* marked:'{itemMarked}'", new Drag(collectionViewFrame, scrollDown ? Drag.Direction.BottomToTop : Drag.Direction.RightToLeft, Drag.DragLength.Long));
			return collectionViewFrame;
		}

		void TesItemsPosition(string gallery)
		{
			var isVertical = !gallery.Contains("Horizontal");
			var isList = !gallery.Contains("Grid");
			App.WaitForNoElement(t => t.Marked(gallery));

			var element1 = App.Query(c => c.Marked("Item: 0"))[0];
			if (App.Query(c => c.Marked("Item: 2")).Length == 0)
			{
				var collectionViewFrame = App.Query(q => q.Marked(_collectionViewId))[0].Rect;
				App.ScrollForElement($"* marked:'Item: 2'", new Drag(collectionViewFrame, isVertical ? Drag.Direction.BottomToTop : Drag.Direction.RightToLeft, Drag.DragLength.Long), 50);
			}
			var element2 = App.Query(c => c.Marked("Item: 2"))[0];

			if (isVertical)
			{
				if (isList)
				{
					Assert.AreEqual(element1.Rect.X, element2.Rect.X);
					Assert.Greater(element2.Rect.Y, element1.Rect.Y);
				}
				else
				{
					var element3 = App.Query(c => c.Marked("Item: 3"))[0];
					Assert.AreEqual(element2.Rect.Y, element1.Rect.Y);
					Assert.Greater(element3.Rect.Y, element1.Rect.Y);
					Assert.AreEqual(element3.Rect.X, element1.Rect.X);
				}
			}
			else
			{
				if (isList)
				{
					Assert.AreEqual(element1.Rect.Y, element2.Rect.Y);
					Assert.Greater(element2.Rect.X, element1.Rect.X);
				}
				else
				{
					var element3 = App.Query(c => c.Marked("Item: 3"))[0];
					Assert.AreEqual(element2.Rect.X, element1.Rect.X);
					Assert.Greater(element3.Rect.X, element1.Rect.X);
					Assert.AreEqual(element3.Rect.Y, element1.Rect.Y);
				}
			}
		}
	}
}