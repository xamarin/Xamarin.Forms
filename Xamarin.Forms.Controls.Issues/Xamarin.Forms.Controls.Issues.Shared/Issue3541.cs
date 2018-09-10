using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3541, "[WPF] Fix Local store not persistant when restarting App", PlatformAffected.WPF)]
	public class Issue3541 : TestContentPage
	{
		Entry _entry;

		protected override void Init()
		{
			var stack = new StackLayout();

			_entry = new Entry
			{
				Placeholder = "Enter a text then click on save",
				Text = GetText()
			};

			Button saveButton = new Button
			{
				Text = "Save"
			};
			saveButton.Clicked += SaveButton_Clicked;
			
			stack.Children.Add(_entry);
			stack.Children.Add(saveButton);
			Content = stack;
		}

		private void SaveButton_Clicked(object sender, System.EventArgs e)
		{
			this.Save(_entry.Text);
		}

		private async void Save(string text)
		{
			Application.Current.Properties[nameof(Issue3541)] = text;
			await Application.Current.SavePropertiesAsync();
		}

		private string GetText()
		{
			if (Application.Current.Properties.ContainsKey(nameof(Issue3541)))
				return Application.Current.Properties[nameof(Issue3541)] as string;

			return null;
		}
	}
}
