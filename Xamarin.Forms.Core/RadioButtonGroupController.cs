using System;

namespace Xamarin.Forms
{
	internal class RadioButtonGroupController
	{
		readonly Layout<View> _layout; // TODO ezhart Move this to weak reference and use weakeventmanager for ChildAdded
		string _groupName;
		private RadioButton _selection;

		public string GroupName { get => _groupName; set => SetGroupName(value); }
		public RadioButton Selection { get => _selection; set => SetSelection(value); }

		public RadioButtonGroupController(Layout<View> layout)
		{
			if (layout is null)
			{
				throw new ArgumentNullException(nameof(layout));
			}

			_layout = layout;
			_layout.ChildAdded += ChildAdded;

			if (!string.IsNullOrEmpty(_groupName))
			{
				UpdateGroupNames(_layout, _groupName);
			}

			MessagingCenter.Subscribe<RadioButton, RadioButtonGroupSelectionChanged>(this, RadioButtonGroup.RadioButtonGroupSelectionChanged, HandleRadioButtonGroupSelectionChanged);
		}

		void HandleRadioButtonGroupSelectionChanged(RadioButton selected, RadioButtonGroupSelectionChanged args)
		{
			if (selected.GroupName != _groupName)
			{
				return;
			}

			var controllerScope = RadioButtonGroup.GetVisualRoot(_layout);
			if (args.Scope != controllerScope)
			{
				return;
			}

			_layout.SetValue(RadioButtonGroup.SelectionProperty, selected);
		}

		void ChildAdded(object sender, ElementEventArgs e)
		{
			if (string.IsNullOrEmpty(_groupName))
			{
				return;
			}

			UpdateGroupName(e.Element, _groupName);
		}

		void UpdateGroupName(Element element, string name, string oldName = null) 
		{
			if (element is RadioButton radioButton)
			{
				var currentName = radioButton.GroupName;

				if (string.IsNullOrEmpty(currentName) || currentName == oldName)
				{
					radioButton.GroupName = name;
				}
			}
		}

		void UpdateGroupNames(Layout<View> layout, string name, string oldName = null) 
		{
			foreach (var descendant in layout.Descendants())
			{
				UpdateGroupName(descendant, name, oldName);
			}
		}

		void SetSelection(RadioButton radioButton)
		{
			_selection = radioButton;
			RadioButtonGroup.UpdateRadioButtonGroup(radioButton);
		}

		void SetGroupName(string groupName)
		{
			var oldName = _groupName;
			_groupName = groupName;
			UpdateGroupNames(_layout, _groupName, oldName);
		}
	}

	internal class RadioButtonGroupSelectionChanged
	{ 
		public Element Scope { get; }

		public RadioButtonGroupSelectionChanged(Element scope) => Scope = scope;
	}
}