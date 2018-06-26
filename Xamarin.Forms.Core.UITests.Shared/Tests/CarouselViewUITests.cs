using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.UITest;

namespace Xamarin.Forms.Core.UITests
{
	[TestFixture]
	[Category(UITestCategories.CarouselView)]
	internal class CarouselViewUITests : BaseTestFixture
	{
		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.CarouselViewGallery);
		}

		[Test]
		public void TestShowCarouselView()
		{
			App.Tap(c => c.Marked("Load"));
		}

		internal const int InitialItems = 4;
		internal const int InitialPosition = 1;
		internal const string OnItemSelectedAbbr = "i";
		internal const string OnPositionSelectedAbbr = "p";
		internal const string Null = "null";
		internal const int EventQueueDepth = 7;

		private const double SwipePercentage = 0.75;
		private const int SwipeSpeed = 2000;

		static class Id
		{
			internal const string Name = "CarouselView Gallery";
			internal static string ItemId = nameof(ItemId);
			internal static string EventLog = nameof(EventLog);
			internal static string SelectedItem = nameof(SelectedItem);
			internal static string Position = nameof(Position);
			internal static string SelectedPosition = nameof(SelectedPosition);
			internal static string Next = nameof(Next);
			internal static string Previous = nameof(Previous);
			internal static string First = nameof(First);
			internal static string Last = nameof(Last);
			internal static string Load = nameof(Load);
			internal static string Load0 = nameof(Load0);
			internal static string Clear = nameof(Clear);
			internal static string Launch = nameof(Launch);
			internal static string Pop = nameof(Pop);

			internal static string RemoveLeft = nameof(RemoveLeft);
			internal static string RemoveRight = nameof(RemoveRight);
			internal static string Remove = nameof(Remove);

			internal static string AddLeft = nameof(AddLeft);
			internal static string AddRight = nameof(AddRight);
			internal static string Change = nameof(Change);
		}
		enum Event
		{
			OnItemSelected,
			OnPositionSelected
		}

		IApp _app => App;
		List<int> _itemIds;
		int _currentPosition;
		int? _currentItem;
		Queue<string> _expectedEvents;
		int _eventId;
		bool _loaded;
		int _start;

		public CarouselViewUITests()
		{
			_itemIds = new List<int>();
			_currentPosition = InitialPosition;
			_expectedEvents = new Queue<string>();
			_eventId = 0;
			_loaded = false;
		}

		private void WaitForValue(string marked, object value)
		{
			if (value == null)
				value = "null";

			var query = $"* marked:'{marked}' text:'{value}'";
			_app.WaitForElement(o => o.Raw(query));
		}
		private void WaitForNotValue(string marked, object value)
		{
			var query = $"* marked:'{marked}' text:'{value}'";
			_app.WaitForNoElement(o => o.Raw(query));
		}
		private void WaitForPosition(int expectedPosition)
		{
			if (!_loaded)
			{
				WaitForValue(Id.Position, expectedPosition);
				_currentPosition = expectedPosition;
				return;
			}

			var expectedItem = _itemIds[expectedPosition];

			// expect no movement
			if (_currentItem == expectedItem)
				Thread.Sleep(TimeSpan.FromMilliseconds(500));

			// wait for for expected item and corresponding event
			WaitForValue(Id.ItemId, expectedItem);
			WaitForValue(Id.SelectedItem, expectedItem);
			_currentItem = expectedItem;

			// wait for for expected position and corresponding event
			WaitForValue(Id.Position, expectedPosition);
			WaitForValue(Id.SelectedPosition, expectedPosition);
			_currentPosition = expectedPosition;

			VerifyEvents();
		}
		private void VerifyEvents()
		{
			// check expected events
			if (_expectedEvents.Count == 0)
				return;
			var expectedEvents = string.Join(", ", _expectedEvents.ToArray().Reverse());
			WaitForValue(Id.EventLog, expectedEvents);
		}
		private void ExpectMovementEvents(int expectedPosition)
		{
			if (expectedPosition == _currentPosition)
				return;

			ExpectEvent(Event.OnPositionSelected);

			if (_loaded)
				ExpectEvent(Event.OnItemSelected);
		}
		private void ExpectEvent(Event e)
		{
			if (e == Event.OnItemSelected)
				_expectedEvents.Enqueue($"{OnItemSelectedAbbr}/{_eventId++}");

			if (e == Event.OnPositionSelected)
				_expectedEvents.Enqueue($"{OnPositionSelectedAbbr}/{_eventId++}");

			if (_expectedEvents.Count == EventQueueDepth)
				_expectedEvents.Dequeue();
		}
		private void Tap(string buttonText, int expectedPosition)
		{
			// tap
			_app.Tap(buttonText);

			// anticipate events
			ExpectMovementEvents(expectedPosition);

			// wait
			WaitForPosition(expectedPosition);
		}
		private void Swipe(bool next, int expectedPosition)
		{
			// swipe
			if (next)
				_app.SwipeRightToLeft(swipePercentage: SwipePercentage/*, swipeSpeed: SwipeSpeed*/);
			else
				_app.SwipeLeftToRight(swipePercentage: SwipePercentage/*, swipeSpeed: SwipeSpeed*/);

			// handle swipe past first
			if (expectedPosition == -1 && _currentPosition == 0)
				expectedPosition = 0;

			// handle swipe past last
			else if (expectedPosition == Count && _currentPosition == Count - 1)
				expectedPosition = Count - 1;

			// anticipate events
			ExpectMovementEvents(expectedPosition);

			// wait
			WaitForPosition(expectedPosition);
		}
		private void Move(int steps, bool swipe)
		{
			Action next = swipe ? (Action)SwipeNext : StepNext;
			Action previous = swipe ? (Action)SwipePrevious : StepPrevious;

			var action = next;
			if (steps < 0)
			{
				action = previous;
				steps = -steps;
			}

			for (int i = 0; i < steps; i++)
				action();
		}
		private void MoveToPosition(int position, bool swipe)
		{
			Assert.True(position >= 0 && position < Count);
			Move(position - _currentPosition, swipe);
		}
		private void MoveToItem(int targetPage, bool swipe)
		{
			MoveToPosition(_itemIds.IndexOf(targetPage), swipe);
		}
		public void MoveToFirst(bool swipe) => MoveToPosition(0, swipe);
		public void MoveToLast(bool swipe) => MoveToPosition(Count - 1, swipe);

		public int ItemId => int.Parse(_app.Query(Id.ItemId)[0].Text);

		public string Name => Id.Name;
		public int Count => _itemIds.Count;

		public void First() => Tap(Id.First, 0);
		public void Last() => Tap(Id.Last, _itemIds.Count - 1);

		public void StepNext() => Tap(Id.Next, _currentPosition + 1);
		public void StepPrevious() => Tap(Id.Previous, _currentPosition - 1);
		public void Step(int steps) => Move(steps, swipe: false);
		public void StepToPosition(int position) => MoveToPosition(position, swipe: false);
		public void StepToItem(int item) => MoveToItem(item, swipe: false);
		public void StepToFirst() => MoveToFirst(swipe: false);
		public void StepToLast() => MoveToLast(swipe: false);

		public void SwipeNext() => Swipe(next: true, expectedPosition: _currentPosition + 1);
		public void SwipePrevious() => Swipe(next: false, expectedPosition: _currentPosition - 1);
		public void Swipe(int swipes) => Move(swipes, swipe: true);
		public void SwipeToPosition(int position) => MoveToPosition(position, swipe: true);
		public void SwipeToItem(int item) => MoveToItem(item, swipe: true);
		public void SwipeToFirst() => MoveToFirst(swipe: true);
		public void SwipeToLast() => MoveToLast(swipe: true);

		public void RemoveAllLeft()
		{
			while (_currentPosition > 0)
				RemoveLeft();
		}
		public void RemoveAllRight()
		{
			while (_currentPosition < _itemIds.Count - 1)
				RemoveRight();
		}
		public void RemoveAll()
		{
			while (_itemIds.Count > 0)
				Remove();
		}
		public void RemoveLeft()
		{
			_app.Tap(Id.RemoveLeft);
			_itemIds.RemoveAt(_currentPosition - 1);
			_currentPosition--;
			ExpectEvent(Event.OnPositionSelected);
			WaitForPosition(_currentPosition);
		}
		public void RemoveRight()
		{
			_app.Tap(Id.RemoveRight);
			_itemIds.RemoveAt(_currentPosition + 1);
			WaitForPosition(_currentPosition);
		}
		public void Remove()
		{
			_app.Tap(Id.Remove);
			_itemIds.RemoveAt(_currentPosition);
			if (_itemIds.Count == _currentPosition)
			{
				if (_currentPosition == 0)
				{
					// removed last element
					WaitForValue(Id.SelectedItem, null);
					WaitForValue(Id.SelectedPosition, _currentPosition);
					_currentItem = null;
					ExpectEvent(Event.OnItemSelected);
					VerifyEvents();
					return;
				}

				// removed tail element
				_currentPosition--;
				ExpectEvent(Event.OnPositionSelected);
			}
			ExpectEvent(Event.OnItemSelected);
			WaitForPosition(_currentPosition);
		}

		public void Load0()
		{
			_app.Tap(Id.Load0);
			_loaded = true;
			_itemIds = Enumerable.Range(_start, 0).ToList();

			if (_currentPosition != 0)
				ExpectEvent(Event.OnPositionSelected);
			_currentPosition = 0;

			if (_currentItem != null)
				ExpectEvent(Event.OnItemSelected);
			_currentItem = null;

			WaitForValue(Id.Position, _currentPosition);
			WaitForValue(Id.SelectedItem, _currentItem);

			VerifyEvents();
		}
		public void Load()
		{
			_app.Tap(Id.Load);
			_loaded = true;
			_itemIds = Enumerable.Range(_start, InitialItems).ToList();
			_start += InitialItems;

			if (_currentPosition >= InitialItems)
			{
				ExpectEvent(Event.OnPositionSelected);
				_currentPosition = InitialItems - 1;
			}

			var currentItem = _itemIds[_currentPosition];
			if (_currentItem != null && _currentItem != currentItem)
				ExpectEvent(Event.OnItemSelected);
			_currentItem = currentItem;

			WaitForValue(Id.Position, _currentPosition);
			WaitForValue(Id.ItemId, _currentItem);
			WaitForValue(Id.SelectedItem, _currentItem);

			VerifyEvents();
		}
		public void Clear()
		{
			_app.Tap(Id.Clear);
			_loaded = false;
			_itemIds = new List<int>();

			WaitForValue(Id.SelectedItem, Null);
			WaitForValue(Id.Position, _currentPosition);

			ExpectEvent(Event.OnItemSelected);
			VerifyEvents();
		}

		public void Launch()
		{
			_currentPosition = InitialPosition;
			Tap(Id.Launch, _currentPosition);
		}
		public void Pop()
		{
			_app.Back();
			_loaded = false;
			_itemIds = new List<int>();
			_expectedEvents.Clear();
			_eventId = 0;

			WaitForValue(Id.Launch, Id.Launch);
		}


		[Test]
		public void SwipeStepJump()
		{

			try
			{
				var carousel = this;

				// start at something other than 0
				Assert.AreNotEqual(0, InitialPosition);

				// remove all
				//carousel.Launch();

				carousel.Load();
				carousel.StepNext();
				carousel.RemoveAll();

				carousel.Load();
				carousel.Last();
				carousel.RemoveAllLeft();

				carousel.Load();
				carousel.First();
				carousel.RemoveAllRight();
				carousel.Pop();

				// remove
				carousel.Launch();
				carousel.Load();
				carousel.RemoveLeft();
				carousel.StepNext();

				carousel.RemoveRight();
				carousel.StepPrevious();
				carousel.StepNext();

				carousel.Remove(); // remove rightmost
				carousel.Pop();

				// clear with Empty ItemsSource
				carousel.Launch();
				carousel.Load();
				carousel.Load0();

				// set position to ItemsSource.Count - 1
				carousel.Load();
				carousel.Last();
				carousel.Clear();
				carousel.StepNext();
				carousel.Load();
				carousel.Pop();

				// swipe before programmatic jump
				carousel.Launch();
				carousel.Load();
				carousel.SwipeNext();
				carousel.Pop();

				// position can be set even if ItemsSource is null
				carousel.Launch();
				carousel.StepNext();
				carousel.Load();
				carousel.Clear();
				carousel.StepPrevious();
				carousel.Load();

				App.SetOrientationPortrait();
				for (var i = 0; i < 2; i++)
				{
					// programmatic jump to first/last
					carousel.Last();
					carousel.First();
					carousel.StepToLast();
					carousel.StepToFirst();

					// swiping
					carousel.SwipeToLast();
					carousel.SwipeNext(); // test swipe past end
					carousel.SwipeToFirst();
					carousel.SwipePrevious(); // test swipe past start

					App.SetOrientationLandscape();
				}
				App.SetOrientationPortrait();

				App.Screenshot("End");
			}
			catch (Exception e)
			{
				App.Screenshot("End");
				throw e;
			}
		}
	}
}