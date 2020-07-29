using System;

namespace Xamarin.Forms
{
	internal class RadioButtonGroupController
	{
		readonly Layout<View> _layout;

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
			layout.ChildAdded += ChildAdded;

			if (!string.IsNullOrEmpty(_groupName))
			{
				UpdateGroupNames(layout, _groupName);
			}

			MessagingCenter.Subscribe<RadioButton, RadioButtonGroupSelectionChanged>(this, 
				RadioButtonGroup.RadioButtonGroupSelectionChanged, HandleRadioButtonGroupSelectionChanged);
			MessagingCenter.Subscribe<RadioButton, RadioButtonGroupNameChanged>(this, RadioButtonGroup.RadioButtonGroupNameChanged,
				HandleRadioButtonGroupNameChanged);
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

		void HandleRadioButtonGroupNameChanged(RadioButton radioButton, RadioButtonGroupNameChanged args) 
		{
			if (args.OldName != _groupName)
			{
				return;
			}

			var controllerScope = RadioButtonGroup.GetVisualRoot(_layout);
			if (args.Scope != controllerScope)
			{
				return;
			}

			_layout.ClearValue(RadioButtonGroup.SelectionProperty);
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
			if (!(element is RadioButton radioButton))
			{
				return;
			}

			var currentName = radioButton.GroupName;

			if (string.IsNullOrEmpty(currentName) || currentName == oldName)
			{
				radioButton.GroupName = name;
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

			if (radioButton != null)
			{
				RadioButtonGroup.UpdateRadioButtonGroup(radioButton);
			}
		}

		void SetGroupName(string groupName)
		{
			var oldName = _groupName;
			_groupName = groupName;
			UpdateGroupNames(_layout, _groupName, oldName);
		}
	}
}