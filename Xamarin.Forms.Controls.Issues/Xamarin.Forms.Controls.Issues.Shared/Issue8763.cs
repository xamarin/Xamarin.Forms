using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using System.ComponentModel;
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
using System.Linq;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8763, "ContextAction Button on ListView Cell on iOS passes Tap to Cell when CanExecute is false",
			PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.ListView)]
#endif
	public class Issue8763 : TestContentPage
	{
		private int _commandExecutedCount = 0;
		private int _itemTappedCount = 0;

		private Label _label = new Label()
		{
			AutomationId = "Label"
		};

		protected override void Init()
		{
			ListView listView = new ListView()
			{
				ItemsSource = new[] { "1" },
				ItemTemplate = new DataTemplate(() =>
				{
					ViewCell cells = new ViewCell();

					cells.ContextActions.Add(new MenuItem()
					{
						Text = "Remove",
						Command = new Command<string>((string item) =>
						{
							_commandExecutedCount++;
							UpdateLabel();
						}, (string item) =>
						{
							return false;
						})
					});

					cells.View = new StackLayout()
					{
						Children =
						{
							new Label()
							{
								Text = "Item",
								AutomationId = "ListViewItem"
							}
						}
					};

					return cells;
				})

			};

			listView.ItemTapped += delegate
			{
				_itemTappedCount++;
				UpdateLabel();
			};

			StackLayout stackLayout = new StackLayout();

			stackLayout.Children.Add(_label);
			stackLayout.Children.Add(listView);

			Content = stackLayout;

			UpdateLabel();
		}

		private void UpdateLabel()
		{
			_label.Text = $"ItemTappedCount {_itemTappedCount} CommandExecutedCount {_commandExecutedCount}";
		}

#if UITEST && __IOS__
		[Test]
		public void TapDisabledContextAction()
		{
			RunningApp.WaitForElement("ListViewItem");
			RunningApp.ActivateContextMenu("ListViewItem");

			RunningApp.WaitForElement(c => c.Marked("Remove"));
			RunningApp.Tap(c => c.Marked("Remove"));

			Assert.AreEqual("ItemTappedCount 0 CommandExecutedCount 0", RunningApp.Query(c => c.Marked("Label")).First().Text);
		}
#endif
	}
}
