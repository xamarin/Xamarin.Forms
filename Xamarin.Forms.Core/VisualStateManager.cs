using System;
//using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	//[ContentProperty("VisualStateGroups")]
	public static class VisualStateManager
	{
		public static readonly BindableProperty VisualStateGroupsProperty =
			BindableProperty.CreateAttached("VisualStateGroups", typeof(Collection<VisualStateGroup>), typeof(VisualElement), null);

		//public static readonly BindableProperty VisualStateGroupsProperty =
		//	BindableProperty.CreateAttached("VisualStateGroups", typeof(Collection<VisualStateGroup>), typeof(VisualElement), new Collection<VisualStateGroup>());

		public static Collection<VisualStateGroup> GetVisualStateGroups(VisualElement visualElement)
		{
			return (Collection<VisualStateGroup>)visualElement.GetValue(VisualStateGroupsProperty);
		}

		public static void SetVisualStateGroups(VisualElement visualElement, Collection<VisualStateGroup> value)
		{
			visualElement.SetValue(VisualStateGroupsProperty, value);
		}

		public static bool GoToState(VisualElement visualElement, string name)
		{
			var groups = visualElement.GetValue(VisualStateGroupsProperty) as Collection<VisualStateGroup>;

			if (groups == null)
			{
				return false;
			}

			// TODO hartez 2017/02/08 17:53:46 Figure out what UWP does about duplicate state names inside of groups and between groups	
			// Right now, if there are duplicates we don't throw any exceptions, and the first one found will always win
			// I think in UWP since everything comes from XAML x:Name is doing the enforcement; if we're going to allow creation
			// of VSM stuff from code (and we are), we need to enforce names on our own

			foreach (VisualStateGroup group in groups)
			{
				if (group.CurrentState?.Name == name)
				{
					// We're already in the target state; nothing else to do
					return true;
				}

				// See if this group contains the new state
				var target = group.GetState(name);
				if (target == null)
				{
					continue;
				}

				// If we've got a new state to transition to, unapply the setters from the current state
				if (group.CurrentState != null)
				{
					foreach (Setter setter in group.CurrentState.Setters)
					{
						setter.UnApply(visualElement);
					}
				}

				// Update the current state
				group.CurrentState = target;

				// Apply the setters from the new state
				foreach (Setter setter in target.Setters)
				{
					setter.Apply(visualElement);
				}

				return true;
			}

			return false;
		}
	}

	[RuntimeNameProperty("Name")]
	[ContentProperty("States")]
	public class VisualStateGroup
	{
		public VisualStateGroup()
		{
			States = new Collection<VisualState>();
		}

		public Type TargetType { get; set; }
		public string Name { get; set; }
		public Collection<VisualState> States { get; }
		public VisualState CurrentState { get; internal set; }

		internal VisualState GetState(string name)
		{
			foreach (VisualState state in States)
			{
				if (state.Name == name)
				{
					return state;
				}
			}

			return null;
		}
	}

	[RuntimeNameProperty("Name")]
	public class VisualState
	{
		public VisualState()
		{
			Setters = new Collection<Setter>();
		}

		public string Name { get; set; }
		public Collection<Setter> Setters { get; set; }
		public Type TargetType { get; internal set; }
	}

}
