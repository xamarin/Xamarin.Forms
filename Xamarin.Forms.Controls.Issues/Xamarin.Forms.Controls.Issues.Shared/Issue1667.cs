using System;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Github, 1667, "Entry: Position and color of caret", PlatformAffected.All)]
	public class Issue1667 : TestContentPage
	{
		Entry _entry;
		Entry _cursorStartPosition;
		Entry _selectionLength;
		Button _updateButton;
		Button _readButton;

		protected override void Init ()
		{
			_entry = new Entry {Text = "Enter cursor position below"};
			_entry.PropertyChanged += ReadCursor;

			_cursorStartPosition = new Entry();
			_selectionLength = new Entry();

			_updateButton = new Button { Text = "Update" };
			_updateButton.Clicked += UpdateCursor;

			Content = new StackLayout
			{
				Margin = new Thickness(10, 40),
				Children =
				{
					_entry,
					new Label {Text = "Start:"},
					_cursorStartPosition,
					new Label {Text = "Selection Length:"},
					_selectionLength,
					_updateButton
				}
			};
		}

		void UpdateCursor(object sender, EventArgs args)
		{
			var start = 0;
			var length = 0;
			if (int.TryParse(_cursorStartPosition.Text, out start))
			{
				_entry.CursorPosition = start;
			}
			if (int.TryParse(_selectionLength.Text, out length))
			{
				_entry.SelectionLength = length;
			}
		}

		void ReadCursor(object sender, EventArgs args)
		{
			_cursorStartPosition.Text = _entry.CursorPosition.ToString();
			_selectionLength.Text = _entry.SelectionLength.ToString();
		}
	}
}
