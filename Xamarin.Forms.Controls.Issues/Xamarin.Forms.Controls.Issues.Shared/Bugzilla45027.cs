using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 45027, "App crashes when double tapping on ToolbarItem or MenuItem very quickly", PlatformAffected.Android)]
	public class Bugzilla45027 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		const string TOOLBAR_ACTION_TEXT = "Action";
		const string TOOLBAR_DELETE_TEXT = "Delete";

		protected override void Init()
		{
			var list = new List<int>();
			for (var i = 0; i < 10; i++)
				list.Add(i);

			var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children =
				{
					new Label
					{
						Text = "Long tap list items to display context menu. Double tapping each action rapidly should not crash.",
						HorizontalTextAlignment = TextAlignment.Center
					}
				}
			};

			var listView = new ListView
			{
				ItemsSource = list,
				ItemTemplate = new DataTemplate(() =>
				{
					var label = new Label();
					label.SetBinding(Label.TextProperty, new Binding("."));

					return new ViewCell
					{
						View = new ContentView
						{
							Content = label,
						},
						ContextActions = { new MenuItem
						{
							Text = TOOLBAR_ACTION_TEXT
						},
						new MenuItem
						{
							Text = TOOLBAR_DELETE_TEXT,
							IsDestructive = true
						} }
					};
				})
			};
			stackLayout.Children.Add(listView);

			Content = stackLayout;
		}

#if UITEST
		[Test]
		public void Bugzilla45027Test()
		{
			var firstItemList = "0";
			RunningApp.WaitForElement(q => q.Marked(firstItemList));

			RunningApp.TouchAndHold(q => q.Marked(firstItemList));
			RunningApp.WaitForElement(q => q.Marked(TOOLBAR_ACTION_TEXT));
			RunningApp.DoubleTap(q => q.Marked(TOOLBAR_ACTION_TEXT));

			RunningApp.TouchAndHold(q => q.Marked(firstItemList));
			RunningApp.WaitForElement(q => q.Marked(TOOLBAR_DELETE_TEXT));
			RunningApp.DoubleTap(q => q.Marked(TOOLBAR_DELETE_TEXT));
		}
#endif
	}
}