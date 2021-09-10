using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8129, "[Bug] Adding children to iOS VisualElementPackager has O(N^2) performance and thrashes the native layer", PlatformAffected.iOS)]
	public class Issue8129 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Page with too many elements (2000) Tests";

			var grid = new Grid();
			var stackLayout = new StackLayout();

			var addChildrenCommand = new Command((parameter) =>
			{
				Enumerable
				.Range(0, 2000)
				.Select(_ => new Label() { HeightRequest = 200, Text = $"I am Label #{_}" })
				.ForEach(x => stackLayout.Children.Add(x));
			});

			var addChildrenButton = new Button
			{
				Text = "Add 2000 Labels",
				Command = addChildrenCommand
			};

			var addZChildrenButton = new Button
			{
				Text = "Add BoxView on Top",
				Command = new Command((parameter) =>
				{
					grid.AddChild(new BoxView
					{
						HeightRequest = 300,
						WidthRequest = 300,
						BackgroundColor = Color.Green
					}, 0, 0, 1, 3);
				})
			};
			grid.AddChild(addChildrenButton, 0, 0);
			grid.AddChild(addZChildrenButton, 0, 1);
			grid.AddChild(stackLayout, 0, 2);

			Content = grid;
		}
#if UITEST
		[Test]
		public void AddTooManyContentsTest()
		{
			RunningApp.WaitForElement(q => q.Button("Add 2000 Labels"));
			RunningApp.Screenshot("Before adding 2000 Labels");
			RunningApp.Tap(q => q.Button("Add 2000 Labels"));
			RunningApp.WaitForElement(q => q.Marked("I am Label #1"));
			RunningApp.Screenshot("After adding 2000 Labels");
		}

		[Test]
		public void ZIndexAfterAddingContentsTest()
		{
			RunningApp.WaitForElement(q => q.Button("Add BoxView on Top"));
			RunningApp.Screenshot("Before adding BoxView on Top");
			RunningApp.Tap(q => q.Button("Add BoxView on Top"));
			RunningApp.WaitForNoElement(q => q.Button("Add BoxView on Top"));
			RunningApp.Screenshot("After adding BoxView on Top");
		}
#endif
	}
}
