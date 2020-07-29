using System;
using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Forms
{
	public static class RadioButtonGroup
	{
		internal const string RadioButtonGroupSelectionChanged = "RadioButtonGroupSelectionChanged";

		internal static Dictionary<string, List<WeakReference<RadioButton>>> GroupNameToElements;

		static readonly BindableProperty RadioButtonGroupControllerProperty =
			BindableProperty.CreateAttached("RadioButtonGroupController", typeof(RadioButtonGroupController), typeof(Layout<View>), default(RadioButtonGroupController),
			defaultValueCreator: (b) => new RadioButtonGroupController((Layout<View>)b),
			propertyChanged: (b, o, n) => OnControllerChanged(b, (RadioButtonGroupController)o, (RadioButtonGroupController)n));

		static RadioButtonGroupController GetRadioButtonGroupController(BindableObject b)
		{
			return (RadioButtonGroupController)b.GetValue(RadioButtonGroupControllerProperty);
		}

		public static readonly BindableProperty GroupNameProperty =
			BindableProperty.Create("GroupName", typeof(string), typeof(Layout<View>), null, 
			propertyChanged: (b, o, n) => { GetRadioButtonGroupController(b).GroupName = (string)n; });

		public static string GetGroupName(BindableObject b)
		{
			return (string)b.GetValue(GroupNameProperty);
		}

		public static readonly BindableProperty SelectionProperty =
			BindableProperty.Create("Selection", typeof(RadioButton), typeof(Layout<View>), null, 
			defaultBindingMode: BindingMode.TwoWay,
			propertyChanged: (b, o, n) => { GetRadioButtonGroupController(b).Selection = (RadioButton)n; });

		public static RadioButton GetSelection(BindableObject b)
		{
			return (RadioButton)b.GetValue(SelectionProperty);
		}

		internal static void UpdateRadioButtonGroup(RadioButton radioButton)
		{
			string groupName = radioButton.GroupName;

			Element scope;

			if (!string.IsNullOrEmpty(groupName))
			{
				Element rootScope = GetVisualRoot(radioButton);
				scope = rootScope;

				if (GroupNameToElements == null)
					GroupNameToElements = new Dictionary<string, List<WeakReference<RadioButton>>>(1);

				// Get all elements bound to this key and remove this element
				List<WeakReference<RadioButton>> elements = GroupNameToElements[groupName];
				for (int i = 0; i < elements.Count;)
				{
					WeakReference<RadioButton> weakRef = elements[i];
					if (weakRef.TryGetTarget(out RadioButton rb))
					{
						// Uncheck all checked RadioButtons different from the current one
						if (rb != radioButton && (rb.IsChecked == true) && rootScope == GetVisualRoot(rb))
							rb.SetValueFromRenderer(RadioButton.IsCheckedProperty, false);

						i++;
					}
					else
					{
						// Remove dead instances
						elements.RemoveAt(i);
					}
				}
			}
			else // Logical parent should be the group
			{
				Element parent = radioButton.Parent;
				scope = parent;

				if (parent != null)
				{
					// Traverse logical children
					IEnumerable children = parent.LogicalChildren;
					IEnumerator itor = children.GetEnumerator();
					while (itor.MoveNext())
					{
						var rb = itor.Current as RadioButton;
						if (rb != null && rb != radioButton && string.IsNullOrEmpty(rb.GroupName) && (rb.IsChecked == true))
							rb.SetValueFromRenderer(RadioButton.IsCheckedProperty, false);
					}
				}
			}

			MessagingCenter.Send(radioButton, RadioButtonGroupSelectionChanged, new RadioButtonGroupSelectionChanged(scope));
		}

		internal static void Register(RadioButton radioButton, string groupName)
		{
			if (GroupNameToElements == null)
				GroupNameToElements = new Dictionary<string, List<WeakReference<RadioButton>>>(1);

			if (GroupNameToElements.TryGetValue(groupName, out List<WeakReference<RadioButton>> elements))
			{
				// There were some elements there, remove dead ones
				PurgeDead(elements, null);
			}
			else
			{
				elements = new List<WeakReference<RadioButton>>(1);
				GroupNameToElements[groupName] = elements;
			}

			elements.Add(new WeakReference<RadioButton>(radioButton));
		}

		internal static void Unregister(RadioButton radioButton, string groupName)
		{
			if (GroupNameToElements == null)
				return;

			// Get all elements bound to this key and remove this element
			if (GroupNameToElements.TryGetValue(groupName, out List<WeakReference<RadioButton>> elements))
			{
				PurgeDead(elements, radioButton);

				if (elements.Count == 0)
					GroupNameToElements.Remove(groupName);
			}
		}

		static void OnControllerChanged(BindableObject b, RadioButtonGroupController oldC, 
			RadioButtonGroupController newC)
		{
			if (newC == null)
			{
				return;
			}

			newC.GroupName = GetGroupName(b);
			newC.Selection = GetSelection(b);
		}

		internal static Element GetVisualRoot(Element element)
		{
			Element parent = element.Parent;
			while (parent != null && !(parent is Page))
				parent = parent.Parent;
			return parent;
		}

		static void PurgeDead(List<WeakReference<RadioButton>> elements, object elementToRemove)
		{
			for (int i = 0; i < elements.Count;)
			{
				if (elements[i].TryGetTarget(out RadioButton rb) && rb == elementToRemove)
					elements.RemoveAt(i);
				else
					i++;
			}
		}
	}
}