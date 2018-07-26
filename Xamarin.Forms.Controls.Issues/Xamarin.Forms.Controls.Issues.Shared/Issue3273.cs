using System;
using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Github, 3273, "Drag and drop reordering not firing CollectionChanged", PlatformAffected.UWP)]
	public class Issue3273 : TestContentPage
	{
		protected override void Init ()
		{
			var statusLabel = new Label();
			var actionLabel = new Label();
			var Items = new ObservableCollection<string>
			{
				"apple",
				"orange",
				"pear",
				"trash"
			};
			BindingContext = Items;

			Items.CollectionChanged += (_, e) =>
			{
				statusLabel.Text = "Success";
				actionLabel.Text = $"<{DateTime.Now.ToLongTimeString()}> {e.Action} action fired.";
			};
			Items.RemoveAt(3);
			Items.Move(0, 1);

			var listView = new ListView();
			listView.SetBinding(ListView.ItemsSourceProperty, ".");

			Content = new StackLayout
			{
				Children = {
					statusLabel,
					actionLabel,
					new Button {
						Text = "Move items",
						Command = new Command(() =>
						{
							actionLabel.Text = string.Empty;
							statusLabel.Text = "Failed";
							Items.Move(0, 1); })
					},
					listView
				}
			};
		}

#if UITEST
		[Test]
		public void Issue3273Test()
		{
			RunningApp.WaitForElement("Move items");
			RunningApp.Tap("Move items");
			RunningApp.WaitForElement("Success");
		}
#endif
	}
}
