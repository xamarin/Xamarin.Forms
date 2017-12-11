using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class VisualStateManagerTests
	{
		const string NormalStateName = "Normal";
		const string InvalidStateName = "Invalid";

		Collection<VisualStateGroup> CreateTestStateGroup()
		{
			var stateGroups = new Collection<VisualStateGroup>();
			var visualStateGroup = new VisualStateGroup { Name = "CommonStates" };
			var normalState = new VisualState { Name = NormalStateName };
			var invalidState = new VisualState { Name = InvalidStateName };

			visualStateGroup.States.Add(normalState);
			visualStateGroup.States.Add(invalidState);

			stateGroups.Add(visualStateGroup);

			return stateGroups;
		}

		Collection<VisualStateGroup> CreateStateGroupWithoutNormalState()
		{
			var stateGroups = new Collection<VisualStateGroup>();
			var visualStateGroup = new VisualStateGroup { Name = "CommonStates" };
			var invalidState = new VisualState { Name = InvalidStateName };

			visualStateGroup.States.Add(invalidState);

			stateGroups.Add(visualStateGroup);

			return stateGroups;
		}

		[Test]
		public void InitialStateIsNormalIfAvailable()
		{
			var label1 = new Label();
		
			VisualStateManager.SetVisualStateGroups(label1, CreateTestStateGroup());

			var groups1 = VisualStateManager.GetVisualStateGroups(label1);

			Assert.That(groups1[0].CurrentState.Name == NormalStateName);
		}

		[Test]
		public void InitialStateIsNullIfNormalNotAvailable()
		{
			var label1 = new Label();

			VisualStateManager.SetVisualStateGroups(label1, CreateStateGroupWithoutNormalState());

			var groups1 = VisualStateManager.GetVisualStateGroups(label1);

			Assert.Null(groups1[0].CurrentState);
		}

		[Test]
		public void VisualElementsStateGroupsAreDistinct()
		{
			var label1 = new Label();
			var label2 = new Label();

			VisualStateManager.SetVisualStateGroups(label1, CreateTestStateGroup());
			VisualStateManager.SetVisualStateGroups(label2, CreateTestStateGroup());

			var groups1 = VisualStateManager.GetVisualStateGroups(label1);
			var groups2 = VisualStateManager.GetVisualStateGroups(label2);

			Assert.AreNotSame(groups1, groups2);

			Assert.That(groups1[0].CurrentState.Name == NormalStateName);
			Assert.That(groups2[0].CurrentState.Name == NormalStateName);

			VisualStateManager.GoToState(label1, InvalidStateName);

			Assert.That(groups1[0].CurrentState.Name == InvalidStateName);
			Assert.That(groups2[0].CurrentState.Name == NormalStateName);
		}

		[Test]
		public void VisualStateGroupsFromSettersAreDistinct()
		{
			var x = new Setter();
			x.Property = VisualStateManager.VisualStateGroupsProperty;
			x.Value = CreateTestStateGroup();

			var label1 = new Label();
			var label2 = new Label();

			x.Apply(label1, true);
			x.Apply(label2, true);

			var groups1 = VisualStateManager.GetVisualStateGroups(label1);
			var groups2 = VisualStateManager.GetVisualStateGroups(label2);

			Assert.NotNull(groups1);
			Assert.NotNull(groups2);

			Assert.AreNotSame(groups1, groups2);

			Assert.That(groups1[0].CurrentState.Name == NormalStateName);
			Assert.That(groups2[0].CurrentState.Name == NormalStateName);

			VisualStateManager.GoToState(label1, InvalidStateName);

			Assert.That(groups1[0].CurrentState.Name == InvalidStateName);
			Assert.That(groups2[0].CurrentState.Name == NormalStateName);
		}
	}
}