using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1678, "[Enhancement] Entry: Read-only entry", PlatformAffected.All)]
	public class Issue1678 
		: TestContentPage
	{
		protected override void Init()
		{
			var text = "Lorem Ipsum";

			var entryDefaults = new Entry { Text = text };
			var editorDefaults = new Editor { Text = text };
			var entryReadOnly = new Entry { Text = text, IsReadOnly = true };
			var editorReadOnly = new Editor { Text = text, IsReadOnly = true };
			var entryToggleable = new Entry { Text = text };
			var editorToggleable = new Editor { Text = text };
			var toggle = new Switch { IsToggled = true };

			var stackLayout = new StackLayout();
			stackLayout.Children.Add(new Label { Text = "Defaults" });
			stackLayout.Children.Add(entryDefaults);
			stackLayout.Children.Add(editorDefaults);
			stackLayout.Children.Add(new Label { Text = "Read Only" });
			stackLayout.Children.Add(entryReadOnly);
			stackLayout.Children.Add(editorReadOnly);
			stackLayout.Children.Add(new Label { Text = "Toggleable is read only" });
			stackLayout.Children.Add(entryToggleable);
			stackLayout.Children.Add(editorToggleable);
			stackLayout.Children.Add(toggle);

			toggle.Toggled += (_, b) =>
			{
				entryToggleable.IsReadOnly = b.Value;
				editorToggleable.IsReadOnly = b.Value;
			};

			stackLayout.Padding = new Thickness(0, 20, 0, 0);
			Content = stackLayout;
		}
	}
}
