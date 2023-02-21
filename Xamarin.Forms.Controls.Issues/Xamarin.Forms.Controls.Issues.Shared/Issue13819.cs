using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13819, "[Bug] Android: ClearButtonVisibility doesn't work correctly when enabled while the field is already focused", PlatformAffected.Android)]
	public class Issue13819 : TestContentPage
	{
		const string IssueInstructions = "1) Type some text in the entry field. 2) click the clear text button on the right. 3) If the clear button works the bug is fixed";
		Entry _entry;
		protected override void Init()
		{
			var stackLayout = new StackLayout();
			_entry = new Entry();
			_entry.ClearButtonVisibility = ClearButtonVisibility.Never;
			_entry.TextChanged += EntryOnTextChanged;
			stackLayout.Children.Add(_entry);

			Content = stackLayout;
		}

		void EntryOnTextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(e.NewTextValue))
			{
				_entry.ClearButtonVisibility = ClearButtonVisibility.Never;
			}
			else
			{
				_entry.ClearButtonVisibility = ClearButtonVisibility.WhileEditing;
			}
		}
	}
}