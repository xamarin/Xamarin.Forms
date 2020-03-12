using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 9580, "[Bug] CollectionView - iOS - Crash when adding first item to empty item group", 
		PlatformAffected.iOS)]
	public class Issue9580 : TestContentPage
	{
		const string Success = "Success";
		const string Test9580 = "9580";
		const string Test9686 = "9686";

		protected override void Init()
		{
			var layout = new StackLayout();

			var cv = new CollectionView
			{
				IsGrouped = true
			};

			var groups = new ObservableCollection<_9580Group>()
			{
				new _9580Group() { Name = "One" }, new _9580Group(){ Name = "Two" }
			};

			cv.ItemTemplate = new DataTemplate(() => {
				var label = new Label();
				label.SetBinding(Label.TextProperty, new Binding("Text"));
				return label;
			});

			cv.GroupHeaderTemplate = new DataTemplate(() => {
				var label = new Label();
				label.SetBinding(Label.TextProperty, new Binding("Name"));
				return label;
			});

			cv.ItemsSource = groups;

			var instructions = new Label { Text = $"Tap the button for the issue to test. The application doesn't crash, this test has passed."};

			var result = new Label { };

			var button = new Button { Text = Test9580 };
			button.Clicked += (sender, args) => {
				groups[0].Add(new _9580Item { Text = "An Item" });
				result.Text = Success;
			};

			var button2 = new Button { Text = Test9686 };
			button2.Clicked += (sender, args) => {
				var group = groups[0];
				groups.Remove(group);
				group.Add(new _9580Item { Text = "An Item" });
				groups.Insert(0, group);
				result.Text = Success;
			};

			layout.Children.Add(instructions);
			layout.Children.Add(result);
			layout.Children.Add(button);
			layout.Children.Add(button2);
			layout.Children.Add(cv);

			Content = layout;
		}

		class _9580Item
		{ 
			public string Text { get; set; }
		}

		class _9580Group : ObservableCollection<_9580Item> 
		{
			public string Name { get; set; }
		}

#if UITEST
		[Category(UITestCategories.CollectionView)]
		[Test]
		public void AllEmptyGroupsShouldNotCrashOnItemInsert()
		{
			RunningApp.WaitForElement(Test9580);
			RunningApp.Tap(Test9580);
			RunningApp.WaitForElement(Success);
		}

		[Category(UITestCategories.CollectionView)]
		[Test]
		public void AddRemoveEmptyGroupsShouldNotCrashOnInsert()
		{
			RunningApp.WaitForElement(Test9686);
			RunningApp.Tap(Test9686);
			RunningApp.WaitForElement(Success);
		}
#endif
	}


}
