using System.Collections.Generic;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class TouchGestureRecognizerTests : BaseTestFixture
	{
		static VisualStateGroupList CreateTestStateGroups(bool includeTouch)
		{
			var stateGroups = new VisualStateGroupList();

			var visualStateGroup = new VisualStateGroup { Name = "CommonStates" };
			var normalState = new VisualState { Name = VisualStateManager.CommonStates.Normal };
			var focusedState = new VisualState { Name = VisualStateManager.CommonStates.Focused };
			var disabledState = new VisualState { Name = VisualStateManager.CommonStates.Disabled };
			visualStateGroup.States.Add(normalState);
			visualStateGroup.States.Add(focusedState);
			visualStateGroup.States.Add(disabledState);
			stateGroups.Add(visualStateGroup);

			if (includeTouch)
			{
				var touchVisualStateGroup = new VisualStateGroup { Name = "TouchStates" };
				var defaultState = new VisualState { Name = VisualStateManager.TouchStates.Press };
				var pressState = new VisualState { Name = VisualStateManager.TouchStates.Default };
				touchVisualStateGroup.States.Add(defaultState);
				touchVisualStateGroup.States.Add(pressState);
				stateGroups.Add(touchVisualStateGroup);
			}

			return stateGroups;
		}

		[Test]
		public void Cancel()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Cancel += (s, e) => eventCount++;
			recognizer.CancelCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Cancel, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Cancel, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void Change()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Change += (s, e) => eventCount++;
			recognizer.ChangeCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Change, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Change, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void Enter()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Enter += (s, e) => eventCount++;
			recognizer.EnterCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Enter, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Enter, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void Exit()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Exit += (s, e) => eventCount++;
			recognizer.ExitCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Exit, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Exit, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void EnterInView()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Enter += (s, e) => eventCount++;
			recognizer.EnterCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Move, false) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Move, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void ExitFromView()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Exit += (s, e) => eventCount++;
			recognizer.ExitCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Move, true) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Move, false) }));
			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Move, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void Fail()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Fail += (s, e) => eventCount++;
			recognizer.FailCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Fail, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Fail, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void Hover()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Hover += (s, e) => eventCount++;
			recognizer.HoverCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Hover, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Hover, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void Move()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Move += (s, e) => eventCount++;
			recognizer.MoveCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Move, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Move, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void Press()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Press += (s, e) => eventCount++;
			recognizer.PressCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void Release()
		{
			var eventCount = 0;
			var commandCount = 0;
			var recognizer = new TouchGestureRecognizer();
			recognizer.Release += (s, e) => eventCount++;
			recognizer.ReleaseCommand = new Command(() => commandCount++);
			var view = new View();

			recognizer.SendTouch(view,
				new TouchEventArgs(1, TouchState.Release, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Release, true) }));

			Assert.AreEqual(1, eventCount);
			Assert.AreEqual(1, commandCount);
		}

		[Test]
		public void VsmEmptyGroup()
		{
			var label = new Label();
			VisualStateManager.SetVisualStateGroups(label, CreateTestStateGroups(false));
			IList<VisualStateGroup> group = VisualStateManager.GetVisualStateGroups(label);

			var recognizer = new TouchGestureRecognizer();
			label.GestureRecognizers.Add(recognizer);

			Assert.That(group[0].CurrentState.Name, Is.EqualTo(VisualStateManager.CommonStates.Normal));

			recognizer.SendTouch(label,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			Assert.That(group[0].CurrentState.Name, Is.EqualTo(VisualStateManager.CommonStates.Normal));
			Assert.That(group.Count, Is.EqualTo(1));
		}

		[Test]
		public void VsmWithGroup()
		{
			var label = new Label();
			VisualStateManager.SetVisualStateGroups(label, CreateTestStateGroups(true));
			IList<VisualStateGroup> group = VisualStateManager.GetVisualStateGroups(label);

			var recognizer = new TouchGestureRecognizer();
			label.GestureRecognizers.Add(recognizer);

			Assert.That(group[0].CurrentState.Name, Is.EqualTo(VisualStateManager.CommonStates.Normal));
			Assert.That(group[1].CurrentState, Is.EqualTo(null));

			recognizer.SendTouch(label,
				new TouchEventArgs(1, TouchState.Press, new List<TouchPoint> { new TouchPoint(0, new Point(50, 50), TouchState.Press, true) }));

			Assert.That(group[0].CurrentState.Name, Is.EqualTo(VisualStateManager.CommonStates.Normal));
			Assert.That(group[1].CurrentState.Name, Is.EqualTo(VisualStateManager.TouchStates.Press));
		}
	}
}