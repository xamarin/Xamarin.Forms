using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5500, "[iOS] Editor with material visuals value binding not working on physical device",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Editor)]
#endif
	public class Issue5500 : TestContentPage
	{
		Editor editor;
		Entry entry;

		protected override void Init()
		{
			Visual = VisualMarker.Material;

			editor = new Editor();
			entry = new Entry();

			editor.SetBinding(Editor.TextProperty, "Text");
			editor.BindingContext = entry;
			editor.Placeholder = "Editor";
			editor.AutoSize = EditorAutoSizeOption.TextChanges;
			editor.AutomationId = "EditorAutomationId";

			entry.SetBinding(Entry.TextProperty, "Text");
			entry.BindingContext = editor;
			entry.Placeholder = "Entry";
			entry.AutomationId = "EntryAutomationId";

			Content = new StackLayout()
			{
				Children =
				{
					new Label(){ Text = "Typing into either text field should change the other field to match" },
					entry, 
					editor
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Device.BeginInvokeOnMainThread(async () =>
			{
				GarbageCollectionHelper.Collect();
				editor.Text = "First Test";
				await Task.Delay(1);
				if(editor.Text != entry.Text)
				{
					editor.Text = "Test has failed";
					entry.Text = "Test has failed";
					return;
				}

				entry.Text = "Second Test";
				await Task.Delay(1);
				if (editor.Text != entry.Text)
				{
					editor.Text = "Test has failed";
					entry.Text = "Test has failed";
					return;
				}

				await Task.Delay(1);
				editor.Text = "Success";
				entry.Text = "Success";
			});
		}

#if UITEST
		[Test]
		public void VerifyEditorTextChangeEventsAreFiring()
		{
			RunningApp.WaitForElement("Success");
		}
#endif
	}
}
