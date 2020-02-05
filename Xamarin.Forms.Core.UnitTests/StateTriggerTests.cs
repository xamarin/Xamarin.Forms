using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class StateTriggerTests
	{
		const string RedStateName = "Red";
		const string GreenStateName = "Green";

		static readonly Entry TestEntry = new Entry();

		static VisualStateGroupList CreateTestStateGroups()
		{
			var stateGroups = new VisualStateGroupList();
			var visualStateGroup = new VisualStateGroup();

			var greenStateTrigger = new CompareStateTrigger { Property = TestEntry.Text, Value = "Test" };
			var greenState = new VisualState { Name = GreenStateName };
			greenState.StateTriggers.Add(greenStateTrigger);

			var redStateTrigger = new CompareStateTrigger { Property = TestEntry.Text, Value = string.Empty };
			var redState = new VisualState { Name = RedStateName };
			redState.StateTriggers.Add(redStateTrigger);

			visualStateGroup.States.Add(greenState);
			visualStateGroup.States.Add(redState);

			stateGroups.Add(visualStateGroup);

			return stateGroups;
		}

		[Test]
		public void StateTriggerDefaultVisualState()
		{
			var grid = new Grid();
			
			TestEntry.Text = string.Empty;

			grid.Children.Add(TestEntry);

			VisualStateManager.SetVisualStateGroups(grid, CreateTestStateGroups());

			var groups = VisualStateManager.GetVisualStateGroups(grid);

			Assert.That(groups[0].CurrentState.Name, Is.EqualTo(RedStateName));
		}

		[Test]
		public void StateTriggerChangedVisualState()
		{
			var grid = new Grid();

			TestEntry.Text = "Test";

			grid.Children.Add(TestEntry);

			VisualStateManager.SetVisualStateGroups(grid, CreateTestStateGroups());

			var groups = VisualStateManager.GetVisualStateGroups(grid);

			Assert.That(groups[0].CurrentState.Name, Is.EqualTo(GreenStateName));
		}
	}
}
