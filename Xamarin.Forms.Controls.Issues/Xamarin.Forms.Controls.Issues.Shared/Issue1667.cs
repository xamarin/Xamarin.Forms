using System;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using NUnit.Framework;
#endif

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
			_entry = new Entry {Text = "Enter cursor position below", AutomationId = "TextField"};
			_entry.PropertyChanged += ReadCursor;

			_cursorStartPosition = new Entry();
			_selectionLength = new Entry();

			_updateButton = new Button { Text = "Update" };
			_updateButton.Clicked += UpdateCursor;

			var red = new Button { Text = "Red", TextColor = Color.Red};
			red.Clicked += (sender, e) => _entry.CursorColor = Color.Red;

			var blue = new Button { Text = "Blue", TextColor = Color.Blue};
			blue.Clicked += (sender, e) => _entry.CursorColor = Color.Blue;

			var defaultColor = new Button { Text = "Default" };
			defaultColor.Clicked += (sender, e) => _entry.CursorColor = Color.Default;
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
					_updateButton,
					red,
					blue,
					defaultColor
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

		#if UITEST
		[Test]
		public void Test()
		{
			RunningApp.WaitForElement(q => q.Marked("TextField"));
			_entry.CursorPosition = 2;
			_entry.SelectionLength = 3;
			RunningApp.Screenshot("Text selection from char 2 length 3.");
			Assert.AreEqual("2", _cursorStartPosition.Text);
			Assert.AreEqual("3", _selectionLength.Text);

			RunningApp.Tap(q => q.Marked("Textfield"));

			Assert.AreEqual(_entry.Text.Length, _entry.CursorPosition);
			Assert.AreEqual(0, _entry.SelectionLength);


		}
		#endif
	}
}
