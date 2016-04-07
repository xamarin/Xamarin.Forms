using System;

using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Bugzilla, 40173, "Android BoxView/Frame not clickthrough in ListView")]
	public class Bugzilla40173 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Content = new Label
			{
				AutomationId = "IssuePageLabel",
				Text = "See if I'm here"
			};

			// because Im like... really lazy with writing these tests
			var outputLabel = new Label();
			var testButton = new Button
			{
				Text = "Can't Touch This",
				AutomationId = "bz40173TestButton1"
			};

			testButton.Clicked += (sender, args) => outputLabel.Text = "Failed";

			var testGrid = new Grid
			{
				Children = {
					testButton,
					new BoxView
					{
						Color = Color.Pink.MultiplyAlpha(0.5)
					}
				}
			};

			var testListView = new ListView();
			var items = new[] { "Foo" };
			testListView.ItemsSource = items;
			testListView.ItemTemplate = new DataTemplate(() =>
			{
				ViewCell result = new ViewCell
				{
					View = new Grid
					{
						Children =
						{
							new BoxView
							{
								AutomationId = "listTapTarget",
								Color = Color.Pink.MultiplyAlpha(0.5)
							}
						}
					}
				};


				return result;
			});

			testListView.ItemSelected += (sender, args) => outputLabel.Text = "ItemTapped";

			Content = new StackLayout
			{
				Children = { outputLabel, testGrid, testListView }
			};
		}

#if UITEST
		[Test]
		public void ButtonBlocked ()
		{
			RunningApp.Tap(q => q.Marked("bz40173TestButton1"));
			RunningApp.WaitForNoElement (q => q.Text ("Failed"));
		}

		[Test]
		public void ListViewSelectionWorks()
		{
			RunningApp.Tap(q => q.Marked("listTapTarget"));
			RunningApp.WaitForElement (q => q.Text ("ItemTapped"));
		}
#endif
	}
}