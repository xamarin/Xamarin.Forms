using System;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Github, 2963, "Disabling Editor in iOS does not disable entry of text")]
	public class Issue2963 : TestContentPage
	{
		protected override void Init ()
		{
			var disabledEditor = new Editor {
				Text = "You should not be able to edit me",
				IsEnabled = false
			};

			BindingContext = disabledEditor;
			var focusedLabel = new Label();
			focusedLabel.SetBinding (Label.TextProperty, "IsFocused");

			Content = new StackLayout {
				Children = {
					disabledEditor,
					focusedLabel,
				}
			};
		}
	}
}
