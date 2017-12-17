using System;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	public static class VisualStateManager
	{
		internal class CommonStates
		{
			public const string Normal = "Normal";
			public const string Disabled = "Disabled";
			public const string Focused = "Focused";
		}

		public static readonly BindableProperty VisualStateGroupsProperty =
			BindableProperty.CreateAttached("VisualStateGroups", typeof(IList<VisualStateGroup>), typeof(VisualElement), 
				defaultValue: null, propertyChanged: VisualStateGroupsPropertyChanged, 
				defaultValueCreator: bindable => new List<VisualStateGroup>());

		static void VisualStateGroupsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			GoToState((VisualElement)bindable, CommonStates.Normal);
		}

		public static IList<VisualStateGroup> GetVisualStateGroups(VisualElement visualElement)
		{
			return (IList<VisualStateGroup>)visualElement.GetValue(VisualStateGroupsProperty);
		}

		public static void SetVisualStateGroups(VisualElement visualElement, IList<VisualStateGroup> value)
		{
			visualElement.SetValue(VisualStateGroupsProperty, value);
		}

		public static bool GoToState(VisualElement visualElement, string name)
		{
			if (!(visualElement.GetValue(VisualStateGroupsProperty) is IList<VisualStateGroup> groups))
			{
				return false;
			}

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

		public static bool HasVisualStateGroups(this VisualElement element)
		{
			return !element.GetIsDefault(VisualStateGroupsProperty);
		}
	}

	[RuntimeNameProperty(nameof(Name))]
	[ContentProperty(nameof(States))]
	public sealed class VisualStateGroup 
	{
		public VisualStateGroup()
		{
			States = new List<VisualState>();
		}

		public Type TargetType { get; set; }
		public string Name { get; set; }
		public IList<VisualState> States { get; }
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

		internal VisualStateGroup Clone()
		{
			var clone =  new VisualStateGroup {TargetType = TargetType, Name = Name, CurrentState = CurrentState};
			foreach (VisualState state in States)
			{
				clone.States.Add(state.Clone());
			}

			return clone;
		}
	}

	[RuntimeNameProperty(nameof(Name))]
	public sealed class VisualState 
	{
		public VisualState()
		{
			Setters = new List<Setter>();
		}

		public string Name { get; set; }
		public IList<Setter> Setters { get;}
		public Type TargetType { get; set; }

		internal VisualState Clone()
		{
			var clone = new VisualState { Name = Name, TargetType = TargetType };
			foreach (var setter in Setters)
			{
				clone.Setters.Add(setter);
			}

			return clone;
		}
	}

	internal static class VisualStateGroupListExtensions
	{
		internal static IList<VisualStateGroup> Clone(this IList<VisualStateGroup> groups)
		{
			var actual = new List<VisualStateGroup>();
			foreach (var group in groups)
			{
				actual.Add(group.Clone());
			}

			return actual;
		}
	}
}
